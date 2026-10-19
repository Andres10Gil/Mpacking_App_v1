namespace MPACKING.Application.Modules.Auth;

public record RegistroRequest(string Nombre, string Correo, string Clave, string Rol);

public record LoginRequest(string Correo, string Clave);

public record TokenResponse(string AccessToken, string RefreshToken,
                            int ExpiraEnSegundos, UsuarioResponse Usuario);

public record UsuarioResponse(int Id, string Nombre, string Correo,
                              string Rol, int EcopesosTotal);
