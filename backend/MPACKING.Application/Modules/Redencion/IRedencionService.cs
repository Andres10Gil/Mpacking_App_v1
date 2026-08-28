namespace MPACKING.Application.Modules.Redencion;

public interface IRedencionService
{
    Task<RedencionResponse> SolicitarAsync(int idUsuario,
        SolicitarRedencionRequest request, CancellationToken ct = default);

    Task<RedencionResponse> AprobarAsync(string codigoUnico, CancellationToken ct = default);

    Task<IReadOnlyList<RedencionResponse>> ListarPorUsuarioAsync(
        int idUsuario, CancellationToken ct = default);
}
