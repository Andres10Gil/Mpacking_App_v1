using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MPACKING.Application.Modules.Auth;

namespace MPACKING.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    /// <summary>Crea una cuenta nueva. RF-01.</summary>
    [HttpPost("registro")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> Registrar(
        RegistroRequest request, CancellationToken ct)
        => Ok(await _auth.RegistrarAsync(request, ct));

    /// <summary>Inicia sesión y devuelve el token JWT. RF-02.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> Login(
        LoginRequest request, CancellationToken ct)
        => Ok(await _auth.LoginAsync(request, ct));
}
