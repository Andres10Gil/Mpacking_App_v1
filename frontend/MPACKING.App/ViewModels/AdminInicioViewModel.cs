using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPACKING.App.Services;

namespace MPACKING.App.ViewModels;

public partial class AdminInicioViewModel : BaseViewModel
{
    private static readonly CultureInfo CulturaCo = new("es-CO");

    /// <summary>Meta visual para la barra de progreso de CO2 del mes.</summary>
    private const double MetaCo2VisualKg = 30.0;

    private readonly AdminApiService _adminApi;

    [ObservableProperty]
    private string usuariosActivosTexto = "0";

    [ObservableProperty]
    private string qrEscaneadosTexto = "0";

    [ObservableProperty]
    private string ecopesosDadosTexto = "0";

    [ObservableProperty]
    private string materialKgTexto = "0";

    [ObservableProperty]
    private string co2EvitadoMesTexto = "0 kg";

    [ObservableProperty]
    private double progresoCo2Mes;

    [ObservableProperty]
    private bool hayAlertas;

    public ObservableCollection<AlertaVm> Alertas { get; } = new();

    public AdminInicioViewModel(AdminApiService adminApi)
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
            var resumen = await _adminApi.ObtenerResumenAsync();

            UsuariosActivosTexto = resumen.UsuariosActivos.ToString("N0", CulturaCo);
            QrEscaneadosTexto = resumen.QrEscaneados.ToString("N0", CulturaCo);
            EcopesosDadosTexto = resumen.EcopesosDados.ToString("N0", CulturaCo);
            MaterialKgTexto = resumen.MaterialKgTotal.ToString("0.#", CulturaCo);
            Co2EvitadoMesTexto = $"{resumen.Co2EvitadoMes.ToString("0.#", CulturaCo)} kg";
            ProgresoCo2Mes = Math.Min(1.0, (double)resumen.Co2EvitadoMes / MetaCo2VisualKg);

            Alertas.Clear();
            foreach (var a in resumen.Alertas)
                Alertas.Add(new AlertaVm(a.Titulo, a.Detalle, a.Etiqueta));
            HayAlertas = Alertas.Count > 0;
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
    private static async Task IrAUsuariosAsync() =>
        await Shell.Current.GoToAsync(nameof(Views.AdminUsuariosPage));

    [RelayCommand]
    private static async Task IrAReportesAsync() =>
        await Shell.Current.GoToAsync(nameof(Views.AdminReportesPage));

    [RelayCommand]
    private static async Task IrAConfigAsync() =>
        await Shell.Current.DisplayAlertAsync("Configuracion", "Esta seccion estara disponible pronto.", "Listo");
}
