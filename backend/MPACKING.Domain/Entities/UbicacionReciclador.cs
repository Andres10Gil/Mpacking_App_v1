namespace MPACKING.Domain.Entities;

/// <summary>
/// Punto GPS del reciclador. Se guarda cada 30 segundos mientras está en ruta.
/// Los registros se eliminan a los 7 días por la Ley 1581 de 2012.
/// </summary>
public class UbicacionReciclador
{
    public long IdUbicacion { get; private set; }
    public int IdReciclador { get; private set; }
    public decimal Latitud { get; private set; }
    public decimal Longitud { get; private set; }
    public int? PrecisionM { get; private set; }
    public decimal? VelocidadKmh { get; private set; }
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

    public Reciclador Reciclador { get; private set; } = null!;

    private UbicacionReciclador() { }

    public UbicacionReciclador(int idReciclador, decimal latitud, decimal longitud,
                               int? precisionM = null, decimal? velocidadKmh = null)
    {
        IdReciclador = idReciclador;
        Latitud = latitud;
        Longitud = longitud;
        PrecisionM = precisionM;
        VelocidadKmh = velocidadKmh;
    }

    /// <summary>
    /// Si la precisión supera los 50 metros se muestra "zona aproximada"
    /// en lugar de un punto exacto, según lo definido en el módulo de geolocalización.
    /// </summary>
    public bool EsPrecisionConfiable => !PrecisionM.HasValue || PrecisionM.Value <= 50;
}
