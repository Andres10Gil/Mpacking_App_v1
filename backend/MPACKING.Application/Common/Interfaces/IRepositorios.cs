using MPACKING.Domain.Entities;

namespace MPACKING.Application.Common.Interfaces;

/// <summary>
/// Contratos de acceso a datos. La capa Application depende de estas
/// abstracciones, nunca de Entity Framework. Es el principio de
/// inversión de dependencias (la D de SOLID).
/// </summary>
public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken ct = default);
    Task<bool> ExisteCorreoAsync(string correo, CancellationToken ct = default);
    Task<int> ObtenerSaldoAsync(int idUsuario, CancellationToken ct = default);
    Task AgregarAsync(Usuario usuario, CancellationToken ct = default);
    Task<IReadOnlyList<Usuario>> ListarAsync(int pagina, int tamano, CancellationToken ct = default);
}

public interface IQrRepository
{
    Task<Qr?> ObtenerPorCodigoAsync(string codigoHash, CancellationToken ct = default);
    Task<Qr?> ObtenerPorIdAsync(int idQr, CancellationToken ct = default);
    Task AgregarAsync(Qr qr, CancellationToken ct = default);
}

public interface ITransaccionRepository
{
    Task AgregarAsync(Transaccion transaccion, CancellationToken ct = default);
    Task<IReadOnlyList<Transaccion>> ListarPorUsuarioAsync(
        int idUsuario, int pagina, int tamano, CancellationToken ct = default);
    Task<decimal> TotalKgRecicladosAsync(int idUsuario, CancellationToken ct = default);
}

public interface IMaterialRepository
{
    Task<Material?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Material>> ListarActivosAsync(CancellationToken ct = default);
}

public interface IRestauranteRepository
{
    Task<Restaurante?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Restaurante>> ListarActivosAsync(CancellationToken ct = default);
}

public interface IBeneficioRepository
{
    Task<Beneficio?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Beneficio>> ListarPorRestauranteAsync(
        int idRestaurante, CancellationToken ct = default);
}

public interface IRedencionRepository
{
    Task<Redencion?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task<Redencion?> ObtenerPorCodigoAsync(string codigo, CancellationToken ct = default);
    Task AgregarAsync(Redencion redencion, CancellationToken ct = default);
    Task<IReadOnlyList<Redencion>> ListarPorUsuarioAsync(
        int idUsuario, CancellationToken ct = default);
}

public interface IPagoRepository
{
    Task AgregarAsync(Pago pago, CancellationToken ct = default);
    Task<Pago?> ObtenerPorRedencionAsync(int idRedencion, CancellationToken ct = default);
}

public interface IUbicacionRepository
{
    Task AgregarAsync(UbicacionReciclador ubicacion, CancellationToken ct = default);
    Task<UbicacionReciclador?> UltimaPosicionAsync(
        int idReciclador, CancellationToken ct = default);
}

public interface IGuiaLimpiezaRepository
{
    Task<GuiaLimpieza?> ObtenerPorMaterialAsync(int idMaterial, CancellationToken ct = default);
}

/// <summary>
/// Confirma los cambios pendientes en una sola transacción de base de datos.
/// Al hacer SaveChanges se disparan los triggers T1, T3 y T4 en PostgreSQL.
/// </summary>
public interface IUnitOfWork
{
    Task<int> GuardarCambiosAsync(CancellationToken ct = default);
}
