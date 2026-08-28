namespace MPACKING.Application.Modules.Geolocalizacion;

public interface IGeolocalizacionService
{
    Task ReportarPosicionAsync(int idReciclador,
        ReportarPosicionRequest request, CancellationToken ct = default);

    Task<PosicionResponse?> ObtenerPosicionAsync(
        int idReciclador, CancellationToken ct = default);

    Task<IReadOnlyList<RestauranteCercanoDto>> BuscarRestaurantesCercanosAsync(
        decimal latitud, decimal longitud, double radioKm = 5,
        CancellationToken ct = default);
}
