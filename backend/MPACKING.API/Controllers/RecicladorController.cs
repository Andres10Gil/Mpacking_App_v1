using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MPACKING.API.Extensions;
using MPACKING.Application.Modules.Reciclador;

namespace MPACKING.API.Controllers;

/// <summary>Panel del reciclador. Solo accesible con rol Reciclador.</summary>
[ApiController]
[Route("api/reciclador")]
[Authorize(Roles = "Reciclador")]
public class RecicladorController : ControllerBase
{
    private readonly IRecicladorService _reciclador;
    public RecicladorController(IRecicladorService reciclador) => _reciclador = reciclador;

    /// <summary>Datos del reciclador logueado, kg de hoy y del mes, y si esta en ruta.</summary>
    [HttpGet("perfil")]
    public async Task<ActionResult<PerfilRecicladorResponse>> Perfil(CancellationToken ct)
        => Ok(await _reciclador.ObtenerPerfilAsync(User.ObtenerIdUsuario(), ct));

    /// <summary>Historial de recolecciones confirmadas por este reciclador.</summary>
    [HttpGet("historial")]
    public async Task<ActionResult<HistorialRecicladorResponse>> Historial(CancellationToken ct)
        => Ok(await _reciclador.ObtenerHistorialAsync(User.ObtenerIdUsuario(), ct));

    /// <summary>
    /// Confirma la recoleccion a nombre del cliente (identificado por correo).
    /// Los ecopesos quedan en la cuenta del cliente, no en la del reciclador.
    /// </summary>
    [HttpPost("confirmar")]
    public async Task<IActionResult> Confirmar(
        ConfirmarRecoleccionRequest request, CancellationToken ct)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var resultado = await _reciclador.ConfirmarRecoleccionAsync(
            User.ObtenerIdUsuario(), request, ip, ct);
        return Ok(resultado);
    }
}
