using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPACKING.App.Services;

namespace MPACKING.App.ViewModels;

public partial class RecicladorHistorialViewModel : BaseViewModel
{
    private static readonly CultureInfo CulturaCo = new("es-CO");
    private static readonly Color[] Colores =
    {
        Color.FromArgb("#E8F6E1"), Color.FromArgb("#D8ECFB"),
        Color.FromArgb("#FFF3C4"), Color.FromArgb("#EBE3FA")
    };

    private readonly RecicladorApiService _recicladorApi;

    [ObservableProperty]
    private string kgMesTexto = "0 kg";

    [ObservableProperty]
    private string recoleccionesMesTexto = "0 recolecciones";

    public ObservableCollection<MovimientoRecicladorVm> Movimientos { get; } = new();

    public RecicladorHistorialViewModel(RecicladorApiService recicladorApi)
    {
        _recicladorApi = recicladorApi;
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        MensajeError = string.Empty;
        EstaOcupado = true;
        try
        {
            var historial = await _recicladorApi.ObtenerHistorialAsync();

            KgMesTexto = $"{historial.KgMes.ToString("0.#", CulturaCo)} kg";
            RecoleccionesMesTexto = $"{historial.RecoleccionesMes.ToString("N0", CulturaCo)} recolecciones";

            Movimientos.Clear();
            var i = 0;
            foreach (var m in historial.Movimientos)
            {
                Movimientos.Add(new MovimientoRecicladorVm(
                    m.Usuario,
                    $"{m.Material} · {m.PesoKg.ToString("0.#", CulturaCo)} kg",
                    $"+{m.Ecopesos}",
                    Colores[i++ % Colores.Length]));
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
    private static async Task IrAConfirmarAsync()
    {
        await Shell.Current.GoToAsync("..");
        await Shell.Current.GoToAsync(nameof(Views.RecicladorConfirmarPage));
    }

    [RelayCommand]
    private static async Task IrAPerfilAsync()
    {
        await Shell.Current.GoToAsync("..");
        await Shell.Current.GoToAsync(nameof(Views.RecicladorPerfilPage));
    }
}
