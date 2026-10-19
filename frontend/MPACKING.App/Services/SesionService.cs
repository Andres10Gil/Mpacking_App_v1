using MPACKING.App.Models;

namespace MPACKING.App.Services;

public class SesionService
{
    private const string ClaveToken = "mp_access_token";

    public UsuarioResponse? UsuarioActual { get; private set; }
    public string? AccessToken { get; private set; }

    public async Task GuardarSesionAsync(TokenResponse token)
    {
        AccessToken = token.AccessToken;
        UsuarioActual = token.Usuario;
        await SecureStorage.Default.SetAsync(ClaveToken, token.AccessToken);
    }

    public async Task<bool> HaySesionGuardadaAsync()
    {
        var token = await SecureStorage.Default.GetAsync(ClaveToken);
        return !string.IsNullOrEmpty(token);
    }

    public void CerrarSesion()
    {
        AccessToken = null;
        UsuarioActual = null;
        SecureStorage.Default.Remove(ClaveToken);
    }
}
