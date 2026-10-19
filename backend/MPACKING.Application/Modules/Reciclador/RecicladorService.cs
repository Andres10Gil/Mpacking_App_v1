using MPACKING.Application.Common.Interfaces;
using MPACKING.Application.Modules.Geolocalizacion;
using MPACKING.Application.Modules.Reciclaje;
using MPACKING.Domain.Exceptions;
using RecicladorEntity = MPACKING.Domain.Entities.Reciclador;

namespace MPACKING.Application.Modules.Reciclador;

/// <summary>
/// Panel del reciclador: su perfil, el kilaje que lleva recolectado y su
/// historial. La confirmación del reciclaje en si la hace IReciclajeService
/// (no se duplica aqui), este modulo solo resuelve el IdReciclador real y
/// expone los datos de resumen. RF-18.
/// </summary>
public class RecicladorService : IRecicladorService
{
    private const int CantidadHistorial = 40;

    private readonly IRecicladorRepository _recicladores;
    private readonly ITransaccionRepository _transacciones;
    private readonly IUsuarioRepository _usuarios;
    private readonly IGeolocalizacionService _geo;
    private readonly IReciclajeService _reciclaje;
    private readonly IRelojSistema _reloj;

    public RecicladorService(
        IRecicladorRepository recicladores,
        ITransaccionRepository transacciones,
        IUsuarioRepository usuarios,
        IGeolocalizacionService geo,
        IReciclajeService reciclaje,
        IRelojSistema reloj)
    {
        _recicladores = recicladores;
        _transacciones = transacciones;
        _usuarios = usuarios;
        _geo = geo;
        _reciclaje = reciclaje;
        _reloj = reloj;
    }

    public async Task<PerfilRecicladorResponse> ObtenerPerfilAsync(
        int idUsuario, CancellationToken ct = default)
    {
        var reciclador = await ObtenerRecicladorAsync(idUsuario, ct);

        var inicioDia = _reloj.Ahora.Date;
        var inicioMes = new DateTime(
            _reloj.Ahora.Year, _reloj.Ahora.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var kgHoy = await _transacciones.SumarKgPorRecicladorDesdeAsync(
            reciclador.IdReciclador, inicioDia, ct);
        var kgMes = await _transacciones.SumarKgPorRecicladorDesdeAsync(
            reciclador.IdReciclador, inicioMes, ct);

        var posicion = await _geo.ObtenerPosicionAsync(reciclador.IdReciclador, ct);

        return new PerfilRecicladorResponse(
            reciclador.IdReciclador, reciclador.Nombre, reciclador.ZonaAsignada,
            kgHoy, kgMes, posicion is not null,
            posicion?.Latitud, posicion?.Longitud);
    }

    public async Task<HistorialRecicladorResponse> ObtenerHistorialAsync(
        int idUsuario, CancellationToken ct = default)
    {
        var reciclador = await ObtenerRecicladorAsync(idUsuario, ct);
        var inicioMes = new DateTime(
            _reloj.Ahora.Year, _reloj.Ahora.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var kgMes = await _transacciones.SumarKgPorRecicladorDesdeAsync(
            reciclador.IdReciclador, inicioMes, ct);
        var movimientos = await _transacciones.ListarPorRecicladorAsync(
            reciclador.IdReciclador, CantidadHistorial, ct);

        var delMes = movimientos.Count(m => m.Fecha >= inicioMes);

        return new HistorialRecicladorResponse(
            kgMes, delMes,
            movimientos.Select(m => new MovimientoRecicladorResponse(
                m.Usuario.Nombre, m.Qr.Material.Nombre, m.PesoKg, m.EcopesosGanados, m.Fecha))
                .ToList());
    }

    public async Task<ReciclajeResponse> ConfirmarRecoleccionAsync(
        int idUsuarioReciclador, ConfirmarRecoleccionRequest request,
        string? ip, CancellationToken ct = default)
    {
        var reciclador = await ObtenerRecicladorAsync(idUsuarioReciclador, ct);

        var cliente = await _usuarios.ObtenerPorCorreoAsync(request.CorreoCliente, ct)
            ?? throw new NoEncontradoException("cliente", request.CorreoCliente);

        var solicitud = new ConfirmarReciclajeRequest(
            request.CodigoHash, reciclador.IdReciclador, request.PesoConfirmadoKg);

        return await _reciclaje.ConfirmarAsync(cliente.IdUsuario, solicitud, ip, ct);
    }

    private async Task<RecicladorEntity> ObtenerRecicladorAsync(int idUsuario, CancellationToken ct) =>
        await _recicladores.ObtenerPorIdUsuarioAsync(idUsuario, ct)
            ?? throw new NoEncontradoException("reciclador", idUsuario);
}
