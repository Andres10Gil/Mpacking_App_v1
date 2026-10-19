using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using MPACKING.App.Models;

namespace MPACKING.App.Services;

public class AdminApiService
{
    private static readonly JsonSerializerOptions JsonOpciones = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly HttpClient _http;
    private readonly SesionService _sesion;

    public AdminApiService(HttpClient http, SesionService sesion)
    {
        _http = http;
        _sesion = sesion;
    }

    public async Task<ResumenAdminResponse> ObtenerResumenAsync(CancellationToken ct = default)
    {
        AplicarToken();

        HttpResponseMessage respuesta;
        try
        {
            respuesta = await _http.GetAsync("api/admin/resumen", ct);
        }
        catch (Exception)
        {
            throw new ApiException("No se pudo conectar con el servidor. Revisa tu conexion.");
        }

        if (!respuesta.IsSuccessStatusCode)
            throw new ApiException("No se pudo cargar el resumen del sistema.");

        return (await respuesta.Content.ReadFromJsonAsync<ResumenAdminResponse>(JsonOpciones, ct))!;
    }

    public async Task<List<UsuarioAdminResponse>> ListarUsuariosAsync(
        string? busqueda, CancellationToken ct = default)
    {
        AplicarToken();

        var url = "api/admin/usuarios?tamano=50";
        if (!string.IsNullOrWhiteSpace(busqueda))
            url += $"&busqueda={Uri.EscapeDataString(busqueda)}";

        HttpResponseMessage respuesta;
        try
        {
            respuesta = await _http.GetAsync(url, ct);
        }
        catch (Exception)
        {
            throw new ApiException("No se pudo conectar con el servidor. Revisa tu conexion.");
        }

        if (!respuesta.IsSuccessStatusCode)
            throw new ApiException("No se pudo cargar la lista de usuarios.");

        return (await respuesta.Content.ReadFromJsonAsync<List<UsuarioAdminResponse>>(JsonOpciones, ct))!;
    }

    public async Task<ReporteAmbientalResponse> ObtenerReporteAsync(CancellationToken ct = default)
    {
        AplicarToken();

        HttpResponseMessage respuesta;
        try
        {
            respuesta = await _http.GetAsync("api/admin/reportes", ct);
        }
        catch (Exception)
        {
            throw new ApiException("No se pudo conectar con el servidor. Revisa tu conexion.");
        }

        if (!respuesta.IsSuccessStatusCode)
            throw new ApiException("No se pudo cargar el reporte ambiental.");

        return (await respuesta.Content.ReadFromJsonAsync<ReporteAmbientalResponse>(JsonOpciones, ct))!;
    }

    private void AplicarToken() =>
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _sesion.AccessToken);
}
