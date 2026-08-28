using MPACKING.Domain.Entities;

namespace MPACKING.Application.Common.Interfaces;

/// <summary>Cifrado y verificación de contraseñas.</summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verificar(string password, string hash);
}

/// <summary>Generación de tokens JWT y refresh tokens.</summary>
public interface ITokenService
{
    string GenerarAccessToken(Usuario usuario);
    string GenerarRefreshToken();
}

/// <summary>
/// Pasarela de pago. Está detrás de una interfaz para poder desarrollar
/// el flujo completo con una implementación falsa mientras se aprueba
/// la cuenta sandbox de Wompi.
/// </summary>
public interface IPasarelaPago
{
    Task<ResultadoPago> TransferirAsync(
        decimal montoCop, string referencia, CancellationToken ct = default);
}

public record ResultadoPago(bool Exitoso, string? TransaccionId, string? MensajeError);

/// <summary>Envío de la posición del reciclador a los usuarios suscritos.</summary>
public interface INotificadorUbicacion
{
    Task EnviarPosicionAsync(int idReciclador, decimal latitud, decimal longitud,
                             CancellationToken ct = default);
}

/// <summary>Caché para la última posición GPS y catálogos consultados con frecuencia.</summary>
public interface ICacheService
{
    Task<T?> ObtenerAsync<T>(string clave, CancellationToken ct = default);
    Task GuardarAsync<T>(string clave, T valor, TimeSpan expiracion,
                         CancellationToken ct = default);
    Task EliminarAsync(string clave, CancellationToken ct = default);
}

/// <summary>Abstrae DateTime.UtcNow para poder probar la lógica temporal.</summary>
public interface IRelojSistema
{
    DateTime Ahora { get; }
}
