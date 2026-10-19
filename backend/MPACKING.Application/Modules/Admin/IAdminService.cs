namespace MPACKING.Application.Modules.Admin;

public interface IAdminService
{
    Task<ResumenAdminResponse> ObtenerResumenAsync(CancellationToken ct = default);

    Task<IReadOnlyList<UsuarioAdminResponse>> ListarUsuariosAsync(
        string? busqueda, int pagina, int tamano, CancellationToken ct = default);

    Task<ReporteAmbientalResponse> ObtenerReporteAsync(CancellationToken ct = default);
}
