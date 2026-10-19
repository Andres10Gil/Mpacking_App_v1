using Microsoft.Extensions.Logging;
using MPACKING.App.Services;
using MPACKING.App.ViewModels;
using MPACKING.App.Views;

namespace MPACKING.App;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// Servicios que hablan con el backend
		builder.Services.AddSingleton<SesionService>();
		builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(ApiConfig.BaseUrl) });
		builder.Services.AddTransient<AuthApiService>();
		builder.Services.AddTransient<ReciclajeApiService>();
		builder.Services.AddTransient<GeoApiService>();
		builder.Services.AddTransient<CatalogoApiService>();
		builder.Services.AddTransient<RedencionApiService>();
		builder.Services.AddTransient<AdminApiService>();
		builder.Services.AddTransient<RestauranteApiService>();
		builder.Services.AddTransient<RecicladorApiService>();

		// Pantallas y sus ViewModel
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<RegistroViewModel>();
		builder.Services.AddTransient<RegistroPage>();
		builder.Services.AddTransient<InicioViewModel>();
		builder.Services.AddTransient<InicioPage>();
		builder.Services.AddTransient<EscanearViewModel>();
		builder.Services.AddTransient<EscanearPage>();
		builder.Services.AddTransient<MapaViewModel>();
		builder.Services.AddTransient<MapaPage>();
		builder.Services.AddTransient<BilleteraViewModel>();
		builder.Services.AddTransient<BilleteraPage>();
		builder.Services.AddTransient<TablaValoresViewModel>();
		builder.Services.AddTransient<TablaValoresPage>();
		builder.Services.AddTransient<GuiaLimpiezaViewModel>();
		builder.Services.AddTransient<GuiaLimpiezaPage>();
		builder.Services.AddTransient<EcopesosGanadosViewModel>();
		builder.Services.AddTransient<EcopesosGanadosPage>();
		builder.Services.AddTransient<RedimirViewModel>();
		builder.Services.AddTransient<RedimirPage>();
		builder.Services.AddTransient<PerfilViewModel>();
		builder.Services.AddTransient<PerfilPage>();
		builder.Services.AddTransient<AdminInicioViewModel>();
		builder.Services.AddTransient<AdminInicioPage>();
		builder.Services.AddTransient<AdminUsuariosViewModel>();
		builder.Services.AddTransient<AdminUsuariosPage>();
		builder.Services.AddTransient<AdminReportesViewModel>();
		builder.Services.AddTransient<AdminReportesPage>();
		builder.Services.AddTransient<RestauranteInicioViewModel>();
		builder.Services.AddTransient<RestauranteInicioPage>();
		builder.Services.AddTransient<RestauranteBeneficiosViewModel>();
		builder.Services.AddTransient<RestauranteBeneficiosPage>();
		builder.Services.AddTransient<RestauranteHistorialViewModel>();
		builder.Services.AddTransient<RestauranteHistorialPage>();
		builder.Services.AddTransient<RestaurantePerfilViewModel>();
		builder.Services.AddTransient<RestaurantePerfilPage>();
		builder.Services.AddTransient<RecicladorRutaViewModel>();
		builder.Services.AddTransient<RecicladorRutaPage>();
		builder.Services.AddTransient<RecicladorConfirmarViewModel>();
		builder.Services.AddTransient<RecicladorConfirmarPage>();
		builder.Services.AddTransient<RecicladorHistorialViewModel>();
		builder.Services.AddTransient<RecicladorHistorialPage>();
		builder.Services.AddTransient<RecicladorPerfilViewModel>();
		builder.Services.AddTransient<RecicladorPerfilPage>();

		return builder.Build();
	}
}
