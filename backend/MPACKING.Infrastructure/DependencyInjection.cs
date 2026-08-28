using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MPACKING.Application.Common.Interfaces;
using MPACKING.Infrastructure.Auth;
using MPACKING.Infrastructure.Persistence;
using MPACKING.Infrastructure.Persistence.Repositories;
using MPACKING.Infrastructure.Services;

namespace MPACKING.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registra el acceso a datos y los servicios externos.
    /// Cambiar una implementación (por ejemplo caché en memoria por Redis)
    /// solo requiere modificar la línea correspondiente aquí.
    /// </summary>
    public static IServiceCollection AgregarInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<MpackingDbContext>(opciones =>
            opciones.UseNpgsql(config.GetConnectionString("MpackingDb")));

        // Repositorios
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IQrRepository, QrRepository>();
        services.AddScoped<ITransaccionRepository, TransaccionRepository>();
        services.AddScoped<IMaterialRepository, MaterialRepository>();
        services.AddScoped<IRestauranteRepository, RestauranteRepository>();
        services.AddScoped<IBeneficioRepository, BeneficioRepository>();
        services.AddScoped<IRedencionRepository, RedencionRepository>();
        services.AddScoped<IPagoRepository, PagoRepository>();
        services.AddScoped<IUbicacionRepository, UbicacionRepository>();
        services.AddScoped<IGuiaLimpiezaRepository, GuiaLimpiezaRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Seguridad
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();

        // Servicios externos
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, CacheEnMemoria>();
        services.AddSingleton<IRelojSistema, RelojSistema>();

        // Reemplazar por WompiPasarelaPago cuando esté la cuenta sandbox
        services.AddScoped<IPasarelaPago, PasarelaPagoSimulada>();

        return services;
    }
}
