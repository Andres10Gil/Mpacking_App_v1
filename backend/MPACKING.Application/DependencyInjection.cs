using Microsoft.Extensions.DependencyInjection;
using MPACKING.Application.Modules.Admin;
using MPACKING.Application.Modules.Auth;
using MPACKING.Application.Modules.Catalogo;
using MPACKING.Application.Modules.Geolocalizacion;
using MPACKING.Application.Modules.Reciclaje;
using MPACKING.Application.Modules.Reciclador;
using MPACKING.Application.Modules.Redencion;
using MPACKING.Application.Modules.Restaurante;

namespace MPACKING.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registra los servicios de negocio. Cada módulo expone su interfaz,
    /// nunca su implementación concreta.
    /// </summary>
    public static IServiceCollection AgregarApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IReciclajeService, ReciclajeService>();
        services.AddScoped<IRedencionService, RedencionService>();
        services.AddScoped<ICatalogoService, CatalogoService>();
        services.AddScoped<IGeolocalizacionService, GeolocalizacionService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IRestauranteService, RestauranteService>();
        services.AddScoped<IRecicladorService, RecicladorService>();
        return services;
    }
}
