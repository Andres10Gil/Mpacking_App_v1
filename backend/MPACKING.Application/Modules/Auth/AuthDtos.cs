namespace MPACKING.Application.Modules.Auth;

public record RegistroRequest(string Nombre, string Correo, string Password, string Rol);

public record LoginRequest(string Correo, string Password);

public record TokenResponse(string AccessToken, string RefreshToken,
                            int ExpiraEnSegundos, UsuarioResponse Usuario);

public record UsuarioResponse(int Id, string Nombre, string Correo,
                              string Rol, int EcopesosTotal);
