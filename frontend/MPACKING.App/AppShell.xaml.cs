using MPACKING.App.Views;

namespace MPACKING.App;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(RegistroPage), typeof(RegistroPage));
		Routing.RegisterRoute(nameof(InicioPage), typeof(InicioPage));
		Routing.RegisterRoute(nameof(EscanearPage), typeof(EscanearPage));
		Routing.RegisterRoute(nameof(MapaPage), typeof(MapaPage));
		Routing.RegisterRoute(nameof(BilleteraPage), typeof(BilleteraPage));
		Routing.RegisterRoute(nameof(TablaValoresPage), typeof(TablaValoresPage));
		Routing.RegisterRoute(nameof(GuiaLimpiezaPage), typeof(GuiaLimpiezaPage));
		Routing.RegisterRoute(nameof(EcopesosGanadosPage), typeof(EcopesosGanadosPage));
		Routing.RegisterRoute(nameof(RedimirPage), typeof(RedimirPage));
		Routing.RegisterRoute(nameof(PerfilPage), typeof(PerfilPage));
		Routing.RegisterRoute(nameof(AdminInicioPage), typeof(AdminInicioPage));
		Routing.RegisterRoute(nameof(AdminUsuariosPage), typeof(AdminUsuariosPage));
		Routing.RegisterRoute(nameof(AdminReportesPage), typeof(AdminReportesPage));
		Routing.RegisterRoute(nameof(RestauranteInicioPage), typeof(RestauranteInicioPage));
		Routing.RegisterRoute(nameof(RestauranteBeneficiosPage), typeof(RestauranteBeneficiosPage));
		Routing.RegisterRoute(nameof(RestauranteHistorialPage), typeof(RestauranteHistorialPage));
		Routing.RegisterRoute(nameof(RestaurantePerfilPage), typeof(RestaurantePerfilPage));
		Routing.RegisterRoute(nameof(RecicladorRutaPage), typeof(RecicladorRutaPage));
		Routing.RegisterRoute(nameof(RecicladorConfirmarPage), typeof(RecicladorConfirmarPage));
		Routing.RegisterRoute(nameof(RecicladorHistorialPage), typeof(RecicladorHistorialPage));
		Routing.RegisterRoute(nameof(RecicladorPerfilPage), typeof(RecicladorPerfilPage));
	}
}
