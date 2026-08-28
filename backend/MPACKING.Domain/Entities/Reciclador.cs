namespace MPACKING.Domain.Entities;

/// <summary>Perfil extendido del reciclador. Relación 1:1 con Usuario.</summary>
public class Reciclador
{
    public int IdReciclador { get; private set; }
    public int IdUsuario { get; private set; }
    public string Nombre { get; private set; } = null!;
    public string? Telefono { get; private set; }
    public string? ZonaAsignada { get; private set; }
    public bool Activo { get; private set; } = true;

    public Usuario Usuario { get; private set; } = null!;

    private Reciclador() { }

    public Reciclador(int idUsuario, string nombre, string? telefono, string? zona)
    {
        IdUsuario = idUsuario;
        Nombre = nombre;
        Telefono = telefono;
        ZonaAsignada = zona;
    }

    public void CambiarZona(string zona) => ZonaAsignada = zona;
}
