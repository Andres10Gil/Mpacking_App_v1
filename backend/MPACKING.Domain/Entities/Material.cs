using MPACKING.Domain.Exceptions;

namespace MPACKING.Domain.Entities;

/// <summary>
/// Catálogo de materiales reciclables. El precio se maneja como rango
/// porque varía según la calidad del material entregado.
/// </summary>
public class Material
{
    public int IdMaterial { get; private set; }
    public string Nombre { get; private set; } = null!;
    public string Categoria { get; private set; } = null!;
    public decimal PrecioMinCopKg { get; private set; }
    public decimal PrecioMaxCopKg { get; private set; }
    public int EcopesosMinKg { get; private set; }
    public int EcopesosMaxKg { get; private set; }
    public decimal ReduccionCo2Kg { get; private set; }
    public string? IconoUrl { get; private set; }
    public bool Activo { get; private set; } = true;
    public DateOnly? UltimaActualizacion { get; private set; }

    private Material() { }

    /// <summary>
    /// Calcula los ecopesos correspondientes a un peso dado.
    /// Usa el valor mínimo del rango como base conservadora.
    /// Implementa el RF-07.
    /// </summary>
    public int CalcularEcopesos(decimal pesoKg)
    {
        if (pesoKg <= 0)
            throw new DatosInvalidosException("El peso debe ser mayor a cero.");

        return (int)Math.Round(pesoKg * EcopesosMinKg, MidpointRounding.AwayFromZero);
    }

    /// <summary>Calcula el CO2 evitado por reciclar este material.</summary>
    public decimal CalcularCo2Evitado(decimal pesoKg) => pesoKg * ReduccionCo2Kg;
}
