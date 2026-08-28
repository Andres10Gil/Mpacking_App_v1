using MPACKING.Domain.Enums;

namespace MPACKING.Domain.Entities;

/// <summary>
/// Pago automático al restaurante vía Wompi al confirmar una redención.
/// Implementa el RF-10 y RF-20.
/// </summary>
public class Pago
{
    /// <summary>Tasa de conversión: 1 ecopeso equivale a 10 pesos colombianos.</summary>
    public const decimal CopPorEcopeso = 10m;

    /// <summary>Comisión que retiene MPACKING sobre el valor redimido.</summary>
    public const decimal PorcentajeComisionMpacking = 0.05m;

    /// <summary>Comisión de la pasarela Wompi.</summary>
    public const decimal PorcentajeComisionWompi = 0.0299m;
    public const decimal CargoFijoWompi = 900m;

    public int IdPago { get; private set; }
    public int IdRedencion { get; private set; }
    public decimal MontoCop { get; private set; }
    public decimal ComisionMpacking { get; private set; }
    public decimal ComisionPasarela { get; private set; }
    public decimal MontoNeto { get; private set; }
    public string? WompiTxId { get; private set; }
    public EstadoPago Estado { get; private set; } = EstadoPago.Pendiente;
    public DateTime FechaPago { get; private set; } = DateTime.UtcNow;
    public DateOnly? FechaLiquidacion { get; private set; }

    public Redencion Redencion { get; private set; } = null!;

    private Pago() { }

    /// <summary>
    /// Crea el pago calculando comisiones a partir de los ecopesos redimidos.
    /// </summary>
    public static Pago DesdeEcopesos(int idRedencion, int ecopesos)
    {
        var monto = ecopesos * CopPorEcopeso;
        var comisionMpacking = Math.Round(monto * PorcentajeComisionMpacking, 2);
        var comisionWompi = Math.Round(monto * PorcentajeComisionWompi + CargoFijoWompi, 2);

        return new Pago
        {
            IdRedencion = idRedencion,
            MontoCop = monto,
            ComisionMpacking = comisionMpacking,
            ComisionPasarela = comisionWompi,
            MontoNeto = monto - comisionMpacking - comisionWompi
        };
    }

    public void MarcarAprobado(string wompiTxId)
    {
        WompiTxId = wompiTxId;
        Estado = EstadoPago.Aprobado;
        FechaLiquidacion = DateOnly.FromDateTime(DateTime.UtcNow);
    }

    public void MarcarRechazado(string? wompiTxId = null)
    {
        WompiTxId = wompiTxId;
        Estado = EstadoPago.Rechazado;
    }

    public void MarcarDevuelto() => Estado = EstadoPago.Devuelto;
}
