using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPACKING.App.Models;
using MPACKING.App.Services;

namespace MPACKING.App.ViewModels;

public partial class EscanearViewModel : BaseViewModel
{
    private static readonly CultureInfo CulturaCo = new("es-CO");

    // Fijo por ahora: solo hay un reciclador cargado en los datos de prueba.
    private const int IdReciclador = 1;

    private readonly ReciclajeApiService _reciclajeApi;
    private QrInfoResponse? _qrConsultado;
    private string _codigoHash = string.Empty;

    [ObservableProperty]
    private bool hayQrConsultado;

    [ObservableProperty]
    private string material = string.Empty;

    [ObservableProperty]
    private string pesoTexto = string.Empty;

    [ObservableProperty]
    private string estadoTexto = string.Empty;

    [ObservableProperty]
    private string ecopesosTexto = string.Empty;

    public EscanearViewModel(ReciclajeApiService reciclajeApi)
    {
        _reciclajeApi = reciclajeApi;
    }

    [RelayCommand]
    private async Task EscanearAsync()
    {
        // Todavia no hay lectura de camara: mientras tanto, se escribe el
        // codigo a mano. El paso siguiente es reemplazar esto por un
        // lector de QR real (ej. con ZXing.Net.Maui).
        var codigo = await Shell.Current.DisplayPromptAsync(
            "Escanear QR",
            "Escribe o pega el codigo del QR (por ahora no hay camara conectada):",
            "Consultar", "Cancelar");

        if (string.IsNullOrWhiteSpace(codigo))
            return;

        MensajeError = string.Empty;
        EstaOcupado = true;
        try
        {
            _codigoHash = codigo.Trim();
            _qrConsultado = await _reciclajeApi.ConsultarQrAsync(_codigoHash);

            Material = _qrConsultado.Material;
            PesoTexto = $"{_qrConsultado.PesoKg.ToString("0.#", CulturaCo)} kg";
            EstadoTexto = _qrConsultado.EsCanjeable ? "ACTIVO" : "NO DISPONIBLE";
            EcopesosTexto = $"+{_qrConsultado.EcopesosEstimados} ecopesos";
            HayQrConsultado = true;
        }
        catch (ApiException ex)
        {
            HayQrConsultado = false;
            MensajeError = ex.Message;
        }
        finally
        {
            EstaOcupado = false;
        }
    }

    [RelayCommand]
    private async Task ConfirmarEscaneoAsync()
    {
        if (_qrConsultado is null)
            return;

        MensajeError = string.Empty;
        EstaOcupado = true;
        try
        {
            var resultado = await _reciclajeApi.ConfirmarAsync(
                new ConfirmarReciclajeRequest(_codigoHash, IdReciclador, _qrConsultado.PesoKg));

            HayQrConsultado = false;
            _qrConsultado = null;

            var pesoTexto = resultado.PesoKg.ToString("0.#", CulturaCo);
            var co2Texto = resultado.Co2Evitado.ToString("0.##", CulturaCo);

            await Shell.Current.GoToAsync("..");
            await Shell.Current.GoToAsync(
                $"{nameof(Views.EcopesosGanadosPage)}" +
                $"?ecopesos={resultado.EcopesosGanados}" +
                $"&material={Uri.EscapeDataString(resultado.Material)}" +
                $"&peso={Uri.EscapeDataString(pesoTexto)}" +
                $"&co2={Uri.EscapeDataString(co2Texto)}");
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
    private static async Task IrMapaAsync()
    {
        await Shell.Current.GoToAsync("..");
        await Shell.Current.GoToAsync(nameof(Views.MapaPage));
    }

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
