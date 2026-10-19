using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPACKING.App.Services;

namespace MPACKING.App.ViewModels;

public partial class RestauranteHistorialViewModel : BaseViewModel
{
    private static readonly CultureInfo CulturaCo = new("es-CO");
    private static readonly Color ColorAprobado = Color.FromArgb("#E8F6E1");
    private static readonly Color ColorOtro = Color.FromArgb("#FFE0E3");

    private readonly RestauranteApiService _restauranteApi;

    [ObservableProperty]
    private string totalMesTexto = "$0";

    [ObservableProperty]
    private string redencionesMesTexto = "0 redenciones";

    public ObservableCollection<PagoHistorialVm> Pagos { get; } = new();

    public RestauranteHistorialViewModel(RestauranteApiService restauranteApi)
    {
        _restauranteApi = restauranteApi;
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        MensajeError = string.Empty;
        EstaOcupado = true;
        try
        {
            var historial = await _restauranteApi.ObtenerHistorialAsync();

            TotalMesTexto = $"${historial.TotalMesCop.ToString("N0", CulturaCo)}";
            RedencionesMesTexto = $"{historial.RedencionesMes.ToString("N0", CulturaCo)} redenciones";

            Pagos.Clear();
            foreach (var p in historial.Pagos)
            {
                var color = p.Estado == "Aprobado" ? ColorAprobado : ColorOtro;
                Pagos.Add(new PagoHistorialVm(p.Usuario, p.Detalle, p.MontoTexto, color));
            }
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
    private static async Task IrAPerfilAsync()
    {
        await Shell.Current.GoToAsync("..");
        await Shell.Current.GoToAsync(nameof(Views.RestaurantePerfilPage));
    }
}
