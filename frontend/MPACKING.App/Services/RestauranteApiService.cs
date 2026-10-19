using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using MPACKING.App.Models;

namespace MPACKING.App.Services;

public class RestauranteApiService
{
    private static readonly JsonSerializerOptions JsonOpciones = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly HttpClient _http;
    private readonly SesionService _sesion;

    public RestauranteApiService(HttpClient http, SesionService sesion)
    {
        _http = http;
        _sesion = sesion;
    }

    public async Task<ResumenRestauranteResponse> ObtenerResumenAsync(CancellationToken ct = default)
    {
        AplicarToken();
        var respuesta = await EnviarAsync(() => _http.GetAsync("api/restaurante/resumen", ct));
        return (await respuesta.Content.ReadFromJsonAsync<ResumenRestauranteResponse>(JsonOpciones, ct))!;
    }

    public async Task<List<BeneficioRestauranteResponse>> ListarBeneficiosAsync(CancellationToken ct = default)
    {
        AplicarToken();
        var respuesta = await EnviarAsync(() => _http.GetAsync("api/restaurante/beneficios", ct));
        return (await respuesta.Content.ReadFromJsonAsync<List<BeneficioRestauranteResponse>>(JsonOpciones, ct))!;
    }

    public async Task CrearBeneficioAsync(string descripcion, int ecopesosCosto, CancellationToken ct = default)
    {
        AplicarToken();
        var cuerpo = new CrearBeneficioRequest(descripcion, ecopesosCosto);
        await EnviarAsync(() => _http.PostAsJsonAsync("api/restaurante/beneficios", cuerpo, JsonOpciones, ct));
    }

    public async Task CambiarEstadoBeneficioAsync(int idBeneficio, bool activar, CancellationToken ct = default)
    {
        AplicarToken();
        await EnviarAsync(() => _http.PatchAsync(
            $"api/restaurante/beneficios/{idBeneficio}/estado?activar={activar}", null, ct));
    }

    public async Task<HistorialPagosResponse> ObtenerHistorialAsync(CancellationToken ct = default)
    {
        AplicarToken();
        var respuesta = await EnviarAsync(() => _http.GetAsync("api/restaurante/historial", ct));
        return (await respuesta.Content.ReadFromJsonAsync<HistorialPagosResponse>(JsonOpciones, ct))!;
    }

    public async Task<string> ValidarCodigoAsync(string codigoUnico, CancellationToken ct = default)
    {
        AplicarToken();
        var respuesta = await EnviarAsync(() =>
            _http.PostAsync($"api/restaurante/validar/{Uri.EscapeDataString(codigoUnico)}", null, ct));
        return await respuesta.Content.ReadAsStringAsync(ct);
    }

    private async Task<HttpResponseMessage> EnviarAsync(Func<Task<HttpResponseMessage>> llamada)
    {
        HttpResponseMessage respuesta;
        try
        {
            respuesta = await llamada();
        }
        catch (Exception)
        {
            throw new ApiException("No se pudo conectar con el servidor. Revisa tu conexion.");
        }

        if (!respuesta.IsSuccessStatusCode)
        {
            var cuerpo = await respuesta.Content.ReadAsStringAsync();
            var mensaje = ExtraerMensaje(cuerpo) ?? "Ocurrio un error. Intenta de nuevo.";
            throw new ApiException(mensaje);
        }

        return respuesta;
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
