using MPACKING.Application.Modules.Reciclaje;

namespace MPACKING.Application.Modules.Reciclador;

public interface IRecicladorService
{
    Task<PerfilRecicladorResponse> ObtenerPerfilAsync(int idUsuario, CancellationToken ct = default);

    Task<HistorialRecicladorResponse> ObtenerHistorialAsync(int idUsuario, CancellationToken ct = default);

    /// <summary>
    /// Confirma la recoleccion a nombre del cliente identificado por su correo.
    /// Los ecopesos se acreditan a ESE cliente, nunca al reciclador.
    /// </summary>
    Task<ReciclajeResponse> ConfirmarRecoleccionAsync(int idUsuarioReciclador,
        ConfirmarRecoleccionRequest request, string? ip, CancellationToken ct = default);
}
