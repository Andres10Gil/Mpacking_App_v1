using MPACKING.Domain.Exceptions;

namespace MPACKING.Domain.Entities;

/// <summary>Oferta que el restaurante pone a disposición a cambio de ecopesos.</summary>
public class Beneficio
{
    public int IdBeneficio { get; private set; }
    public int IdRestaurante { get; private set; }
    public string Descripcion { get; private set; } = null!;
    public int EcopesosCosto { get; private set; }
    public bool Activo { get; private set; } = true;
    public DateOnly? FechaExpiracion { get; private set; }

    public Restaurante Restaurante { get; private set; } = null!;

    private Beneficio() { }

    public Beneficio(int idRestaurante, string descripcion, int ecopesosCosto)
    {
        if (ecopesosCosto <= 0)
            throw new DatosInvalidosException("El costo en ecopesos debe ser mayor a cero.");

        IdRestaurante = idRestaurante;
        Descripcion = descripcion;
        EcopesosCosto = ecopesosCosto;
    }

    public bool EstaDisponible =>
        Activo && (!FechaExpiracion.HasValue ||
                   FechaExpiracion.Value >= DateOnly.FromDateTime(DateTime.UtcNow));

    public void Pausar() => Activo = false;
    public void Reactivar() => Activo = true;
}
