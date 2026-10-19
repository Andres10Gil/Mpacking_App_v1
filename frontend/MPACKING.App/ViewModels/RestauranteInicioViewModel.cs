using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPACKING.App.Services;

namespace MPACKING.App.ViewModels;

public partial class RestauranteInicioViewModel : BaseViewModel
{
    private static readonly CultureInfo CulturaCo = new("es-CO");
    private static readonly Color ColorAprobada = Color.FromArgb("#E8F6E1");
    private static readonly Color ColorPendiente = Color.FromArgb("#FFF3C4");
    private static readonly Color ColorOtro = Color.FromArgb("#FFE0E3");

    private readonly RestauranteApiService _restauranteApi;

    [ObservableProperty]
    private string nombreRestaurante = string.Empty;

    [ObservableProperty]
    private string direccion = string.Empty;

    [ObservableProperty]
    private string recibidoHoyTexto = "$0";

    [ObservableProperty]
    private string redencionesHoyTexto = "0";

    [ObservableProperty]
    private string ecopesosValidadosTexto = "0";

    public ObservableCollection<RedencionRecienteVm> Ultimas { get; } = new();

    public RestauranteInicioViewModel(RestauranteApiService restauranteApi)
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
            var resumen = await _restauranteApi.ObtenerResumenAsync();

            NombreRestaurante = resumen.NombreRestaurante;
            Direccion = resumen.Direccion;
            RecibidoHoyTexto = $"${resumen.RecibidoHoyCop.ToString("N0", CulturaCo)}";
            RedencionesHoyTexto = resumen.RedencionesHoy.ToString("N0", CulturaCo);
            EcopesosValidadosTexto = resumen.EcopesosValidadosHoy.ToString("N0", CulturaCo);

            Ultimas.Clear();
            foreach (var r in resumen.Ultimas)
            {
                var color = r.Estado switch
                {
                    "Aprobada" => ColorAprobada,
                    "Pendiente" => ColorPendiente,
                    _ => ColorOtro
                };
                Ultimas.Add(new RedencionRecienteVm(r.Usuario, r.Detalle, r.MontoTexto, color));
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
    private async Task ValidarRedencionAsync()
    {
        var codigo = await Shell.Current.DisplayPromptAsync(
            "Validar redencion",
            "Escribe el codigo que te muestra el cliente:",
            "Validar", "Cancelar");

        if (string.IsNullOrWhiteSpace(codigo))
            return;

        MensajeError = string.Empty;
        EstaOcupado = true;
        try
        {
            await _restauranteApi.ValidarCodigoAsync(codigo.Trim());
            await Shell.Current.DisplayAlertAsync("Listo", "La redencion fue validada y pagada.", "Aceptar");
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
    private static async Task IrABeneficiosAsync() =>
        await Shell.Current.GoToAsync(nameof(Views.RestauranteBeneficiosPage));

    [RelayCommand]
    private static async Task IrAHistorialAsync() =>
        await Shell.Current.GoToAsync(nameof(Views.RestauranteHistorialPage));

    [RelayCommand]
    private static async Task IrAPerfilAsync() =>
        await Shell.Current.GoToAsync(nameof(Views.RestaurantePerfilPage));
}
