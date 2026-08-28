namespace MPACKING.Application.Modules.Catalogo;

public interface ICatalogoService
{
    Task<IReadOnlyList<MaterialResponse>> ListarMaterialesAsync(CancellationToken ct = default);
    Task<GuiaLimpiezaResponse> ObtenerGuiaAsync(int idMaterial, CancellationToken ct = default);
}
