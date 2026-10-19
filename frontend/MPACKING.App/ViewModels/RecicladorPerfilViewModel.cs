using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPACKING.App.Services;

namespace MPACKING.App.ViewModels;

public partial class RecicladorPerfilViewModel : BaseViewModel
{
    private readonly RecicladorApiService _recicladorApi;
    private readonly SesionService _sesion;

    [ObservableProperty]
    private string nombre = string.Empty;

    [ObservableProperty]
    private string correo = string.Empty;

    [ObservableProperty]
    private string zona = string.Empty;

    public RecicladorPerfilViewModel(RecicladorApiService recicladorApi, SesionService sesion)
    {
        _recicladorApi = recicladorApi;
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
            var perfil = await _recicladorApi.ObtenerPerfilAsync();
            Nombre = perfil.Nombre;
            Zona = perfil.Zona ?? "Sin zona asignada";
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
    private static async Task IrAConfirmarAsync()
    {
        await Shell.Current.GoToAsync("..");
        await Shell.Current.GoToAsync(nameof(Views.RecicladorConfirmarPage));
    }

    [RelayCommand]
    private static async Task IrAHistorialAsync()
    {
        await Shell.Current.GoToAsync("..");
        await Shell.Current.GoToAsync(nameof(Views.RecicladorHistorialPage));
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
