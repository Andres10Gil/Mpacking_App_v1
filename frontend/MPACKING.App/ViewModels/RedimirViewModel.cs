using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPACKING.App.Services;

namespace MPACKING.App.ViewModels;

[QueryProperty(nameof(IdRestaurante), "idRestaurante")]
[QueryProperty(nameof(NombreRestaurante), "nombre")]
[QueryProperty(nameof(DireccionRestaurante), "direccion")]
public partial class RedimirViewModel : BaseViewModel
{
    private static readonly CultureInfo CulturaCo = new("es-CO");
    private const decimal CopPorEcopeso = 10m;

    private readonly RedencionApiService _redencionApi;
    private readonly ReciclajeApiService _reciclajeApi;

    [ObservableProperty]
    private int idRestaurante;

    [ObservableProperty]
    private string nombreRestaurante = string.Empty;

    [ObservableProperty]
    private string direccionRestaurante = string.Empty;

    [ObservableProperty]
    private string saldoTexto = "0";

    [ObservableProperty]
    private bool haySeleccion;

    [ObservableProperty]
    private string resumenEcopesosTexto = string.Empty;

    [ObservableProperty]
    private string resumenCopTexto = string.Empty;

    private BeneficioVm? _seleccionado;

    public ObservableCollection<BeneficioVm> Beneficios { get; } = new();

    public RedimirViewModel(RedencionApiService redencionApi, ReciclajeApiService reciclajeApi)
    {
        _redencionApi = redencionApi;
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

            var beneficios = await _redencionApi.ListarBeneficiosAsync(IdRestaurante);

            Beneficios.Clear();
            foreach (var b in beneficios.Where(b => b.Disponible))
                Beneficios.Add(new BeneficioVm(b.Id, b.Descripcion, b.EcopesosCosto));

            if (Beneficios.Count > 0)
                Seleccionar(Beneficios[0]);
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
    private void Seleccionar(BeneficioVm beneficio)
    {
        if (_seleccionado is not null)
            _seleccionado.EstaSeleccionado = false;

        _seleccionado = beneficio;
        beneficio.EstaSeleccionado = true;

        HaySeleccion = true;
        ResumenEcopesosTexto = $"-{beneficio.EcopesosCosto} ecp";
        ResumenCopTexto = $"${(beneficio.EcopesosCosto * CopPorEcopeso).ToString("N0", CulturaCo)} COP";
    }

    [RelayCommand]
    private async Task ConfirmarCanjeAsync()
    {
        if (_seleccionado is null)
            return;

        MensajeError = string.Empty;
        EstaOcupado = true;
        try
        {
            var resultado = await _redencionApi.SolicitarAsync(_seleccionado.Id);

            await Shell.Current.DisplayAlertAsync(
                "Canje solicitado",
                $"Presenta este codigo en el restaurante para que te lo validen:\n\n{resultado.CodigoUnico}",
                "Listo");

            await Shell.Current.GoToAsync("..");
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
}
