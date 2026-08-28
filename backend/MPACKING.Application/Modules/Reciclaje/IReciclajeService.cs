namespace MPACKING.Application.Modules.Reciclaje;

public interface IReciclajeService
{
    Task<QrInfoResponse> ConsultarQrAsync(string codigoHash, CancellationToken ct = default);

    Task<ReciclajeResponse> ConfirmarAsync(int idUsuario,
        ConfirmarReciclajeRequest request, string? ip, CancellationToken ct = default);

    Task<BilleteraResponse> ObtenerBilleteraAsync(int idUsuario,
        int pagina = 1, int tamano = 20, CancellationToken ct = default);
}
