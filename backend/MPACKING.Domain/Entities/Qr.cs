using MPACKING.Domain.Enums;
using MPACKING.Domain.Exceptions;

namespace MPACKING.Domain.Entities;

/// <summary>
/// Código QR impreso en cada empaque. Es de un solo uso: el trigger T3
/// lo marca como Usado al registrar la transacción y el trigger T4
/// bloquea cualquier reintento. Implementa los RF-05 y RF-06.
/// </summary>
public class Qr
{
    public int IdQr { get; private set; }
    public int IdRestaurante { get; private set; }
    public int IdMaterial { get; private set; }
    public string CodigoHash { get; private set; } = null!;
    public decimal PesoKg { get; private set; }
    public EstadoQr Estado { get; private set; } = EstadoQr.Activo;
    public DateTime FechaExpiracion { get; private set; }
    public DateTime FechaCreacion { get; private set; } = DateTime.UtcNow;

    public Restaurante Restaurante { get; private set; } = null!;
    public Material Material { get; private set; } = null!;

    private Qr() { }

    public Qr(int idRestaurante, int idMaterial, string codigoHash,
              decimal pesoKg, int diasVigencia = 30)
    {
        if (pesoKg <= 0)
            throw new DatosInvalidosException("El peso del empaque debe ser mayor a cero.");

        IdRestaurante = idRestaurante;
        IdMaterial = idMaterial;
        CodigoHash = codigoHash;
        PesoKg = pesoKg;
        FechaExpiracion = DateTime.UtcNow.AddDays(diasVigencia);
    }

    public bool EstaExpirado => FechaExpiracion < DateTime.UtcNow;

    public bool EsCanjeable => !EstaExpirado &&
        (Estado == EstadoQr.Activo || Estado == EstadoQr.Pendiente);

    /// <summary>
    /// Valida el QR antes de registrar la transacción. Replica la lógica del
    /// trigger T4 para dar un mensaje claro al usuario antes de llegar a la base.
    /// El trigger sigue siendo la garantía final.
    /// </summary>
    public void ValidarCanjeable()
    {
        if (EstaExpirado)
            throw new ReglaNegocioException(
                $"El código QR expiró el {FechaExpiracion:dd/MM/yyyy}.");

        if (Estado == EstadoQr.Usado)
            throw new ReglaNegocioException(
                "Este código QR ya fue utilizado en un reciclaje anterior.");

        if (Estado == EstadoQr.Expirado)
            throw new ReglaNegocioException("Este código QR está marcado como expirado.");
    }
}
