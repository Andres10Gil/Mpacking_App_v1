using MPACKING.Application.Common.Interfaces;

namespace MPACKING.Infrastructure.Auth;

/// <summary>
/// Cifrado de contraseñas con bcrypt y factor de costo 12,
/// según lo definido en la sección de seguridad del informe técnico.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int FactorCosto = 12;

    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password, FactorCosto);

    public bool Verificar(string password, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            // Hash con formato inválido (por ejemplo los placeholder de los datos iniciales)
            return false;
        }
    }
}
