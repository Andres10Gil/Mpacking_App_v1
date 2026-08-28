using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MPACKING.API.Extensions;
using MPACKING.Application.Modules.Reciclaje;

namespace MPACKING.API.Controllers;

[ApiController]
[Route("api/reciclaje")]
[Authorize]
public class ReciclajeController : ControllerBase
{
    private readonly IReciclajeService _reciclaje;
    public ReciclajeController(IReciclajeService reciclaje) => _reciclaje = reciclaje;

    /// <summary>
    /// Consulta un QR escaneado sin registrarlo. Permite mostrar el material
    /// y los ecopesos estimados antes de confirmar. RF-05.
    /// </summary>
    [HttpGet("qr/{codigoHash}")]
    public async Task<ActionResult<QrInfoResponse>> ConsultarQr(
        string codigoHash, CancellationToken ct)
        => Ok(await _reciclaje.ConsultarQrAsync(codigoHash, ct));

    /// <summary>
    /// Registra el reciclaje. Dispara los triggers T4, T1 y T3. RF-06 a RF-08.
    /// </summary>
    [HttpPost("confirmar")]
    public async Task<ActionResult<ReciclajeResponse>> Confirmar(
        ConfirmarReciclajeRequest request, CancellationToken ct)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var resultado = await _reciclaje.ConfirmarAsync(
            User.ObtenerIdUsuario(), request, ip, ct);
        return Ok(resultado);
    }

    /// <summary>Saldo, historial e impacto ambiental del usuario. RF-09.</summary>
    [HttpGet("billetera")]
    public async Task<ActionResult<BilleteraResponse>> Billetera(
        CancellationToken ct, int pagina = 1, int tamano = 20)
        => Ok(await _reciclaje.ObtenerBilleteraAsync(
            User.ObtenerIdUsuario(), pagina, tamano, ct));
}
