using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPACKING.App.Services;

namespace MPACKING.App.ViewModels;

public partial class AdminReportesViewModel : BaseViewModel
{
    private static readonly CultureInfo CulturaCo = new("es-CO");

    private readonly AdminApiService _adminApi;

    [ObservableProperty]
    private string co2TotalTexto = "0 kg";

    [ObservableProperty]
    private string arbolesTexto = "equivale a 0 arboles";

    public ObservableCollection<DistribucionVm> Distribucion { get; } = new();
    public ObservableCollection<TotalMaterialVm> Totales { get; } = new();

    public AdminReportesViewModel(AdminApiService adminApi)
    {
        _adminApi = adminApi;
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        MensajeError = string.Empty;
        EstaOcupado = true;
        try
        {
            var reporte = await _adminApi.ObtenerReporteAsync();

            Co2TotalTexto = $"{reporte.Co2Total.ToString("N0", CulturaCo)} kg";
            ArbolesTexto = $"equivale a {reporte.ArbolesEquivalentes} arboles";

            Distribucion.Clear();
            foreach (var d in reporte.Distribucion)
            {
                var porcentaje = (double)d.Porcentaje;
                Distribucion.Add(new DistribucionVm(
                    d.Material, porcentaje, $"{Math.Round(porcentaje * 100)}%"));
            }

            Totales.Clear();
            foreach (var t in reporte.Totales)
            {
                Totales.Add(new TotalMaterialVm(
                    t.Material,
                    $"{t.KgTotal.ToString("0.#", CulturaCo)} kg",
                    $"{t.EcopesosTotal.ToString("N0", CulturaCo)} ecp"));
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
    private static async Task IrAUsuariosAsync()
    {
        await Shell.Current.GoToAsync("..");
        await Shell.Current.GoToAsync(nameof(Views.AdminUsuariosPage));
    }

    [RelayCommand]
    private static async Task IrAConfigAsync() =>
        await Shell.Current.DisplayAlertAsync("Configuracion", "Esta seccion estara disponible pronto.", "Listo");
}
