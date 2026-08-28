using MPACKING.Application.Common.Interfaces;

namespace MPACKING.Infrastructure.Services;

/// <summary>
/// Implementación de desarrollo de la pasarela de pago.
/// Permite construir y probar todo el flujo de redención mientras
/// se aprueba la cuenta sandbox de Wompi.
///
/// Para pasar a producción se crea WompiPasarelaPago con la misma
/// interfaz y se cambia el registro en DependencyInjection.
/// </summary>
public class PasarelaPagoSimulada : IPasarelaPago
{
    public Task<ResultadoPago> TransferirAsync(
        decimal montoCop, string referencia, CancellationToken ct = default)
    {
        // Regla de prueba: los montos negativos o cero se rechazan.
        // Permite probar el camino de error del RF-12.
        if (montoCop <= 0)
            return Task.FromResult(new ResultadoPago(
                false, null, "El monto debe ser mayor a cero."));

        var transaccionId = $"SIM-{Guid.NewGuid().ToString("N")[..12].ToUpperInvariant()}";
        return Task.FromResult(new ResultadoPago(true, transaccionId, null));
    }
}
