using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using MPACKING.App.Models;

namespace MPACKING.App.Services;

public class GeoApiService
{
    private static readonly JsonSerializerOptions JsonOpciones = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly HttpClient _http;
    private readonly SesionService _sesion;

    public GeoApiService(HttpClient http, SesionService sesion)
    {
        _http = http;
        _sesion = sesion;
    }

    public async Task<List<RestauranteCercanoDto>> BuscarRestaurantesCercanosAsync(
        decimal lat, decimal lng, double radioKm = 15, CancellationToken ct = default)
    {
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _sesion.AccessToken);

        var culturaInvariante = CultureInfo.InvariantCulture;
        var ruta = $"api/geo/restaurantes/cercanos?lat={lat.ToString(culturaInvariante)}" +
                   $"&lng={lng.ToString(culturaInvariante)}&radioKm={radioKm.ToString(culturaInvariante)}";

        HttpResponseMessage respuesta;
        try
        {
            respuesta = await _http.GetAsync(ruta, ct);
        }
        catch (Exception)
        {
            throw new ApiException("No se pudo conectar con el servidor. Revisa tu conexion.");
        }

        if (!respuesta.IsSuccessStatusCode)
            throw new ApiException("No se pudieron cargar los restaurantes cercanos.");

        return (await respuesta.Content.ReadFromJsonAsync<List<RestauranteCercanoDto>>(JsonOpciones, ct))!;
    }

    public async Task ReportarPosicionAsync(
        decimal latitud, decimal longitud, CancellationToken ct = default)
    {
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _sesion.AccessToken);

        var cuerpo = new { Latitud = latitud, Longitud = longitud, PrecisionM = 10, VelocidadKmh = 0m };

        HttpResponseMessage respuesta;
        try
        {
            respuesta = await _http.PostAsJsonAsync("api/geo/posicion", cuerpo, JsonOpciones, ct);
        }
        catch (Exception)
        {
            throw new ApiException("No se pudo conectar con el servidor. Revisa tu conexion.");
        }

        if (!respuesta.IsSuccessStatusCode)
            throw new ApiException("No se pudo reportar tu ubicacion.");
    }

    /// <summary>Devuelve null si el reciclador no esta en ruta ahora mismo (no es un error).</summary>
    public async Task<PosicionResponse?> ObtenerPosicionRecicladorAsync(int idReciclador, CancellationToken ct = default)
    {
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _sesion.AccessToken);

        HttpResponseMessage respuesta;
        try
        {
            respuesta = await _http.GetAsync($"api/geo/reciclador/{idReciclador}", ct);
        }
        catch (Exception)
        {
            return null;
        }

        if (respuesta.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!respuesta.IsSuccessStatusCode)
            return null;

        return await respuesta.Content.ReadFromJsonAsync<PosicionResponse>(JsonOpciones, ct);
    }
}
