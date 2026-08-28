namespace MPACKING.Domain.Entities;

/// <summary>
/// Ficha educativa de limpieza por material. Implementa el RF-22.
/// Los pasos y errores se guardan como JSONB en PostgreSQL.
/// </summary>
public class GuiaLimpieza
{
    public int IdGuia { get; private set; }
    public int IdMaterial { get; private set; }
    public List<string> PasosLimpieza { get; private set; } = new();
    public List<string> ErroresComunes { get; private set; } = new();
    public string? DatoCurioso { get; private set; }
    public string? ImagenUrl { get; private set; }
    public string? VideoUrl { get; private set; }
    public bool Activo { get; private set; } = true;

    public Material Material { get; private set; } = null!;

    private GuiaLimpieza() { }

    public GuiaLimpieza(int idMaterial, List<string> pasos,
                        List<string> errores, string? datoCurioso = null)
    {
        IdMaterial = idMaterial;
        PasosLimpieza = pasos;
        ErroresComunes = errores;
        DatoCurioso = datoCurioso;
    }
}
