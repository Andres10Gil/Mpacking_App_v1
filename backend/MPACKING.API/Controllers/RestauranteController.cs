using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MPACKING.API.Extensions;
using MPACKING.Application.Modules.Restaurante;

namespace MPACKING.API.Controllers;

/// <summary>Panel del restaurante aliado. Solo accesible con rol Restaurante.</summary>
[ApiController]
[Route("api/restaurante")]
[Authorize(Roles = "Restaurante")]
public class RestauranteController : ControllerBase
{
    private readonly IRestauranteService _restaurante;
    public RestauranteController(IRestauranteService restaurante) => _restaurante = restaurante;

    /// <summary>Recibido hoy, redenciones del dia y ultimos canjes.</summary>
    [HttpGet("resumen")]
    public async Task<ActionResult<ResumenRestauranteResponse>> Resumen(CancellationToken ct)
        => Ok(await _restaurante.ObtenerResumenAsync(User.ObtenerIdUsuario(), ct));

    /// <summary>Beneficios propios, incluidos los pausados.</summary>
    [HttpGet("beneficios")]
    public async Task<ActionResult<IReadOnlyList<BeneficioRestauranteResponse>>> Beneficios(CancellationToken ct)
        => Ok(await _restaurante.ListarBeneficiosAsync(User.ObtenerIdUsuario(), ct));

    /// <summary>Crea un nuevo beneficio para el restaurante.</summary>
    [HttpPost("beneficios")]
    public async Task<ActionResult<BeneficioRestauranteResponse>> CrearBeneficio(
        CrearBeneficioRequest request, CancellationToken ct)
        => Ok(await _restaurante.CrearBeneficioAsync(User.ObtenerIdUsuario(), request, ct));

    /// <summary>Pausa o reactiva un beneficio propio.</summary>
    [HttpPatch("beneficios/{idBeneficio:int}/estado")]
    public async Task<ActionResult<BeneficioRestauranteResponse>> CambiarEstado(
        int idBeneficio, [FromQuery] bool activar, CancellationToken ct)
        => Ok(await _restaurante.CambiarEstadoBeneficioAsync(User.ObtenerIdUsuario(), idBeneficio, activar, ct));

    /// <summary>Historial de pagos recibidos del mes.</summary>
    [HttpGet("historial")]
    public async Task<ActionResult<HistorialPagosResponse>> Historial(CancellationToken ct)
        => Ok(await _restaurante.ObtenerHistorialAsync(User.ObtenerIdUsuario(), ct));

    /// <summary>Valida el codigo que el usuario muestra al redimir un beneficio.</summary>
    [HttpPost("validar/{codigoUnico}")]
    public async Task<IActionResult> Validar(string codigoUnico, CancellationToken ct)
    {
        var resultado = await _restaurante.ValidarCodigoAsync(User.ObtenerIdUsuario(), codigoUnico, ct);
        return Ok(resultado);
    }
}
