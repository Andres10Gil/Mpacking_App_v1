using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MPACKING.API.Extensions;
using MPACKING.Application.Modules.Geolocalizacion;

namespace MPACKING.API.Controllers;

[ApiController]
[Route("api/geo")]
[Authorize]
public class GeolocalizacionController : ControllerBase
{
    private readonly IGeolocalizacionService _geo;
    public GeolocalizacionController(IGeolocalizacionService geo) => _geo = geo;

    /// <summary>
    /// El reciclador reporta su posición. La app la envía cada 30 segundos. RF-18.
    /// </summary>
    [HttpPost("posicion")]
    [Authorize(Roles = "Reciclador")]
    public async Task<IActionResult> ReportarPosicion(
        ReportarPosicionRequest request, CancellationToken ct)
    {
        await _geo.ReportarPosicionAsync(User.ObtenerIdUsuario(), request, ct);
        return NoContent();
    }

    /// <summary>Última posición conocida del reciclador. RF-18.</summary>
    [HttpGet("reciclador/{idReciclador:int}")]
    public async Task<ActionResult<PosicionResponse>> Posicion(
        int idReciclador, CancellationToken ct)
    {
        var posicion = await _geo.ObtenerPosicionAsync(idReciclador, ct);
        return posicion is null
            ? NotFound(new { mensaje = "El reciclador no está en ruta en este momento." })
            : Ok(posicion);
    }

    /// <summary>Restaurantes aliados dentro del radio indicado. RF-19.</summary>
    [HttpGet("restaurantes/cercanos")]
    public async Task<ActionResult<IReadOnlyList<RestauranteCercanoDto>>> Cercanos(
        decimal lat, decimal lng, CancellationToken ct, double radioKm = 5)
        => Ok(await _geo.BuscarRestaurantesCercanosAsync(lat, lng, radioKm, ct));
}
