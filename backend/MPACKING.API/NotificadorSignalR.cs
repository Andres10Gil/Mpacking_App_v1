using Microsoft.AspNetCore.SignalR;
using MPACKING.Application.Common.Interfaces;

namespace MPACKING.API;

/// <summary>
/// Envía la posición del reciclador por SignalR a quienes lo siguen.
/// Vive en la capa API porque SignalR es un detalle de transporte.
/// </summary>
public class NotificadorSignalR : INotificadorUbicacion
{
    private readonly IHubContext<GpsHub> _hub;
    public NotificadorSignalR(IHubContext<GpsHub> hub) => _hub = hub;

    public Task EnviarPosicionAsync(int idReciclador, decimal latitud,
                                    decimal longitud, CancellationToken ct = default) =>
        _hub.Clients
            .Group(GpsHub.NombreGrupo(idReciclador))
            .SendAsync("PosicionActualizada", new
            {
                idReciclador,
                latitud,
                longitud,
                timestamp = DateTime.UtcNow
            }, ct);
}
