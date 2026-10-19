using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPACKING.App.Models;
using MPACKING.App.Services;

namespace MPACKING.App.ViewModels;

public partial class RecicladorConfirmarViewModel : BaseViewModel
{
    private static readonly CultureInfo CulturaCo = new("es-CO");

    private readonly ReciclajeApiService _reciclajeApi;
    private readonly RecicladorApiService _recicladorApi;
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

    public RecicladorConfirmarViewModel(
        ReciclajeApiService reciclajeApi, RecicladorApiService recicladorApi)
    {
        _reciclajeApi = reciclajeApi;
        _recicladorApi = recicladorApi;
    }

    [RelayCommand]
    private async Task EscanearAsync()
    {
        var codigo = await Shell.Current.DisplayPromptAsync(
            "Confirmar material",
            "Escribe el codigo del QR que te muestra el cliente:",
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
    private async Task ConfirmarRecoleccionAsync()
    {
        if (_qrConsultado is null)
            return;

        var correo = await Shell.Current.DisplayPromptAsync(
            "Datos del cliente",
            "Escribe el correo con el que el cliente tiene su cuenta MPACKING:",
            "Confirmar", "Cancelar",
            keyboard: Keyboard.Email);

        if (string.IsNullOrWhiteSpace(correo))
            return;

        MensajeError = string.Empty;
        EstaOcupado = true;
        try
        {
            var resultado = await _recicladorApi.ConfirmarRecoleccionAsync(
                _codigoHash, correo.Trim(), _qrConsultado.PesoKg);

            HayQrConsultado = false;
            _qrConsultado = null;

            await Shell.Current.DisplayAlertAsync(
                "Recoleccion confirmada",
                $"{resultado.Material} · {resultado.PesoKg.ToString("0.#", CulturaCo)} kg\n" +
                $"El cliente gano {resultado.EcopesosGanados} ecopesos.",
                "Listo");
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
        await Shell.Current.GoToAsync(nameof(Views.RecicladorHistorialPage));
    }

    [RelayCommand]
    private static async Task IrAPerfilAsync()
    {
        await Shell.Current.GoToAsync("..");
        await Shell.Current.GoToAsync(nameof(Views.RecicladorPerfilPage));
    }
}
