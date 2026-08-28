namespace MPACKING.Domain.Entities;

/// <summary>Perfil extendido del restaurante aliado. Relación 1:1 con Usuario.</summary>
public class Restaurante
{
    public int IdRestaurante { get; private set; }
    public int IdUsuario { get; private set; }
    public string Nombre { get; private set; } = null!;
    public string Direccion { get; private set; } = null!;
    public decimal? Latitud { get; private set; }
    public decimal? Longitud { get; private set; }
    public TimeOnly? HorarioApertura { get; private set; }
    public TimeOnly? HorarioCierre { get; private set; }
    public string? DiasAtencion { get; private set; }
    public bool Activo { get; private set; } = true;

    public Usuario Usuario { get; private set; } = null!;
    public ICollection<Beneficio> Beneficios { get; private set; } = new List<Beneficio>();

    private Restaurante() { }

    public Restaurante(int idUsuario, string nombre, string direccion,
                       decimal? latitud = null, decimal? longitud = null)
    {
        IdUsuario = idUsuario;
        Nombre = nombre;
        Direccion = direccion;
        Latitud = latitud;
        Longitud = longitud;
    }

    public bool TieneUbicacion => Latitud.HasValue && Longitud.HasValue;
}
