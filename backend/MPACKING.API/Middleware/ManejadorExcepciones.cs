using System.Text.Json;
using MPACKING.Domain.Exceptions;

namespace MPACKING.API.Middleware;

/// <summary>
/// Traduce las excepciones de dominio a respuestas HTTP con el código
/// adecuado. Evita repetir bloques try-catch en cada controller.
/// </summary>
public class ManejadorExcepciones
{
    private readonly RequestDelegate _siguiente;
    private readonly ILogger<ManejadorExcepciones> _logger;

    public ManejadorExcepciones(RequestDelegate siguiente,
                                ILogger<ManejadorExcepciones> logger)
    {
        _siguiente = siguiente;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await _siguiente(contexto);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning("Regla de negocio: {Mensaje}", ex.Message);
            await EscribirRespuesta(contexto, ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado");
            await EscribirRespuesta(contexto, 500,
                "Ocurrió un error inesperado. Intenta de nuevo más tarde.");
        }
    }

    private static async Task EscribirRespuesta(
        HttpContext contexto, int codigo, string mensaje)
    {
        contexto.Response.StatusCode = codigo;
        contexto.Response.ContentType = "application/json";

        var cuerpo = JsonSerializer.Serialize(new
        {
            exito = false,
            mensaje,
            codigo
        });

        await contexto.Response.WriteAsync(cuerpo);
    }
}
