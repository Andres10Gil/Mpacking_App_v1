using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPACKING.App.Services;

namespace MPACKING.App.ViewModels;

public partial class InicioViewModel : BaseViewModel
{
    private static readonly CultureInfo CulturaCo = new("es-CO");
    private const double MetaKgVisual = 20.0;

    private readonly ReciclajeApiService _reciclajeApi;
    private readonly SesionService _sesion;

    [ObservableProperty]
    private string nombreUsuario = string.Empty;

    [ObservableProperty]
    private string saldoTexto = "0";

    [ObservableProperty]
    private string equivalenteTexto = "$0 COP";

    [ObservableProperty]
    private string totalKgTexto = "0 kg";

    [ObservableProperty]
    private double progresoImpacto;

    [ObservableProperty]
    private bool hayMovimientos;

    [ObservableProperty]
    private string ultimoMovimientoTitulo = string.Empty;

    [ObservableProperty]
    private string ultimoMovimientoFecha = string.Empty;

    [ObservableProperty]
    private string ultimoMovimientoEcopesos = string.Empty;

    public InicioViewModel(ReciclajeApiService reciclajeApi, SesionService sesion)
    {
        _reciclajeApi = reciclajeApi;
        _sesion = sesion;
        NombreUsuario = _sesion.UsuarioActual?.Nombre.Split(' ')[0] ?? "";
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
            EquivalenteTexto = $"${billetera.EquivalenteCop.ToString("N0", CulturaCo)} COP";
            TotalKgTexto = $"{billetera.TotalKgReciclados.ToString("0.#", CulturaCo)} kg";
            ProgresoImpacto = Math.Min(1.0, (double)billetera.TotalKgReciclados / MetaKgVisual);

            var ultimo = billetera.Movimientos.FirstOrDefault();
            HayMovimientos = ultimo is not null;
            if (ultimo is not null)
            {
                UltimoMovimientoTitulo = $"{ultimo.Descripcion} · {ultimo.PesoKg.ToString("0.#", CulturaCo)} kg";
                UltimoMovimientoFecha = ultimo.Fecha.ToLocalTime().ToString("dd/MM  h:mm tt", CulturaCo);
                UltimoMovimientoEcopesos = $"+{ultimo.Ecopesos}";
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
    private static async Task IrEscanearAsync() =>
        await Shell.Current.GoToAsync(nameof(Views.EscanearPage));

    [RelayCommand]
    private static async Task IrMapaAsync() =>
        await Shell.Current.GoToAsync(nameof(Views.MapaPage));

    [RelayCommand]
    private static async Task IrBilleteraAsync() =>
        await Shell.Current.GoToAsync(nameof(Views.BilleteraPage));

    [RelayCommand]
    private static async Task IrPerfilAsync() =>
        await Shell.Current.GoToAsync(nameof(Views.PerfilPage));
}
