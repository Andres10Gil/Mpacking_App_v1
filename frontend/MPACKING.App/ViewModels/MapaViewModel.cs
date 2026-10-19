using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPACKING.App.Services;

namespace MPACKING.App.ViewModels;

public partial class MapaViewModel : BaseViewModel
{
    private static readonly CultureInfo CulturaCo = new("es-CO");

    // Ubicacion de referencia mientras no hay GPS real conectado en la app.
    private const decimal LatitudDemo = 4.6589m;
    private const decimal LongitudDemo = -74.1089m;
    private const int IdRecicladorDemo = 1;

    private readonly GeoApiService _geoApi;

    [ObservableProperty]
    private bool hayRestauranteCercano;

    [ObservableProperty]
    private int idMasCercano;

    [ObservableProperty]
    private string nombreMasCercano = string.Empty;

    [ObservableProperty]
    private string direccionMasCercano = string.Empty;

    [ObservableProperty]
    private string detalleMasCercano = string.Empty;

    [ObservableProperty]
    private bool hayRecicladorEnRuta;

    public MapaViewModel(GeoApiService geoApi)
    {
        _geoApi = geoApi;
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        MensajeError = string.Empty;
        EstaOcupado = true;
        try
        {
            var restaurantes = await _geoApi.BuscarRestaurantesCercanosAsync(LatitudDemo, LongitudDemo);
            var masCercano = restaurantes.FirstOrDefault();

            HayRestauranteCercano = masCercano is not null;
            if (masCercano is not null)
            {
                IdMasCercano = masCercano.Id;
                NombreMasCercano = masCercano.Nombre;
                DireccionMasCercano = masCercano.Direccion;
                DetalleMasCercano =
                    $"{masCercano.DistanciaKm.ToString("0.#", CulturaCo)} km · {masCercano.Direccion}";
            }

            var posicion = await _geoApi.ObtenerPosicionRecicladorAsync(IdRecicladorDemo);
            HayRecicladorEnRuta = posicion is not null;
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
    private async Task VerBeneficiosAsync()
    {
        await Shell.Current.GoToAsync(
            $"{nameof(Views.RedimirPage)}" +
            $"?idRestaurante={IdMasCercano}" +
            $"&nombre={Uri.EscapeDataString(NombreMasCercano)}" +
            $"&direccion={Uri.EscapeDataString(DireccionMasCercano)}");
    }

    [RelayCommand]
    private static async Task VolverAsync() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private static async Task IrEscanearAsync() =>
        await Shell.Current.DisplayAlertAsync("Escanear QR", "Entra desde Inicio para escanear.", "Listo");

    [RelayCommand]
    private static async Task IrBilleteraAsync()
    {
        await Shell.Current.GoToAsync("..");
        await Shell.Current.GoToAsync(nameof(Views.BilleteraPage));
    }

    [RelayCommand]
    private static async Task IrPerfilAsync()
    {
        await Shell.Current.GoToAsync("..");
        await Shell.Current.GoToAsync(nameof(Views.PerfilPage));
    }
}
