using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using MPACKING.App.Models;

namespace MPACKING.App.Services;

public class RedencionApiService
{
    private static readonly JsonSerializerOptions JsonOpciones = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly HttpClient _http;
    private readonly SesionService _sesion;

    public RedencionApiService(HttpClient http, SesionService sesion)
    {
        _http = http;
        _sesion = sesion;
    }

    public async Task<List<BeneficioResponse>> ListarBeneficiosAsync(int idRestaurante, CancellationToken ct = default)
    {
        AplicarToken();

        HttpResponseMessage respuesta;
        try
        {
            respuesta = await _http.GetAsync($"api/redenciones/beneficios/{idRestaurante}", ct);
        }
        catch (Exception)
        {
            throw new ApiException("No se pudo conectar con el servidor. Revisa tu conexion.");
        }

        if (!respuesta.IsSuccessStatusCode)
            throw new ApiException("No se pudieron cargar los beneficios de este restaurante.");

        return (await respuesta.Content.ReadFromJsonAsync<List<BeneficioResponse>>(JsonOpciones, ct))!;
    }

    public async Task<RedencionResponse> SolicitarAsync(int idBeneficio, CancellationToken ct = default)
    {
        AplicarToken();

        HttpResponseMessage respuesta;
        try
        {
            respuesta = await _http.PostAsJsonAsync(
                "api/redenciones", new SolicitarRedencionRequest(idBeneficio), JsonOpciones, ct);
        }
        catch (Exception)
        {
            throw new ApiException("No se pudo conectar con el servidor. Revisa tu conexion.");
        }

        if (!respuesta.IsSuccessStatusCode)
        {
            var error = await respuesta.Content.ReadFromJsonAsync<ErrorApi>(JsonOpciones, ct);
            throw new ApiException(error?.Mensaje ?? "No se pudo solicitar el canje.");
        }

        return (await respuesta.Content.ReadFromJsonAsync<RedencionResponse>(JsonOpciones, ct))!;
    }

    public async Task<List<RedencionResponse>> ListarMisRedencionesAsync(CancellationToken ct = default)
    {
        AplicarToken();

        HttpResponseMessage respuesta;
        try
        {
            respuesta = await _http.GetAsync("api/redenciones/mias", ct);
        }
        catch (Exception)
        {
            throw new ApiException("No se pudo conectar con el servidor. Revisa tu conexion.");
        }

        if (!respuesta.IsSuccessStatusCode)
            throw new ApiException("No se pudo cargar tu historial de canjes.");

        return (await respuesta.Content.ReadFromJsonAsync<List<RedencionResponse>>(JsonOpciones, ct))!;
    }

    private void AplicarToken() =>
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _sesion.AccessToken);
}
