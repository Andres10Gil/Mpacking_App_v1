namespace MPACKING.Domain.Entities;

/// <summary>
/// Auditoría inmutable de cambios de precio. Cada actualización del
/// administrador genera un registro. Implementa el RF-17.
/// </summary>
public class HistorialPrecio
{
    public int IdHistorial { get; private set; }
    public int IdMaterial { get; private set; }
    public int CambiadoPor { get; private set; }
    public decimal PrecioCopKg { get; private set; }
    public int EcopesosKg { get; private set; }
    public DateOnly FechaInicio { get; private set; }
    public DateOnly? FechaFin { get; private set; }

    public Material Material { get; private set; } = null!;
    public Usuario Responsable { get; private set; } = null!;

    private HistorialPrecio() { }

    public HistorialPrecio(int idMaterial, int cambiadoPor,
                           decimal precioCopKg, int ecopesosKg)
    {
        IdMaterial = idMaterial;
        CambiadoPor = cambiadoPor;
        PrecioCopKg = precioCopKg;
        EcopesosKg = ecopesosKg;
        FechaInicio = DateOnly.FromDateTime(DateTime.UtcNow);
    }

    public void Cerrar() => FechaFin = DateOnly.FromDateTime(DateTime.UtcNow);
}
