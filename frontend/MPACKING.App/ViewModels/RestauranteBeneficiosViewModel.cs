using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPACKING.App.Services;

namespace MPACKING.App.ViewModels;

public partial class RestauranteBeneficiosViewModel : BaseViewModel
{
    private static readonly CultureInfo CulturaCo = new("es-CO");

    private readonly RestauranteApiService _restauranteApi;

    public ObservableCollection<BeneficioRestauranteVm> Beneficios { get; } = new();

    public RestauranteBeneficiosViewModel(RestauranteApiService restauranteApi)
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
            var beneficios = await _restauranteApi.ListarBeneficiosAsync();

            Beneficios.Clear();
            foreach (var b in beneficios)
            {
                Beneficios.Add(new BeneficioRestauranteVm(
                    b.Id, b.Descripcion,
                    $"{b.EcopesosCosto.ToString("N0", CulturaCo)} ecp",
                    b.Activo));
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
    private async Task AnadirBeneficioAsync()
    {
        var descripcion = await Shell.Current.DisplayPromptAsync(
            "Nuevo beneficio", "Que recibe el cliente a cambio de sus ecopesos?",
            "Siguiente", "Cancelar");

        if (string.IsNullOrWhiteSpace(descripcion))
            return;

        var costoTexto = await Shell.Current.DisplayPromptAsync(
            "Nuevo beneficio", "Cuantos ecopesos cuesta?",
            "Guardar", "Cancelar", keyboard: Keyboard.Numeric);

        if (!int.TryParse(costoTexto, out var costo) || costo <= 0)
        {
            MensajeError = "El costo en ecopesos debe ser un numero mayor a cero.";
            return;
        }

        MensajeError = string.Empty;
        EstaOcupado = true;
        try
        {
            await _restauranteApi.CrearBeneficioAsync(descripcion.Trim(), costo);
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
    private async Task CambiarEstadoAsync(BeneficioRestauranteVm beneficio)
    {
        MensajeError = string.Empty;
        EstaOcupado = true;
        try
        {
            var activar = !beneficio.Activo;
            await _restauranteApi.CambiarEstadoBeneficioAsync(beneficio.Id, activar);
            beneficio.Activo = activar;
            beneficio.EstadoTexto = activar ? "Activo" : "Pausado";
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
    private static async Task IrAHistorialAsync()
    {
        await Shell.Current.GoToAsync("..");
        await Shell.Current.GoToAsync(nameof(Views.RestauranteHistorialPage));
    }

    [RelayCommand]
    private static async Task IrAPerfilAsync()
    {
        await Shell.Current.GoToAsync("..");
        await Shell.Current.GoToAsync(nameof(Views.RestaurantePerfilPage));
    }
}
