using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPACKING.App.Services;

namespace MPACKING.App.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly AuthApiService _authApi;
    private readonly SesionService _sesion;

    [ObservableProperty]
    private string correo = string.Empty;

    [ObservableProperty]
    private string clave = string.Empty;

    public LoginViewModel(AuthApiService authApi, SesionService sesion)
    {
        _authApi = authApi;
        _sesion = sesion;
    }

    [RelayCommand]
    private async Task IniciarSesionAsync()
    {
        MensajeError = string.Empty;

        if (string.IsNullOrWhiteSpace(Correo) || string.IsNullOrWhiteSpace(Clave))
        {
            MensajeError = "Escribe tu correo y tu clave.";
            return;
        }

        EstaOcupado = true;
        try
        {
            var token = await _authApi.LoginAsync(Correo.Trim(), Clave);
            await _sesion.GuardarSesionAsync(token);

            var destino = token.Usuario.Rol switch
            {
                "Admin" => nameof(Views.AdminInicioPage),
                "Restaurante" => nameof(Views.RestauranteInicioPage),
                "Reciclador" => nameof(Views.RecicladorRutaPage),
                _ => nameof(Views.InicioPage)
            };
            await Shell.Current.GoToAsync(destino);
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
    private static async Task IrARegistroAsync() =>
        await Shell.Current.GoToAsync(nameof(Views.RegistroPage));
}
