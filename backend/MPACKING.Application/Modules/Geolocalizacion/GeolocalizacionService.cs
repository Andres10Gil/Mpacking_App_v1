using MPACKING.Application.Common.Interfaces;
using MPACKING.Domain.Entities;

namespace MPACKING.Application.Modules.Geolocalizacion;

/// <summary>
/// Posición del reciclador en tiempo real y búsqueda de restaurantes cercanos.
/// Implementa los RF-18, RF-19 y RF-23.
///
/// La última posición vive en caché con vigencia de 60 segundos: si el
/// reciclador se desconecta, el pin desaparece solo. El histórico va a la
/// base de datos y se purga a los 7 días (Ley 1581 de 2012).
/// </summary>
public class GeolocalizacionService : IGeolocalizacionService
{
    private const int MaxResultados = 20;
    private static readonly TimeSpan VigenciaPosicion = TimeSpan.FromSeconds(60);

    private readonly IUbicacionRepository _ubicaciones;
    private readonly IRestauranteRepository _restaurantes;
    private readonly INotificadorUbicacion _notificador;
    private readonly ICacheService _cache;
    private readonly IUnitOfWork _uow;

    public GeolocalizacionService(IUbicacionRepository ubicaciones,
                                  IRestauranteRepository restaurantes,
                                  INotificadorUbicacion notificador,
                                  ICacheService cache,
                                  IUnitOfWork uow)
    {
        _ubicaciones = ubicaciones;
        _restaurantes = restaurantes;
        _notificador = notificador;
        _cache = cache;
        _uow = uow;
    }

    public async Task ReportarPosicionAsync(int idReciclador,
        ReportarPosicionRequest request, CancellationToken ct = default)
    {
        var ubicacion = new UbicacionReciclador(
            idReciclador, request.Latitud, request.Longitud,
            request.PrecisionM, request.VelocidadKmh);

        await _ubicaciones.AgregarAsync(ubicacion, ct);
        await _uow.GuardarCambiosAsync(ct);

        var respuesta = new PosicionResponse(
            request.Latitud, request.Longitud,
            DateTime.UtcNow, ubicacion.EsPrecisionConfiable);

        await _cache.GuardarAsync(
            ClavePosicion(idReciclador), respuesta, VigenciaPosicion, ct);

        // Empuja la posición a los usuarios suscritos vía SignalR
        await _notificador.EnviarPosicionAsync(
            idReciclador, request.Latitud, request.Longitud, ct);
    }

    public Task<PosicionResponse?> ObtenerPosicionAsync(
        int idReciclador, CancellationToken ct = default) =>
        _cache.ObtenerAsync<PosicionResponse>(ClavePosicion(idReciclador), ct);

    public async Task<IReadOnlyList<RestauranteCercanoDto>> BuscarRestaurantesCercanosAsync(
        decimal latitud, decimal longitud, double radioKm = 5,
        CancellationToken ct = default)
    {
        var restaurantes = await _restaurantes.ListarActivosAsync(ct);

        return restaurantes
            .Where(r => r.TieneUbicacion)
            .Select(r => new
            {
                Restaurante = r,
                Distancia = DistanciaHaversineKm(
                    (double)latitud, (double)longitud,
                    (double)r.Latitud!.Value, (double)r.Longitud!.Value)
            })
            .Where(x => x.Distancia <= radioKm)
            .OrderBy(x => x.Distancia)
            .Take(MaxResultados)
            .Select(x => new RestauranteCercanoDto(
                x.Restaurante.IdRestaurante,
                x.Restaurante.Nombre,
                x.Restaurante.Direccion,
                x.Restaurante.Latitud!.Value,
                x.Restaurante.Longitud!.Value,
                Math.Round(x.Distancia, 2)))
            .ToList();
    }

    private static string ClavePosicion(int idReciclador) => $"gps:reciclador:{idReciclador}";

    /// <summary>
    /// Distancia entre dos puntos GPS sobre la superficie terrestre.
    /// Se calcula en el servidor para no depender de PostGIS.
    /// </summary>
    private static double DistanciaHaversineKm(
        double lat1, double lon1, double lat2, double lon2)
    {
        const double radioTierraKm = 6371;

        var dLat = GradosARadianes(lat2 - lat1);
        var dLon = GradosARadianes(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(GradosARadianes(lat1)) * Math.Cos(GradosARadianes(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        return radioTierraKm * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    private static double GradosARadianes(double grados) => grados * Math.PI / 180;
}
