using System.Net.Http.Json;
using System.Text.Json;
using MPACKING.App.Models;

namespace MPACKING.App.Services;

public class AuthApiService
{
    private static readonly JsonSerializerOptions JsonOpciones = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly HttpClient _http;

    public AuthApiService(HttpClient http)
    {
        _http = http;
    }

    public Task<TokenResponse> LoginAsync(string correo, string clave, CancellationToken ct = default) =>
        EnviarAsync(new LoginRequest(correo, clave), "api/auth/login", ct);

    public Task<TokenResponse> RegistrarAsync(
        string nombre, string correo, string clave, string rol, CancellationToken ct = default) =>
        EnviarAsync(new RegistroRequest(nombre, correo, clave, rol), "api/auth/registro", ct);

    private async Task<TokenResponse> EnviarAsync<T>(T cuerpo, string ruta, CancellationToken ct)
    {
        HttpResponseMessage respuesta;
        try
        {
            respuesta = await _http.PostAsJsonAsync(ruta, cuerpo, JsonOpciones, ct);
        }
        catch (Exception)
        {
            throw new ApiException("No se pudo conectar con el servidor. Revisa tu conexion.");
        }

        if (!respuesta.IsSuccessStatusCode)
        {
            var error = await respuesta.Content.ReadFromJsonAsync<ErrorApi>(JsonOpciones, ct);
            throw new ApiException(error?.Mensaje ?? "Ocurrio un error inesperado.");
        }

        return (await respuesta.Content.ReadFromJsonAsync<TokenResponse>(JsonOpciones, ct))!;
    }
}
