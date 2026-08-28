using Microsoft.AspNetCore.SignalR;

namespace MPACKING.API;

/// <summary>
/// Canal WebSocket que empuja la posición del reciclador a los usuarios.
/// Evita que la app tenga que consultar el servidor cada pocos segundos.
/// </summary>
public class GpsHub : Hub
{
    /// <summary>El usuario se suscribe a un reciclador concreto.</summary>
    public Task SeguirReciclador(int idReciclador) =>
        Groups.AddToGroupAsync(Context.ConnectionId, NombreGrupo(idReciclador));

    public Task DejarDeSeguir(int idReciclador) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, NombreGrupo(idReciclador));

    public static string NombreGrupo(int idReciclador) => $"reciclador-{idReciclador}";
}
