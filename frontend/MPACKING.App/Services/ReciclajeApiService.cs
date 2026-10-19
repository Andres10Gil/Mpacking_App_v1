using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using MPACKING.App.Models;

namespace MPACKING.App.Services;

public class ReciclajeApiService
{
    private static readonly JsonSerializerOptions JsonOpciones = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly HttpClient _http;
    private readonly SesionService _sesion;

    public ReciclajeApiService(HttpClient http, SesionService sesion)
    {
        _http = http;
        _sesion = sesion;
    }

    public Task<BilleteraResponse> ObtenerBilleteraAsync(CancellationToken ct = default) =>
        EnviarAsync<object?, BilleteraResponse>(HttpMethod.Get, "api/reciclaje/billetera", null, ct);

    public Task<QrInfoResponse> ConsultarQrAsync(string codigoHash, CancellationToken ct = default) =>
        EnviarAsync<object?, QrInfoResponse>(
            HttpMethod.Get, $"api/reciclaje/qr/{Uri.EscapeDataString(codigoHash)}", null, ct);

    public Task<ReciclajeResponse> ConfirmarAsync(ConfirmarReciclajeRequest request, CancellationToken ct = default) =>
        EnviarAsync<ConfirmarReciclajeRequest, ReciclajeResponse>(
            HttpMethod.Post, "api/reciclaje/confirmar", request, ct);

    private async Task<TRespuesta> EnviarAsync<TCuerpo, TRespuesta>(
        HttpMethod metodo, string ruta, TCuerpo? cuerpo, CancellationToken ct)
    {
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _sesion.AccessToken);

        var mensaje = new HttpRequestMessage(metodo, ruta);
        if (cuerpo is not null)
            mensaje.Content = JsonContent.Create(cuerpo, options: JsonOpciones);

        HttpResponseMessage respuesta;
        try
        {
            respuesta = await _http.SendAsync(mensaje, ct);
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

        return (await respuesta.Content.ReadFromJsonAsync<TRespuesta>(JsonOpciones, ct))!;
    }
}
