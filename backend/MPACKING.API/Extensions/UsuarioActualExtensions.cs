using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MPACKING.Domain.Exceptions;

namespace MPACKING.API.Extensions;

/// <summary>Lee el identificador del usuario desde el token JWT.</summary>
public static class UsuarioActualExtensions
{
    public static int ObtenerIdUsuario(this ClaimsPrincipal usuario)
    {
        var claim = usuario.FindFirst(JwtRegisteredClaimNames.Sub)
                 ?? usuario.FindFirst(ClaimTypes.NameIdentifier);

        if (claim is null || !int.TryParse(claim.Value, out var id))
            throw new AutenticacionException("El token no contiene un usuario válido.");

        return id;
    }
}
