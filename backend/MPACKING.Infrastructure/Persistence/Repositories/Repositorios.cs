using Microsoft.EntityFrameworkCore;
using MPACKING.Application.Common.Interfaces;
using MPACKING.Domain.Entities;

namespace MPACKING.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementaciones de los contratos de repositorio con Entity Framework Core.
/// Esta es la única capa que conoce EF: si mañana se cambia el ORM,
/// solo se toca este archivo.
/// </summary>
public class UsuarioRepository : IUsuarioRepository
{
    private readonly MpackingDbContext _db;
    public UsuarioRepository(MpackingDbContext db) => _db = db;

    public Task<Usuario?> ObtenerPorIdAsync(int id, CancellationToken ct = default) =>
        _db.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id, ct);

    public Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken ct = default) =>
        _db.Usuarios.FirstOrDefaultAsync(
            u => u.Correo == correo.ToLower(), ct);

    public Task<bool> ExisteCorreoAsync(string correo, CancellationToken ct = default) =>
        _db.Usuarios.AnyAsync(u => u.Correo == correo.ToLower(), ct);

    /// <summary>
    /// Lee el saldo directamente de la base sin caché de EF, porque el
    /// trigger T1 lo modificó fuera del contexto de seguimiento.
    /// </summary>
    public async Task<int> ObtenerSaldoAsync(int idUsuario, CancellationToken ct = default) =>
        await _db.Usuarios
            .AsNoTracking()
            .Where(u => u.IdUsuario == idUsuario)
            .Select(u => u.EcopesosTotal)
            .FirstOrDefaultAsync(ct);

    public async Task AgregarAsync(Usuario usuario, CancellationToken ct = default) =>
        await _db.Usuarios.AddAsync(usuario, ct);

    public async Task<IReadOnlyList<Usuario>> ListarAsync(
        int pagina, int tamano, CancellationToken ct = default) =>
        await _db.Usuarios
            .AsNoTracking()
            .OrderBy(u => u.IdUsuario)
            .Skip((pagina - 1) * tamano)
            .Take(tamano)
            .ToListAsync(ct);
}

public class QrRepository : IQrRepository
{
    private readonly MpackingDbContext _db;
    public QrRepository(MpackingDbContext db) => _db = db;

    public Task<Qr?> ObtenerPorCodigoAsync(string codigoHash, CancellationToken ct = default) =>
        _db.Qrs
            .Include(q => q.Material)
            .Include(q => q.Restaurante)
            .FirstOrDefaultAsync(q => q.CodigoHash == codigoHash, ct);

    public Task<Qr?> ObtenerPorIdAsync(int idQr, CancellationToken ct = default) =>
        _db.Qrs
            .Include(q => q.Material)
            .FirstOrDefaultAsync(q => q.IdQr == idQr, ct);

    public async Task AgregarAsync(Qr qr, CancellationToken ct = default) =>
        await _db.Qrs.AddAsync(qr, ct);
}

public class TransaccionRepository : ITransaccionRepository
{
    private readonly MpackingDbContext _db;
    public TransaccionRepository(MpackingDbContext db) => _db = db;

    public async Task AgregarAsync(Transaccion transaccion, CancellationToken ct = default) =>
        await _db.Transacciones.AddAsync(transaccion, ct);

    public async Task<IReadOnlyList<Transaccion>> ListarPorUsuarioAsync(
        int idUsuario, int pagina, int tamano, CancellationToken ct = default) =>
        await _db.Transacciones
            .AsNoTracking()
            .Include(t => t.Qr).ThenInclude(q => q.Material)
            .Where(t => t.IdUsuario == idUsuario)
            .OrderByDescending(t => t.Fecha)
            .Skip((pagina - 1) * tamano)
            .Take(tamano)
            .ToListAsync(ct);

    public async Task<decimal> TotalKgRecicladosAsync(
        int idUsuario, CancellationToken ct = default) =>
        await _db.Transacciones
            .Where(t => t.IdUsuario == idUsuario)
            .SumAsync(t => (decimal?)t.PesoKg, ct) ?? 0m;
}

public class MaterialRepository : IMaterialRepository
{
    private readonly MpackingDbContext _db;
    public MaterialRepository(MpackingDbContext db) => _db = db;

    public Task<Material?> ObtenerPorIdAsync(int id, CancellationToken ct = default) =>
        _db.Materiales.FirstOrDefaultAsync(m => m.IdMaterial == id, ct);

    public async Task<IReadOnlyList<Material>> ListarActivosAsync(
        CancellationToken ct = default) =>
        await _db.Materiales
            .AsNoTracking()
            .Where(m => m.Activo)
            .OrderBy(m => m.Categoria).ThenBy(m => m.Nombre)
            .ToListAsync(ct);
}

public class RestauranteRepository : IRestauranteRepository
{
    private readonly MpackingDbContext _db;
    public RestauranteRepository(MpackingDbContext db) => _db = db;

    public Task<Restaurante?> ObtenerPorIdAsync(int id, CancellationToken ct = default) =>
        _db.Restaurantes
            .Include(r => r.Beneficios)
            .FirstOrDefaultAsync(r => r.IdRestaurante == id, ct);

    public async Task<IReadOnlyList<Restaurante>> ListarActivosAsync(
        CancellationToken ct = default) =>
        await _db.Restaurantes
            .AsNoTracking()
            .Include(r => r.Beneficios)
            .Where(r => r.Activo)
            .ToListAsync(ct);
}

public class BeneficioRepository : IBeneficioRepository
{
    private readonly MpackingDbContext _db;
    public BeneficioRepository(MpackingDbContext db) => _db = db;

    public Task<Beneficio?> ObtenerPorIdAsync(int id, CancellationToken ct = default) =>
        _db.Beneficios
            .Include(b => b.Restaurante)
            .FirstOrDefaultAsync(b => b.IdBeneficio == id, ct);

    public async Task<IReadOnlyList<Beneficio>> ListarPorRestauranteAsync(
        int idRestaurante, CancellationToken ct = default) =>
        await _db.Beneficios
            .AsNoTracking()
            .Where(b => b.IdRestaurante == idRestaurante && b.Activo)
            .OrderBy(b => b.EcopesosCosto)
            .ToListAsync(ct);
}

public class RedencionRepository : IRedencionRepository
{
    private readonly MpackingDbContext _db;
    public RedencionRepository(MpackingDbContext db) => _db = db;

    public Task<Redencion?> ObtenerPorIdAsync(int id, CancellationToken ct = default) =>
        _db.Redenciones
            .Include(r => r.Beneficio).ThenInclude(b => b.Restaurante)
            .FirstOrDefaultAsync(r => r.IdRedencion == id, ct);

    public Task<Redencion?> ObtenerPorCodigoAsync(string codigo, CancellationToken ct = default) =>
        _db.Redenciones
            .Include(r => r.Beneficio).ThenInclude(b => b.Restaurante)
            .FirstOrDefaultAsync(r => r.CodigoUnico == codigo, ct);

    public async Task AgregarAsync(Redencion redencion, CancellationToken ct = default) =>
        await _db.Redenciones.AddAsync(redencion, ct);

    public async Task<IReadOnlyList<Redencion>> ListarPorUsuarioAsync(
        int idUsuario, CancellationToken ct = default) =>
        await _db.Redenciones
            .AsNoTracking()
            .Include(r => r.Beneficio).ThenInclude(b => b.Restaurante)
            .Where(r => r.IdUsuario == idUsuario)
            .OrderByDescending(r => r.FechaSolicitud)
            .ToListAsync(ct);
}

public class PagoRepository : IPagoRepository
{
    private readonly MpackingDbContext _db;
    public PagoRepository(MpackingDbContext db) => _db = db;

    public async Task AgregarAsync(Pago pago, CancellationToken ct = default) =>
        await _db.Pagos.AddAsync(pago, ct);

    public Task<Pago?> ObtenerPorRedencionAsync(int idRedencion, CancellationToken ct = default) =>
        _db.Pagos.FirstOrDefaultAsync(p => p.IdRedencion == idRedencion, ct);
}

public class UbicacionRepository : IUbicacionRepository
{
    private readonly MpackingDbContext _db;
    public UbicacionRepository(MpackingDbContext db) => _db = db;

    public async Task AgregarAsync(UbicacionReciclador ubicacion, CancellationToken ct = default) =>
        await _db.Ubicaciones.AddAsync(ubicacion, ct);

    public Task<UbicacionReciclador?> UltimaPosicionAsync(
        int idReciclador, CancellationToken ct = default) =>
        _db.Ubicaciones
            .AsNoTracking()
            .Where(u => u.IdReciclador == idReciclador)
            .OrderByDescending(u => u.Timestamp)
            .FirstOrDefaultAsync(ct);
}

public class GuiaLimpiezaRepository : IGuiaLimpiezaRepository
{
    private readonly MpackingDbContext _db;
    public GuiaLimpiezaRepository(MpackingDbContext db) => _db = db;

    public Task<GuiaLimpieza?> ObtenerPorMaterialAsync(
        int idMaterial, CancellationToken ct = default) =>
        _db.GuiasLimpieza
            .Include(g => g.Material)
            .FirstOrDefaultAsync(g => g.IdMaterial == idMaterial && g.Activo, ct);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly MpackingDbContext _db;
    public UnitOfWork(MpackingDbContext db) => _db = db;

    public Task<int> GuardarCambiosAsync(CancellationToken ct = default) =>
        _db.SaveChangesAsync(ct);
}
