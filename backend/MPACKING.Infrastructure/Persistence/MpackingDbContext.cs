using Microsoft.EntityFrameworkCore;
using MPACKING.Domain.Entities;

namespace MPACKING.Infrastructure.Persistence;

/// <summary>
/// Contexto de Entity Framework Core. Mapea las 12 tablas creadas
/// por los scripts SQL del proyecto.
/// </summary>
public class MpackingDbContext : DbContext
{
    public MpackingDbContext(DbContextOptions<MpackingDbContext> options)
        : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Reciclador> Recicladores => Set<Reciclador>();
    public DbSet<Restaurante> Restaurantes => Set<Restaurante>();
    public DbSet<Material> Materiales => Set<Material>();
    public DbSet<HistorialPrecio> HistorialPrecios => Set<HistorialPrecio>();
    public DbSet<GuiaLimpieza> GuiasLimpieza => Set<GuiaLimpieza>();
    public DbSet<Qr> Qrs => Set<Qr>();
    public DbSet<Transaccion> Transacciones => Set<Transaccion>();
    public DbSet<Beneficio> Beneficios => Set<Beneficio>();
    public DbSet<Redencion> Redenciones => Set<Redencion>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<UbicacionReciclador> Ubicaciones => Set<UbicacionReciclador>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Carga todas las clases que implementan IEntityTypeConfiguration
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(MpackingDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
