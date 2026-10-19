using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using MPACKING.App.Models;

namespace MPACKING.App.Services;

public class RecicladorApiService
{
    private static readonly JsonSerializerOptions JsonOpciones = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly HttpClient _http;
    private readonly SesionService _sesion;

    public RecicladorApiService(HttpClient http, SesionService sesion)
    {
        _http = http;
        _sesion = sesion;
    }

    public async Task<PerfilRecicladorResponse> ObtenerPerfilAsync(CancellationToken ct = default)
    {
        AplicarToken();

        HttpResponseMessage respuesta;
        try
        {
            respuesta = await _http.GetAsync("api/reciclador/perfil", ct);
        }
        catch (Exception)
        {
            throw new ApiException("No se pudo conectar con el servidor. Revisa tu conexion.");
        }

        if (!respuesta.IsSuccessStatusCode)
            throw new ApiException("No se pudo cargar tu perfil de reciclador.");

        return (await respuesta.Content.ReadFromJsonAsync<PerfilRecicladorResponse>(JsonOpciones, ct))!;
    }

    public async Task<HistorialRecicladorResponse> ObtenerHistorialAsync(CancellationToken ct = default)
    {
        AplicarToken();

        HttpResponseMessage respuesta;
        try
        {
            respuesta = await _http.GetAsync("api/reciclador/historial", ct);
        }
        catch (Exception)
        {
            throw new ApiException("No se pudo conectar con el servidor. Revisa tu conexion.");
        }

        if (!respuesta.IsSuccessStatusCode)
            throw new ApiException("No se pudo cargar tu historial.");

        return (await respuesta.Content.ReadFromJsonAsync<HistorialRecicladorResponse>(JsonOpciones, ct))!;
    }

    public async Task<ReciclajeResponse> ConfirmarRecoleccionAsync(
        string codigoHash, string correoCliente, decimal pesoKg, CancellationToken ct = default)
    {
        AplicarToken();

        var cuerpo = new { CodigoHash = codigoHash, CorreoCliente = correoCliente, PesoConfirmadoKg = pesoKg };

        HttpResponseMessage respuesta;
        try
        {
            respuesta = await _http.PostAsJsonAsync("api/reciclador/confirmar", cuerpo, JsonOpciones, ct);
        }
        catch (Exception)
        {
            throw new ApiException("No se pudo conectar con el servidor. Revisa tu conexion.");
        }

        if (!respuesta.IsSuccessStatusCode)
        {
            var cuerpoError = await respuesta.Content.ReadAsStringAsync(ct);
            throw new ApiException(ExtraerMensaje(cuerpoError) ?? "No se pudo confirmar la recoleccion.");
        }

        return (await respuesta.Content.ReadFromJsonAsync<ReciclajeResponse>(JsonOpciones, ct))!;
    }

    private static string? ExtraerMensaje(string cuerpoJson)
    {
        try
        {
            using var doc = JsonDocument.Parse(cuerpoJson);
            return doc.RootElement.TryGetProperty("mensaje", out var valor) ? valor.GetString() : null;
        }
        catch
        {
            return null;
        }
    }

    private void AplicarToken() =>
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _sesion.AccessToken);
}
