using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using MPACKING.App.Models;

namespace MPACKING.App.Services;

public class CatalogoApiService
{
    private static readonly JsonSerializerOptions JsonOpciones = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly HttpClient _http;
    private readonly SesionService _sesion;

    public CatalogoApiService(HttpClient http, SesionService sesion)
    {
        _http = http;
        _sesion = sesion;
    }

    public async Task<List<MaterialResponse>> ListarMaterialesAsync(CancellationToken ct = default)
    {
        AplicarToken();

        HttpResponseMessage respuesta;
        try
        {
            respuesta = await _http.GetAsync("api/catalogo/materiales", ct);
        }
        catch (Exception)
        {
            throw new ApiException("No se pudo conectar con el servidor. Revisa tu conexion.");
        }

        if (!respuesta.IsSuccessStatusCode)
            throw new ApiException("No se pudo cargar el catalogo de materiales.");

        return (await respuesta.Content.ReadFromJsonAsync<List<MaterialResponse>>(JsonOpciones, ct))!;
    }

    public async Task<GuiaLimpiezaResponse> ObtenerGuiaAsync(int idMaterial, CancellationToken ct = default)
    {
        AplicarToken();

        HttpResponseMessage respuesta;
        try
        {
            respuesta = await _http.GetAsync($"api/catalogo/materiales/{idMaterial}/guia", ct);
        }
        catch (Exception)
        {
            throw new ApiException("No se pudo conectar con el servidor. Revisa tu conexion.");
        }

        if (!respuesta.IsSuccessStatusCode)
            throw new ApiException("Este material todavia no tiene guia de limpieza.");

        return (await respuesta.Content.ReadFromJsonAsync<GuiaLimpiezaResponse>(JsonOpciones, ct))!;
    }

    private void AplicarToken() =>
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _sesion.AccessToken);
}
