using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPACKING.App.Services;

namespace MPACKING.App.ViewModels;

public partial class RestaurantePerfilViewModel : BaseViewModel
{
    private readonly RestauranteApiService _restauranteApi;
    private readonly SesionService _sesion;

    [ObservableProperty]
    private string nombre = string.Empty;

    [ObservableProperty]
    private string correo = string.Empty;

    [ObservableProperty]
    private string direccion = string.Empty;

    public RestaurantePerfilViewModel(RestauranteApiService restauranteApi, SesionService sesion)
    {
        _restauranteApi = restauranteApi;
        _sesion = sesion;
        Correo = _sesion.UsuarioActual?.Correo ?? string.Empty;
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        MensajeError = string.Empty;
        EstaOcupado = true;
        try
        {
            var resumen = await _restauranteApi.ObtenerResumenAsync();
            Nombre = resumen.NombreRestaurante;
            Direccion = resumen.Direccion;
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }
        finally
        {
            EstaOcupado = false;
        }
    }

    [RelayCommand]
    private static async Task VolverAsync() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private static async Task IrABeneficiosAsync()
    {
        await Shell.Current.GoToAsync("..");
        await Shell.Current.GoToAsync(nameof(Views.RestauranteBeneficiosPage));
    }

    [RelayCommand]
    private static async Task IrAHistorialAsync()
    {
        await Shell.Current.GoToAsync("..");
        await Shell.Current.GoToAsync(nameof(Views.RestauranteHistorialPage));
    }

    [RelayCommand]
    private async Task CerrarSesionAsync()
    {
        var confirmar = await Shell.Current.DisplayAlertAsync(
            "Cerrar sesion", "Seguro que quieres salir?", "Si, salir", "Cancelar");
        if (!confirmar) return;

        _sesion.CerrarSesion();
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
