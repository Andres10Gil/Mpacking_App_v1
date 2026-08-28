using MPACKING.Application.Common.Interfaces;
using MPACKING.Domain.Entities;
using MPACKING.Domain.Exceptions;

namespace MPACKING.Application.Modules.Reciclaje;

/// <summary>
/// Escaneo de QR y registro de reciclajes. Implementa los RF-05 a RF-09.
///
/// Nota de diseño: este servicio NO actualiza el saldo del usuario ni marca
/// el QR como usado. Eso lo hacen los triggers T1 y T3 en PostgreSQL al
/// insertar la transacción. Duplicar esa lógica aquí causaría doble conteo.
/// El servicio solo inserta y luego relee el saldo ya actualizado.
/// </summary>
public class ReciclajeService : IReciclajeService
{
    private const decimal CopPorEcopeso = 10m;

    private readonly IQrRepository _qrs;
    private readonly IMaterialRepository _materiales;
    private readonly ITransaccionRepository _transacciones;
    private readonly IUsuarioRepository _usuarios;
    private readonly IUnitOfWork _uow;

    public ReciclajeService(IQrRepository qrs, IMaterialRepository materiales,
                            ITransaccionRepository transacciones,
                            IUsuarioRepository usuarios, IUnitOfWork uow)
    {
        _qrs = qrs;
        _materiales = materiales;
        _transacciones = transacciones;
        _usuarios = usuarios;
        _uow = uow;
    }

    /// <summary>
    /// Consulta el QR escaneado sin registrarlo todavía. Permite mostrar al
    /// usuario qué material es y cuántos ecopesos ganaría antes de confirmar.
    /// </summary>
    public async Task<QrInfoResponse> ConsultarQrAsync(
        string codigoHash, CancellationToken ct = default)
    {
        var qr = await _qrs.ObtenerPorCodigoAsync(codigoHash, ct)
            ?? throw new NoEncontradoException("código QR", codigoHash);

        var material = await _materiales.ObtenerPorIdAsync(qr.IdMaterial, ct)
            ?? throw new NoEncontradoException("material", qr.IdMaterial);

        return new QrInfoResponse(
            IdQr: qr.IdQr,
            Material: material.Nombre,
            Categoria: material.Categoria,
            PesoKg: qr.PesoKg,
            EcopesosEstimados: material.CalcularEcopesos(qr.PesoKg),
            Co2Evitado: material.CalcularCo2Evitado(qr.PesoKg),
            Restaurante: qr.Restaurante?.Nombre ?? "No disponible",
            FechaExpiracion: qr.FechaExpiracion,
            EsCanjeable: qr.EsCanjeable);
    }

    /// <summary>
    /// Registra el reciclaje. El reciclador confirma el peso real, que puede
    /// diferir del peso teórico del empaque (RF-08: validación cruzada).
    /// </summary>
    public async Task<ReciclajeResponse> ConfirmarAsync(
        int idUsuario, ConfirmarReciclajeRequest request,
        string? ip, CancellationToken ct = default)
    {
        var qr = await _qrs.ObtenerPorCodigoAsync(request.CodigoHash, ct)
            ?? throw new NoEncontradoException("código QR", request.CodigoHash);

        // Validación temprana para dar un mensaje claro.
        // El trigger T4 es la garantía final a nivel de base de datos.
        qr.ValidarCanjeable();

        var material = await _materiales.ObtenerPorIdAsync(qr.IdMaterial, ct)
            ?? throw new NoEncontradoException("material", qr.IdMaterial);

        var peso = request.PesoConfirmadoKg;
        var ecopesos = material.CalcularEcopesos(peso);

        var transaccion = new Transaccion(
            idUsuario: idUsuario,
            idQr: qr.IdQr,
            idReciclador: request.IdReciclador,
            ecopesosGanados: ecopesos,
            pesoKg: peso,
            precioCopKg: material.PrecioMinCopKg,
            ipEscaneo: ip);

        await _transacciones.AgregarAsync(transaccion, ct);

        // Aquí se disparan T4 (valida), T1 (acredita) y T3 (marca usado)
        await _uow.GuardarCambiosAsync(ct);

        // El saldo ya fue actualizado por el trigger T1: lo releemos
        var saldo = await _usuarios.ObtenerSaldoAsync(idUsuario, ct);

        return new ReciclajeResponse(
            IdTransaccion: transaccion.IdTransaccion,
            EcopesosGanados: ecopesos,
            SaldoActual: saldo,
            Co2Evitado: material.CalcularCo2Evitado(peso),
            Material: material.Nombre,
            PesoKg: peso);
    }

    public async Task<BilleteraResponse> ObtenerBilleteraAsync(
        int idUsuario, int pagina = 1, int tamano = 20, CancellationToken ct = default)
    {
        var saldo = await _usuarios.ObtenerSaldoAsync(idUsuario, ct);
        var totalKg = await _transacciones.TotalKgRecicladosAsync(idUsuario, ct);
        var movimientos = await _transacciones.ListarPorUsuarioAsync(
            idUsuario, pagina, tamano, ct);

        return new BilleteraResponse(
            SaldoEcopesos: saldo,
            EquivalenteCop: saldo * CopPorEcopeso,
            TotalKgReciclados: totalKg,
            Movimientos: movimientos.Select(t => new MovimientoResponse(
                t.IdTransaccion,
                t.Qr?.Material?.Nombre ?? "Reciclaje",
                t.PesoKg,
                t.EcopesosGanados,
                t.Fecha)).ToList());
    }
}
