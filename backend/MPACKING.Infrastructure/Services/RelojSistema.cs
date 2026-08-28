using MPACKING.Application.Common.Interfaces;

namespace MPACKING.Infrastructure.Services;

/// <summary>
/// Abstrae la hora del sistema para poder simular el paso del tiempo
/// en las pruebas unitarias (por ejemplo, QR expirados).
/// </summary>
public class RelojSistema : IRelojSistema
{
    public DateTime Ahora => DateTime.UtcNow;
}
