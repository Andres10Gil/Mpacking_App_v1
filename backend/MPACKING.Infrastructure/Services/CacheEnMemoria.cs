using Microsoft.Extensions.Caching.Memory;
using MPACKING.Application.Common.Interfaces;

namespace MPACKING.Infrastructure.Services;

/// <summary>
/// Caché en memoria para desarrollo. En producción se reemplaza por Redis
/// implementando la misma interfaz ICacheService.
/// </summary>
public class CacheEnMemoria : ICacheService
{
    private readonly IMemoryCache _cache;
    public CacheEnMemoria(IMemoryCache cache) => _cache = cache;

    public Task<T?> ObtenerAsync<T>(string clave, CancellationToken ct = default) =>
        Task.FromResult(_cache.TryGetValue(clave, out T? valor) ? valor : default);

    public Task GuardarAsync<T>(string clave, T valor, TimeSpan expiracion,
                                CancellationToken ct = default)
    {
        _cache.Set(clave, valor, expiracion);
        return Task.CompletedTask;
    }

    public Task EliminarAsync(string clave, CancellationToken ct = default)
    {
        _cache.Remove(clave);
        return Task.CompletedTask;
    }
}
