using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPACKING.App.Services;

namespace MPACKING.App.ViewModels;

public partial class RegistroViewModel : BaseViewModel
{
    private readonly AuthApiService _authApi;
    private readonly SesionService _sesion;

    [ObservableProperty]
    private string nombreCompleto = string.Empty;

    [ObservableProperty]
    private string correo = string.Empty;

    [ObservableProperty]
    private string clave = string.Empty;

    [ObservableProperty]
    private bool aceptoTerminos;

    public RegistroViewModel(AuthApiService authApi, SesionService sesion)
    {
        _authApi = authApi;
        _sesion = sesion;
    }

    [RelayCommand]
    private async Task CrearCuentaAsync()
    {
        MensajeError = string.Empty;

        if (string.IsNullOrWhiteSpace(NombreCompleto) ||
            string.IsNullOrWhiteSpace(Correo) ||
            string.IsNullOrWhiteSpace(Clave))
        {
            MensajeError = "Completa todos los campos.";
            return;
        }

        if (!AceptoTerminos)
        {
            MensajeError = "Debes aceptar los terminos y la politica de privacidad.";
            return;
        }

        EstaOcupado = true;
        try
        {
            var token = await _authApi.RegistrarAsync(
                NombreCompleto.Trim(), Correo.Trim(), Clave, rol: "Usuario");
            await _sesion.GuardarSesionAsync(token);

            await Shell.Current.GoToAsync(nameof(Views.InicioPage));
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
    private static async Task IrALoginAsync() => await Shell.Current.GoToAsync("..");
}
