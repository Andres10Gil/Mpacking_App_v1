using MPACKING.Domain.Enums;
using MPACKING.Domain.Exceptions;

namespace MPACKING.Domain.Entities;

/// <summary>
/// Canje de ecopesos por un beneficio. El trigger T2 descuenta los ecopesos
/// al pasar a Aprobada y los reintegra al pasar a Devuelta.
/// </summary>
public class Redencion
{
    public int IdRedencion { get; private set; }
    public int IdUsuario { get; private set; }
    public int IdBeneficio { get; private set; }
    public string CodigoUnico { get; private set; } = null!;
    public int EcopesosUsados { get; private set; }
    public EstadoRedencion Estado { get; private set; } = EstadoRedencion.Pendiente;
    public DateTime FechaSolicitud { get; private set; } = DateTime.UtcNow;
    public DateTime? FechaValidacion { get; private set; }

    public Usuario Usuario { get; private set; } = null!;
    public Beneficio Beneficio { get; private set; } = null!;

    private Redencion() { }

    public Redencion(int idUsuario, int idBeneficio, string codigoUnico, int ecopesosUsados)
    {
        IdUsuario = idUsuario;
        IdBeneficio = idBeneficio;
        CodigoUnico = codigoUnico;
        EcopesosUsados = ecopesosUsados;
    }

    /// <summary>Aprueba la redención. El trigger T2 descuenta los ecopesos.</summary>
    public void Aprobar()
    {
        if (Estado != EstadoRedencion.Pendiente)
            throw new ReglaNegocioException(
                $"Solo se puede aprobar una redención pendiente. Estado actual: {Estado}.");

        Estado = EstadoRedencion.Aprobada;
        FechaValidacion = DateTime.UtcNow;
    }

    public void Rechazar()
    {
        if (Estado != EstadoRedencion.Pendiente)
            throw new ReglaNegocioException(
                $"Solo se puede rechazar una redención pendiente. Estado actual: {Estado}.");

        Estado = EstadoRedencion.Rechazada;
        FechaValidacion = DateTime.UtcNow;
    }

    /// <summary>Devuelve la redención. El trigger T2 reintegra los ecopesos.</summary>
    public void Devolver()
    {
        if (Estado != EstadoRedencion.Aprobada)
            throw new ReglaNegocioException(
                "Solo se puede devolver una redención previamente aprobada.");

        Estado = EstadoRedencion.Devuelta;
    }
}
