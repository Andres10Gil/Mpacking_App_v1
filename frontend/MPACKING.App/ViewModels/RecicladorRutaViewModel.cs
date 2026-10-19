using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPACKING.App.Services;

namespace MPACKING.App.ViewModels;

public partial class RecicladorRutaViewModel : BaseViewModel
{
    private static readonly CultureInfo CulturaCo = new("es-CO");

    // Ubicacion de referencia mientras no hay GPS real conectado en la app.
    private const decimal LatitudDemo = 4.6589m;
    private const decimal LongitudDemo = -74.1089m;

    private readonly RecicladorApiService _recicladorApi;
    private readonly GeoApiService _geoApi;

    [ObservableProperty]
    private string nombre = string.Empty;

    [ObservableProperty]
    private string zona = string.Empty;

    [ObservableProperty]
    private string kgHoyTexto = "0 kg";

    [ObservableProperty]
    private string kgMesTexto = "0 kg";

    [ObservableProperty]
    private bool enRuta;

    public RecicladorRutaViewModel(RecicladorApiService recicladorApi, GeoApiService geoApi)
    {
        _recicladorApi = recicladorApi;
        _geoApi = geoApi;
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
            KgHoyTexto = $"{perfil.KgHoy.ToString("0.#", CulturaCo)} kg";
            KgMesTexto = $"{perfil.KgMes.ToString("0.#", CulturaCo)} kg";
            EnRuta = perfil.EnRuta;
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
    private async Task ReportarUbicacionAsync()
    {
        MensajeError = string.Empty;
        EstaOcupado = true;
        try
        {
            await _geoApi.ReportarPosicionAsync(LatitudDemo, LongitudDemo);
            await CargarAsync();
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
    private static async Task IrAConfirmarAsync() =>
        await Shell.Current.GoToAsync(nameof(Views.RecicladorConfirmarPage));

    [RelayCommand]
    private static async Task IrAHistorialAsync() =>
        await Shell.Current.GoToAsync(nameof(Views.RecicladorHistorialPage));

    [RelayCommand]
    private static async Task IrAPerfilAsync() =>
        await Shell.Current.GoToAsync(nameof(Views.RecicladorPerfilPage));
}
