using MPACKING.Application.Common.Interfaces;
using MPACKING.Domain.Entities;
using MPACKING.Domain.Enums;
using MPACKING.Domain.Exceptions;

namespace MPACKING.Application.Modules.Auth;

/// <summary>
/// Registro y autenticación de usuarios. Implementa los RF-01, RF-02 y RF-03.
/// Responsabilidad única: no calcula ecopesos ni valida QR.
/// </summary>
public class AuthService : IAuthService
{
    private const int MinutosVigenciaToken = 15;

    private readonly IUsuarioRepository _usuarios;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokens;
    private readonly IUnitOfWork _uow;

    public AuthService(IUsuarioRepository usuarios, IPasswordHasher hasher,
                       ITokenService tokens, IUnitOfWork uow)
    {
        _usuarios = usuarios;
        _hasher = hasher;
        _tokens = tokens;
        _uow = uow;
    }

    public async Task<TokenResponse> RegistrarAsync(
        RegistroRequest request, CancellationToken ct = default)
    {
        if (await _usuarios.ExisteCorreoAsync(request.Correo, ct))
            throw new ReglaNegocioException("Ya existe una cuenta con ese correo.");

        if (!Enum.TryParse<RolUsuario>(request.Rol, ignoreCase: true, out var rol))
            throw new DatosInvalidosException($"El rol '{request.Rol}' no es válido.");

        if (request.Password.Length < 8)
            throw new DatosInvalidosException(
                "La contraseña debe tener al menos 8 caracteres.");

        var usuario = new Usuario(
            request.Nombre,
            request.Correo,
            _hasher.Hash(request.Password),
            rol);

        await _usuarios.AgregarAsync(usuario, ct);
        await _uow.GuardarCambiosAsync(ct);

        return ConstruirRespuesta(usuario);
    }

    public async Task<TokenResponse> LoginAsync(
        LoginRequest request, CancellationToken ct = default)
    {
        var usuario = await _usuarios.ObtenerPorCorreoAsync(request.Correo, ct);

        // Mensaje genérico para no revelar si el correo existe
        if (usuario is null)
            throw new AutenticacionException("Correo o contraseña incorrectos.");

        if (usuario.EstaBloqueado)
            throw new AutenticacionException(
                "La cuenta está bloqueada temporalmente. Intenta en 30 minutos.");

        if (!usuario.Activo)
            throw new AutenticacionException("La cuenta está desactivada.");

        if (!_hasher.Verificar(request.Password, usuario.PasswordHash))
        {
            usuario.RegistrarIntentoFallido();
            await _uow.GuardarCambiosAsync(ct);
            throw new AutenticacionException("Correo o contraseña incorrectos.");
        }

        usuario.RegistrarLoginExitoso();
        await _uow.GuardarCambiosAsync(ct);

        return ConstruirRespuesta(usuario);
    }

    private TokenResponse ConstruirRespuesta(Usuario usuario) => new(
        AccessToken: _tokens.GenerarAccessToken(usuario),
        RefreshToken: _tokens.GenerarRefreshToken(),
        ExpiraEnSegundos: MinutosVigenciaToken * 60,
        Usuario: new UsuarioResponse(
            usuario.IdUsuario, usuario.Nombre, usuario.Correo,
            usuario.Rol.ToString(), usuario.EcopesosTotal));
}
