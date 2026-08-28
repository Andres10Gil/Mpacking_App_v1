using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MPACKING.API.Extensions;
using MPACKING.Application.Modules.Redencion;

namespace MPACKING.API.Controllers;

[ApiController]
[Route("api/redenciones")]
[Authorize]
public class RedencionController : ControllerBase
{
    private readonly IRedencionService _redenciones;
    public RedencionController(IRedencionService redenciones) => _redenciones = redenciones;

    /// <summary>El usuario solicita canjear un beneficio. RF-11.</summary>
    [HttpPost]
    public async Task<ActionResult<RedencionResponse>> Solicitar(
        SolicitarRedencionRequest request, CancellationToken ct)
        => Ok(await _redenciones.SolicitarAsync(User.ObtenerIdUsuario(), request, ct));

    /// <summary>
    /// El restaurante valida el código. Se cobra vía Wompi y el trigger T2
    /// descuenta los ecopesos solo si el pago fue exitoso. RF-10, RF-14, RF-20.
    /// </summary>
    [HttpPost("{codigoUnico}/aprobar")]
    [Authorize(Roles = "Restaurante,Admin")]
    public async Task<ActionResult<RedencionResponse>> Aprobar(
        string codigoUnico, CancellationToken ct)
        => Ok(await _redenciones.AprobarAsync(codigoUnico, ct));

    /// <summary>Historial de canjes del usuario.</summary>
    [HttpGet("mias")]
    public async Task<ActionResult<IReadOnlyList<RedencionResponse>>> Mias(
        CancellationToken ct)
        => Ok(await _redenciones.ListarPorUsuarioAsync(User.ObtenerIdUsuario(), ct));
}
