using MPACKING.Application.Common.Interfaces;
using MPACKING.Domain.Entities;
using RedencionEntity = MPACKING.Domain.Entities.Redencion;
using MPACKING.Domain.Exceptions;

namespace MPACKING.Application.Modules.Redencion;

/// <summary>
/// Canje de ecopesos y pago automático al restaurante.
/// Implementa los RF-10, RF-11, RF-12 y RF-20.
///
/// Nota de diseño: el descuento de ecopesos lo hace el trigger T2 al pasar
/// la redención a Aprobada. Si Wompi rechaza el pago, la redención nunca
/// se aprueba y por tanto los ecopesos no se descuentan (RF-12).
/// </summary>
public class RedencionService : IRedencionService
{
    private readonly IRedencionRepository _redenciones;
    private readonly IBeneficioRepository _beneficios;
    private readonly IUsuarioRepository _usuarios;
    private readonly IPagoRepository _pagos;
    private readonly IPasarelaPago _pasarela;
    private readonly IUnitOfWork _uow;

    public RedencionService(IRedencionRepository redenciones,
                            IBeneficioRepository beneficios,
                            IUsuarioRepository usuarios,
                            IPagoRepository pagos,
                            IPasarelaPago pasarela,
                            IUnitOfWork uow)
    {
        _redenciones = redenciones;
        _beneficios = beneficios;
        _usuarios = usuarios;
        _pagos = pagos;
        _pasarela = pasarela;
        _uow = uow;
    }

    public async Task<RedencionResponse> SolicitarAsync(
        int idUsuario, SolicitarRedencionRequest request, CancellationToken ct = default)
    {
        var beneficio = await _beneficios.ObtenerPorIdAsync(request.IdBeneficio, ct)
            ?? throw new NoEncontradoException("beneficio", request.IdBeneficio);

        if (!beneficio.EstaDisponible)
            throw new ReglaNegocioException("Este beneficio ya no está disponible.");

        var saldo = await _usuarios.ObtenerSaldoAsync(idUsuario, ct);
        if (saldo < beneficio.EcopesosCosto)
            throw new ReglaNegocioException(
                $"Saldo insuficiente. Necesitas {beneficio.EcopesosCosto} ecopesos " +
                $"y tienes {saldo}.");

        var codigo = GenerarCodigoUnico();
        var redencion = new RedencionEntity(
            idUsuario, beneficio.IdBeneficio, codigo, beneficio.EcopesosCosto);

        await _redenciones.AgregarAsync(redencion, ct);
        await _uow.GuardarCambiosAsync(ct);

        return Mapear(redencion, beneficio, saldo);
    }

    /// <summary>
    /// El restaurante valida el código. Se cobra vía Wompi y solo si el pago
    /// es exitoso se aprueba la redención, disparando el trigger T2.
    /// </summary>
    public async Task<RedencionResponse> AprobarAsync(
        string codigoUnico, CancellationToken ct = default)
    {
        var redencion = await _redenciones.ObtenerPorCodigoAsync(codigoUnico, ct)
            ?? throw new NoEncontradoException("redención", codigoUnico);

        var pago = Pago.DesdeEcopesos(redencion.IdRedencion, redencion.EcopesosUsados);

        var resultado = await _pasarela.TransferirAsync(
            pago.MontoNeto, redencion.CodigoUnico, ct);

        if (!resultado.Exitoso)
        {
            pago.MarcarRechazado(resultado.TransaccionId);
            await _pagos.AgregarAsync(pago, ct);
            await _uow.GuardarCambiosAsync(ct);

            // La redención queda Pendiente: T2 no se dispara,
            // los ecopesos no se descuentan y el usuario puede reintentar.
            throw new ReglaNegocioException(
                $"El pago no pudo procesarse: {resultado.MensajeError}. " +
                "Tus ecopesos no fueron descontados.");
        }

        pago.MarcarAprobado(resultado.TransaccionId!);
        await _pagos.AgregarAsync(pago, ct);

        redencion.Aprobar();          // el trigger T2 descuenta los ecopesos
        await _uow.GuardarCambiosAsync(ct);

        var saldo = await _usuarios.ObtenerSaldoAsync(redencion.IdUsuario, ct);
        return Mapear(redencion, redencion.Beneficio, saldo);
    }

    public async Task<IReadOnlyList<RedencionResponse>> ListarPorUsuarioAsync(
        int idUsuario, CancellationToken ct = default)
    {
        var lista = await _redenciones.ListarPorUsuarioAsync(idUsuario, ct);
        var saldo = await _usuarios.ObtenerSaldoAsync(idUsuario, ct);
        return lista.Select(r => Mapear(r, r.Beneficio, saldo)).ToList();
    }

    private static string GenerarCodigoUnico() =>
        $"RED-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";

    private static RedencionResponse Mapear(
        RedencionEntity r, Beneficio? b, int saldo) => new(
            Id: r.IdRedencion,
            CodigoUnico: r.CodigoUnico,
            Beneficio: b?.Descripcion ?? "No disponible",
            Restaurante: b?.Restaurante?.Nombre ?? "No disponible",
            EcopesosUsados: r.EcopesosUsados,
            EquivalenteCop: r.EcopesosUsados * Pago.CopPorEcopeso,
            Estado: r.Estado.ToString(),
            SaldoRestante: saldo);
}
