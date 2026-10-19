using MPACKING.Application.Common.Interfaces;
using MPACKING.Application.Modules.Redencion;
using MPACKING.Domain.Entities;
using MPACKING.Domain.Exceptions;
using RestauranteEntity = MPACKING.Domain.Entities.Restaurante;

namespace MPACKING.Application.Modules.Restaurante;

/// <summary>
/// Panel del restaurante aliado: resumen de pagos recibidos, gestión de
/// beneficios y validación de códigos de canje. RF-14, RF-19, RF-20.
/// </summary>
public class RestauranteService : IRestauranteService
{
    private const int CantidadRecientes = 6;
    private const int CantidadHistorial = 40;

    private readonly IRestauranteRepository _restaurantes;
    private readonly IBeneficioRepository _beneficios;
    private readonly IRedencionRepository _redenciones;
    private readonly IPagoRepository _pagos;
    private readonly IRedencionService _redencionService;
    private readonly IUnitOfWork _uow;
    private readonly IRelojSistema _reloj;

    public RestauranteService(
        IRestauranteRepository restaurantes,
        IBeneficioRepository beneficios,
        IRedencionRepository redenciones,
        IPagoRepository pagos,
        IRedencionService redencionService,
        IUnitOfWork uow,
        IRelojSistema reloj)
    {
        _restaurantes = restaurantes;
        _beneficios = beneficios;
        _redenciones = redenciones;
        _pagos = pagos;
        _redencionService = redencionService;
        _uow = uow;
        _reloj = reloj;
    }

    public async Task<ResumenRestauranteResponse> ObtenerResumenAsync(
        int idUsuario, CancellationToken ct = default)
    {
        var restaurante = await ObtenerRestauranteAsync(idUsuario, ct);
        var inicioDia = _reloj.Ahora.Date;

        var recibidoHoy = await _pagos.SumarMontoNetoDesdeAsync(restaurante.IdRestaurante, inicioDia, ct);
        var redencionesHoy = await _pagos.ContarAprobadosDesdeAsync(restaurante.IdRestaurante, inicioDia, ct);
        var ecopesosHoy = await _pagos.SumarEcopesosValidadosDesdeAsync(restaurante.IdRestaurante, inicioDia, ct);

        var ultimas = await _redenciones.ListarPorRestauranteAsync(
            restaurante.IdRestaurante, CantidadRecientes, ct);

        return new ResumenRestauranteResponse(
            restaurante.Nombre, restaurante.Direccion,
            recibidoHoy, redencionesHoy, ecopesosHoy,
            ultimas.Select(MapearReciente).ToList());
    }

    public async Task<IReadOnlyList<BeneficioRestauranteResponse>> ListarBeneficiosAsync(
        int idUsuario, CancellationToken ct = default)
    {
        var restaurante = await ObtenerRestauranteAsync(idUsuario, ct);
        var beneficios = await _beneficios.ListarTodosPorRestauranteAsync(restaurante.IdRestaurante, ct);
        return beneficios.Select(MapearBeneficio).ToList();
    }

    public async Task<BeneficioRestauranteResponse> CrearBeneficioAsync(
        int idUsuario, CrearBeneficioRequest request, CancellationToken ct = default)
    {
        var restaurante = await ObtenerRestauranteAsync(idUsuario, ct);

        var beneficio = new Beneficio(
            restaurante.IdRestaurante, request.Descripcion, request.EcopesosCosto);

        await _beneficios.AgregarAsync(beneficio, ct);
        await _uow.GuardarCambiosAsync(ct);

        return MapearBeneficio(beneficio);
    }

    public async Task<BeneficioRestauranteResponse> CambiarEstadoBeneficioAsync(
        int idUsuario, int idBeneficio, bool activar, CancellationToken ct = default)
    {
        var restaurante = await ObtenerRestauranteAsync(idUsuario, ct);

        var beneficio = await _beneficios.ObtenerPorIdAsync(idBeneficio, ct)
            ?? throw new NoEncontradoException("beneficio", idBeneficio);

        if (beneficio.IdRestaurante != restaurante.IdRestaurante)
            throw new ReglaNegocioException("Este beneficio no pertenece a tu restaurante.");

        if (activar) beneficio.Reactivar(); else beneficio.Pausar();
        await _uow.GuardarCambiosAsync(ct);

        return MapearBeneficio(beneficio);
    }

    public async Task<HistorialPagosResponse> ObtenerHistorialAsync(
        int idUsuario, CancellationToken ct = default)
    {
        var restaurante = await ObtenerRestauranteAsync(idUsuario, ct);
        var inicioMes = new DateTime(
            _reloj.Ahora.Year, _reloj.Ahora.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var totalMes = await _pagos.SumarMontoNetoDesdeAsync(restaurante.IdRestaurante, inicioMes, ct);
        var cantidadMes = await _pagos.ContarAprobadosDesdeAsync(restaurante.IdRestaurante, inicioMes, ct);
        var pagos = await _pagos.ListarPorRestauranteAsync(restaurante.IdRestaurante, CantidadHistorial, ct);

        return new HistorialPagosResponse(
            totalMes, cantidadMes, pagos.Select(MapearPago).ToList());
    }

    public async Task<RedencionResponse> ValidarCodigoAsync(
        int idUsuario, string codigoUnico, CancellationToken ct = default)
    {
        var restaurante = await ObtenerRestauranteAsync(idUsuario, ct);

        var redencion = await _redenciones.ObtenerPorCodigoAsync(codigoUnico, ct)
            ?? throw new NoEncontradoException("redención", codigoUnico);

        if (redencion.Beneficio.IdRestaurante != restaurante.IdRestaurante)
            throw new ReglaNegocioException("Este código no corresponde a tu restaurante.");

        return await _redencionService.AprobarAsync(codigoUnico, ct);
    }

    private async Task<RestauranteEntity> ObtenerRestauranteAsync(int idUsuario, CancellationToken ct) =>
        await _restaurantes.ObtenerPorIdUsuarioAsync(idUsuario, ct)
            ?? throw new NoEncontradoException("restaurante", idUsuario);

    private static RedencionRecienteResponse MapearReciente(Domain.Entities.Redencion r) => new(
        r.Usuario.Nombre,
        $"{r.Beneficio.Descripcion} · {r.FechaSolicitud.ToLocalTime():dd/MM h:mm tt}",
        r.Estado.ToString(),
        $"{(r.EcopesosUsados * Pago.CopPorEcopeso):N0} COP");

    private static BeneficioRestauranteResponse MapearBeneficio(Beneficio b) => new(
        b.IdBeneficio, b.Descripcion, b.EcopesosCosto, b.Activo);

    private static PagoHistorialResponse MapearPago(Pago p) => new(
        p.Redencion.Usuario.Nombre,
        $"{p.Redencion.Beneficio.Descripcion} · {p.FechaPago.ToLocalTime():dd/MM h:mm tt}",
        $"{p.MontoNeto:N0} COP",
        p.Estado.ToString());
}
