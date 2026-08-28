namespace MPACKING.Application.Modules.Auth;

public interface IAuthService
{
    Task<TokenResponse> RegistrarAsync(RegistroRequest request, CancellationToken ct = default);
    Task<TokenResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
}
