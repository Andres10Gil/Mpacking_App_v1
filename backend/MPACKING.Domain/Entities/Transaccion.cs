using MPACKING.Domain.Exceptions;

namespace MPACKING.Domain.Entities;

/// <summary>
/// Registro inmutable de un reciclaje. Al insertarse dispara tres triggers:
/// T4 valida el QR, T1 acredita los ecopesos y T3 marca el QR como usado.
/// Por eso esta entidad no modifica saldos: sería duplicar el trabajo.
/// </summary>
public class Transaccion
{
    public int IdTransaccion { get; private set; }
    public int IdUsuario { get; private set; }
    public int IdQr { get; private set; }
    public int IdReciclador { get; private set; }
    public int EcopesosGanados { get; private set; }
    public decimal PesoKg { get; private set; }
    public decimal PrecioCopKg { get; private set; }
    public DateTime Fecha { get; private set; } = DateTime.UtcNow;
    public string? IpEscaneo { get; private set; }

    public Usuario Usuario { get; private set; } = null!;
    public Qr Qr { get; private set; } = null!;
    public Reciclador Reciclador { get; private set; } = null!;

    private Transaccion() { }

    public Transaccion(int idUsuario, int idQr, int idReciclador,
                       int ecopesosGanados, decimal pesoKg,
                       decimal precioCopKg, string? ipEscaneo = null)
    {
        if (ecopesosGanados < 0)
            throw new DatosInvalidosException("Los ecopesos no pueden ser negativos.");
        if (pesoKg <= 0)
            throw new DatosInvalidosException("El peso debe ser mayor a cero.");

        IdUsuario = idUsuario;
        IdQr = idQr;
        IdReciclador = idReciclador;
        EcopesosGanados = ecopesosGanados;
        PesoKg = pesoKg;
        PrecioCopKg = precioCopKg;
        IpEscaneo = ipEscaneo;
    }
}
