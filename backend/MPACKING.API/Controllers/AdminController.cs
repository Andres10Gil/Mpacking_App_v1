using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MPACKING.Application.Modules.Admin;

namespace MPACKING.API.Controllers;

/// <summary>Panel de administración. Solo accesible con rol Admin. RF-16, RF-17.</summary>
[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _admin;
    public AdminController(IAdminService admin) => _admin = admin;

    /// <summary>Resumen del sistema: contadores y alertas de redenciones pendientes.</summary>
    [HttpGet("resumen")]
    public async Task<ActionResult<ResumenAdminResponse>> Resumen(CancellationToken ct)
        => Ok(await _admin.ObtenerResumenAsync(ct));

    /// <summary>Lista de usuarios con búsqueda por nombre o correo.</summary>
    [HttpGet("usuarios")]
    public async Task<ActionResult<IReadOnlyList<UsuarioAdminResponse>>> Usuarios(
        CancellationToken ct, string? busqueda = null, int pagina = 1, int tamano = 20)
        => Ok(await _admin.ListarUsuariosAsync(busqueda, pagina, tamano, ct));

    /// <summary>Reporte ambiental: CO2 evitado y totales por material.</summary>
    [HttpGet("reportes")]
    public async Task<ActionResult<ReporteAmbientalResponse>> Reportes(CancellationToken ct)
        => Ok(await _admin.ObtenerReporteAsync(ct));
}
