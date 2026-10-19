using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPACKING.App.Services;

namespace MPACKING.App.ViewModels;

public partial class BilleteraViewModel : BaseViewModel
{
    private static readonly CultureInfo CulturaCo = new("es-CO");
    private const double MetaKgVisual = 20.0;

    private readonly ReciclajeApiService _reciclajeApi;

    [ObservableProperty]
    private string saldoTexto = "0";

    [ObservableProperty]
    private string equivalenteTexto = "0 COP";

    [ObservableProperty]
    private string totalKgTexto = "0 kg";

    [ObservableProperty]
    private double progresoImpacto;

    [ObservableProperty]
    private bool hayMovimientos;

    public ObservableCollection<MovimientoVm> Movimientos { get; } = new();

    public BilleteraViewModel(ReciclajeApiService reciclajeApi)
    {
        _reciclajeApi = reciclajeApi;
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
            EquivalenteTexto = $"{billetera.EquivalenteCop.ToString("N0", CulturaCo)} COP";
            TotalKgTexto = $"{billetera.TotalKgReciclados.ToString("0.#", CulturaCo)} kg";
            ProgresoImpacto = Math.Min(1.0, (double)billetera.TotalKgReciclados / MetaKgVisual);

            Movimientos.Clear();
            foreach (var m in billetera.Movimientos)
            {
                Movimientos.Add(new MovimientoVm(
                    $"{m.Descripcion} · {m.PesoKg.ToString("0.#", CulturaCo)} kg",
                    m.Fecha.ToLocalTime().ToString("dd/MM  h:mm tt", CulturaCo),
                    $"+{m.Ecopesos}"));
            }
            HayMovimientos = Movimientos.Count > 0;
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
    private static async Task IrPerfilAsync()
    {
        await Shell.Current.GoToAsync("..");
        await Shell.Current.GoToAsync(nameof(Views.PerfilPage));
    }
}
