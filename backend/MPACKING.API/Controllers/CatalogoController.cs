using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MPACKING.Application.Modules.Catalogo;

namespace MPACKING.API.Controllers;

[ApiController]
[Route("api/catalogo")]
public class CatalogoController : ControllerBase
{
    private readonly ICatalogoService _catalogo;
    public CatalogoController(ICatalogoService catalogo) => _catalogo = catalogo;

    /// <summary>Tabla de valores por kg de material. RF-21.</summary>
    [HttpGet("materiales")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<MaterialResponse>>> Materiales(
        CancellationToken ct)
        => Ok(await _catalogo.ListarMaterialesAsync(ct));

    /// <summary>Guía de limpieza de un material. RF-22.</summary>
    [HttpGet("materiales/{idMaterial:int}/guia")]
    [AllowAnonymous]
    public async Task<ActionResult<GuiaLimpiezaResponse>> Guia(
        int idMaterial, CancellationToken ct)
        => Ok(await _catalogo.ObtenerGuiaAsync(idMaterial, ct));
}
