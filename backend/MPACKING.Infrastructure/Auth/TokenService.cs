using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MPACKING.Application.Common.Interfaces;
using MPACKING.Domain.Entities;

namespace MPACKING.Infrastructure.Auth;

/// <summary>
/// Genera tokens JWT con vigencia de 15 minutos y refresh tokens
/// aleatorios de 256 bits. Implementa el RF-02.
/// </summary>
public class TokenService : ITokenService
{
    private const int MinutosVigencia = 15;

    private readonly IConfiguration _config;
    public TokenService(IConfiguration config) => _config = config;

    public string GenerarAccessToken(Usuario usuario)
    {
        var clave = _config["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "Falta configurar Jwt:Key en appsettings.");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Correo),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var credenciales = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(MinutosVigencia),
            signingCredentials: credenciales);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerarRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes);
    }
}
