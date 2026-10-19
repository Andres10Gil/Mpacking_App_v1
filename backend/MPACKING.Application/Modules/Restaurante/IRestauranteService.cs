using MPACKING.Application.Modules.Redencion;

namespace MPACKING.Application.Modules.Restaurante;

public interface IRestauranteService
{
    /// <summary>
    /// Valida que el código pertenezca a un beneficio de ESTE restaurante
    /// antes de delegar la aprobación real a IRedencionService.
    /// </summary>
    Task<RedencionResponse> ValidarCodigoAsync(
        int idUsuario, string codigoUnico, CancellationToken ct = default);

    Task<ResumenRestauranteResponse> ObtenerResumenAsync(int idUsuario, CancellationToken ct = default);

    Task<IReadOnlyList<BeneficioRestauranteResponse>> ListarBeneficiosAsync(
        int idUsuario, CancellationToken ct = default);

    Task<BeneficioRestauranteResponse> CrearBeneficioAsync(
        int idUsuario, CrearBeneficioRequest request, CancellationToken ct = default);

    Task<BeneficioRestauranteResponse> CambiarEstadoBeneficioAsync(
        int idUsuario, int idBeneficio, bool activar, CancellationToken ct = default);

    Task<HistorialPagosResponse> ObtenerHistorialAsync(int idUsuario, CancellationToken ct = default);
}
