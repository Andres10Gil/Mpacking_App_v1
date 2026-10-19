using Microsoft.EntityFrameworkCore;
using MPACKING.Application.Common.Interfaces;
using MPACKING.Application.Modules.Admin;
using MPACKING.Domain.Entities;
using MPACKING.Domain.Enums;

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

    public async Task<IReadOnlyList<Transaccion>> ListarPorRecicladorAsync(
        int idReciclador, int cantidad, CancellationToken ct = default) =>
        await _db.Transacciones
            .AsNoTracking()
            .Include(t => t.Usuario)
            .Include(t => t.Qr).ThenInclude(q => q.Material)
            .Where(t => t.IdReciclador == idReciclador)
            .OrderByDescending(t => t.Fecha)
            .Take(cantidad)
            .ToListAsync(ct);

    public async Task<decimal> SumarKgPorRecicladorDesdeAsync(
        int idReciclador, DateTime desde, CancellationToken ct = default) =>
        await _db.Transacciones
            .Where(t => t.IdReciclador == idReciclador && t.Fecha >= desde)
            .SumAsync(t => (decimal?)t.PesoKg, ct) ?? 0m;
}

public class RecicladorRepository : IRecicladorRepository
{
    private readonly MpackingDbContext _db;
    public RecicladorRepository(MpackingDbContext db) => _db = db;

    public Task<Reciclador?> ObtenerPorIdUsuarioAsync(int idUsuario, CancellationToken ct = default) =>
        _db.Recicladores.FirstOrDefaultAsync(r => r.IdUsuario == idUsuario, ct);
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

    public Task<Restaurante?> ObtenerPorIdUsuarioAsync(int idUsuario, CancellationToken ct = default) =>
        _db.Restaurantes.FirstOrDefaultAsync(r => r.IdUsuario == idUsuario, ct);

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

    public async Task<IReadOnlyList<Beneficio>> ListarTodosPorRestauranteAsync(
        int idRestaurante, CancellationToken ct = default) =>
        await _db.Beneficios
            .Where(b => b.IdRestaurante == idRestaurante)
            .OrderBy(b => b.EcopesosCosto)
            .ToListAsync(ct);

    public async Task AgregarAsync(Beneficio beneficio, CancellationToken ct = default) =>
        await _db.Beneficios.AddAsync(beneficio, ct);
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

    public async Task<IReadOnlyList<Redencion>> ListarPorRestauranteAsync(
        int idRestaurante, int cantidad, CancellationToken ct = default) =>
        await _db.Redenciones
            .AsNoTracking()
            .Include(r => r.Usuario)
            .Include(r => r.Beneficio)
            .Where(r => r.Beneficio.IdRestaurante == idRestaurante)
            .OrderByDescending(r => r.FechaSolicitud)
            .Take(cantidad)
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

    public async Task<IReadOnlyList<Pago>> ListarPorRestauranteAsync(
        int idRestaurante, int cantidad, CancellationToken ct = default) =>
        await _db.Pagos
            .AsNoTracking()
            .Include(p => p.Redencion).ThenInclude(r => r.Usuario)
            .Include(p => p.Redencion).ThenInclude(r => r.Beneficio)
            .Where(p => p.Redencion.Beneficio.IdRestaurante == idRestaurante)
            .OrderByDescending(p => p.FechaPago)
            .Take(cantidad)
            .ToListAsync(ct);

    public async Task<decimal> SumarMontoNetoDesdeAsync(
        int idRestaurante, DateTime desde, CancellationToken ct = default) =>
        await _db.Pagos
            .Where(p => p.Redencion.Beneficio.IdRestaurante == idRestaurante &&
                        p.Estado == EstadoPago.Aprobado && p.FechaPago >= desde)
            .SumAsync(p => (decimal?)p.MontoNeto, ct) ?? 0m;

    public Task<int> ContarAprobadosDesdeAsync(
        int idRestaurante, DateTime desde, CancellationToken ct = default) =>
        _db.Pagos.CountAsync(p =>
            p.Redencion.Beneficio.IdRestaurante == idRestaurante &&
            p.Estado == EstadoPago.Aprobado && p.FechaPago >= desde, ct);

    public async Task<int> SumarEcopesosValidadosDesdeAsync(
        int idRestaurante, DateTime desde, CancellationToken ct = default) =>
        await _db.Pagos
            .Where(p => p.Redencion.Beneficio.IdRestaurante == idRestaurante &&
                        p.Estado == EstadoPago.Aprobado && p.FechaPago >= desde)
            .SumAsync(p => (int?)p.Redencion.EcopesosUsados, ct) ?? 0;
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

public class AdminRepository : IAdminRepository
{
    private readonly MpackingDbContext _db;
    public AdminRepository(MpackingDbContext db) => _db = db;

    public Task<int> ContarUsuariosActivosAsync(CancellationToken ct = default) =>
        _db.Usuarios.CountAsync(u => u.Rol == RolUsuario.Usuario && u.Activo, ct);

    public Task<int> ContarTransaccionesAsync(CancellationToken ct = default) =>
        _db.Transacciones.CountAsync(ct);

    public Task<int> SumarEcopesosDadosAsync(CancellationToken ct = default) =>
        _db.Transacciones.SumAsync(t => t.EcopesosGanados, ct);

    public async Task<decimal> SumarKgRecicladosAsync(CancellationToken ct = default) =>
        await _db.Transacciones.SumAsync(t => (decimal?)t.PesoKg, ct) ?? 0m;

    public async Task<decimal> SumarKgRecicladosEnMesAsync(
        int anio, int mes, CancellationToken ct = default) =>
        await _db.Transacciones
            .Where(t => t.Fecha.Year == anio && t.Fecha.Month == mes)
            .SumAsync(t => (decimal?)t.PesoKg, ct) ?? 0m;

    public async Task<IReadOnlyList<Usuario>> ListarUsuariosAsync(
        string? busqueda, int pagina, int tamano, CancellationToken ct = default)
    {
        var query = _db.Usuarios.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var texto = busqueda.Trim().ToLower();
            query = query.Where(u =>
                u.Nombre.ToLower().Contains(texto) || u.Correo.Contains(texto));
        }

        return await query
            .OrderBy(u => u.IdUsuario)
            .Skip((pagina - 1) * tamano)
            .Take(tamano)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Redencion>> ListarRedencionesPendientesAsync(
        int cantidad, CancellationToken ct = default) =>
        await _db.Redenciones
            .AsNoTracking()
            .Include(r => r.Usuario)
            .Include(r => r.Beneficio)
            .Where(r => r.Estado == EstadoRedencion.Pendiente)
            .OrderByDescending(r => r.FechaSolicitud)
            .Take(cantidad)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<MaterialTotalDato>> TotalesPorMaterialAsync(
        CancellationToken ct = default) =>
        await _db.Transacciones
            .AsNoTracking()
            .GroupBy(t => t.Qr.Material.Nombre)
            .Select(g => new MaterialTotalDato(
                g.Key,
                g.Sum(t => t.PesoKg),
                g.Sum(t => t.EcopesosGanados),
                g.Sum(t => t.PesoKg * t.Qr.Material.ReduccionCo2Kg)))
            .ToListAsync(ct);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly MpackingDbContext _db;
    public UnitOfWork(MpackingDbContext db) => _db = db;

    public Task<int> GuardarCambiosAsync(CancellationToken ct = default) =>
        _db.SaveChangesAsync(ct);
}
