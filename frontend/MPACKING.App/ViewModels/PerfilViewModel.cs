using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPACKING.App.Services;

namespace MPACKING.App.ViewModels;

public partial class PerfilViewModel : BaseViewModel
{
    private static readonly CultureInfo CulturaCo = new("es-CO");

    private readonly ReciclajeApiService _reciclajeApi;
    private readonly RedencionApiService _redencionApi;
    private readonly SesionService _sesion;

    [ObservableProperty]
    private string nombre = string.Empty;

    [ObservableProperty]
    private string correo = string.Empty;

    [ObservableProperty]
    private string rol = string.Empty;

    [ObservableProperty]
    private string saldoTexto = "0";

    [ObservableProperty]
    private string kgTexto = "0";

    [ObservableProperty]
    private string canjesTexto = "0";

    public PerfilViewModel(ReciclajeApiService reciclajeApi, RedencionApiService redencionApi, SesionService sesion)
    {
        _reciclajeApi = reciclajeApi;
        _redencionApi = redencionApi;
        _sesion = sesion;

        var usuario = _sesion.UsuarioActual;
        Nombre = usuario?.Nombre ?? "";
        Correo = usuario?.Correo ?? "";
        Rol = usuario?.Rol ?? "";
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        MensajeError = string.Empty;
        EstaOcupado = true;
        try
        {
            var billetera = await _reciclajeApi.ObtenerBilleteraAsync();
            SaldoTexto = billetera.SaldoEcopesos.ToString("N0", CulturaCo);
            KgTexto = billetera.TotalKgReciclados.ToString("0.#", CulturaCo);

            var redenciones = await _redencionApi.ListarMisRedencionesAsync();
            CanjesTexto = redenciones.Count.ToString(CulturaCo);
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
    private static async Task IrTablaValoresAsync() =>
        await Shell.Current.GoToAsync(nameof(Views.TablaValoresPage));

    [RelayCommand]
    private async Task CerrarSesionAsync()
    {
        var confirmar = await Shell.Current.DisplayAlertAsync(
            "Cerrar sesion", "Seguro que quieres salir?", "Si, salir", "Cancelar");

        if (!confirmar)
            return;

        _sesion.CerrarSesion();
        await Shell.Current.GoToAsync("//LoginPage");
    }

    [RelayCommand]
    private static async Task IrEscanearAsync()
    {
        await Shell.Current.GoToAsync("..");
        await Shell.Current.GoToAsync(nameof(Views.EscanearPage));
    }

    [RelayCommand]
    private static async Task IrMapaAsync()
    {
        await Shell.Current.GoToAsync("..");
        await Shell.Current.GoToAsync(nameof(Views.MapaPage));
    }

    [RelayCommand]
    private static async Task IrBilleteraAsync()
    {
        await Shell.Current.GoToAsync("..");
        await Shell.Current.GoToAsync(nameof(Views.BilleteraPage));
    }
}
