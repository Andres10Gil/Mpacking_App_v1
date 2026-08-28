using MPACKING.Application.Common.Interfaces;
using MPACKING.Domain.Exceptions;

namespace MPACKING.Application.Modules.Catalogo;

/// <summary>
/// Tabla de valores por kg y guías de limpieza. Implementa los RF-21 y RF-22.
/// El catálogo se cachea porque cambia rara vez y se consulta con frecuencia.
/// </summary>
public class CatalogoService : ICatalogoService
{
    private const string ClaveCacheMateriales = "catalogo:materiales";
    private static readonly TimeSpan VigenciaCache = TimeSpan.FromMinutes(30);

    private readonly IMaterialRepository _materiales;
    private readonly IGuiaLimpiezaRepository _guias;
    private readonly ICacheService _cache;

    public CatalogoService(IMaterialRepository materiales,
                           IGuiaLimpiezaRepository guias,
                           ICacheService cache)
    {
        _materiales = materiales;
        _guias = guias;
        _cache = cache;
    }

    public async Task<IReadOnlyList<MaterialResponse>> ListarMaterialesAsync(
        CancellationToken ct = default)
    {
        var enCache = await _cache.ObtenerAsync<List<MaterialResponse>>(
            ClaveCacheMateriales, ct);

        if (enCache is not null)
            return enCache;

        var materiales = await _materiales.ListarActivosAsync(ct);

        var respuesta = materiales.Select(m => new MaterialResponse(
            m.IdMaterial, m.Nombre, m.Categoria,
            m.PrecioMinCopKg, m.PrecioMaxCopKg,
            m.EcopesosMinKg, m.EcopesosMaxKg,
            m.ReduccionCo2Kg)).ToList();

        await _cache.GuardarAsync(ClaveCacheMateriales, respuesta, VigenciaCache, ct);
        return respuesta;
    }

    public async Task<GuiaLimpiezaResponse> ObtenerGuiaAsync(
        int idMaterial, CancellationToken ct = default)
    {
        var guia = await _guias.ObtenerPorMaterialAsync(idMaterial, ct)
            ?? throw new NoEncontradoException("guía de limpieza para el material", idMaterial);

        return new GuiaLimpiezaResponse(
            Material: guia.Material?.Nombre ?? "Material",
            Pasos: guia.PasosLimpieza,
            ErroresComunes: guia.ErroresComunes,
            DatoCurioso: guia.DatoCurioso,
            ImagenUrl: guia.ImagenUrl);
    }
}
