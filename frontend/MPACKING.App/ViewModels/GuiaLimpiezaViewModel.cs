using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPACKING.App.Services;

namespace MPACKING.App.ViewModels;

public record PasoVm(int Numero, string Texto);

[QueryProperty(nameof(IdMaterial), "idMaterial")]
public partial class GuiaLimpiezaViewModel : BaseViewModel
{
    private readonly CatalogoApiService _catalogoApi;

    [ObservableProperty]
    private int idMaterial;

    [ObservableProperty]
    private string nombreMaterial = string.Empty;

    [ObservableProperty]
    private string datoCurioso = string.Empty;

    [ObservableProperty]
    private bool hayDatoCurioso;

    [ObservableProperty]
    private bool hayErrores;

    public ObservableCollection<PasoVm> Pasos { get; } = new();
    public ObservableCollection<string> Errores { get; } = new();

    public GuiaLimpiezaViewModel(CatalogoApiService catalogoApi)
    {
        _catalogoApi = catalogoApi;
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        if (IdMaterial <= 0)
            return;

        MensajeError = string.Empty;
        EstaOcupado = true;
        try
        {
            var guia = await _catalogoApi.ObtenerGuiaAsync(IdMaterial);

            NombreMaterial = guia.Material;
            DatoCurioso = guia.DatoCurioso ?? string.Empty;
            HayDatoCurioso = !string.IsNullOrWhiteSpace(guia.DatoCurioso);

            Pasos.Clear();
            var numero = 1;
            foreach (var paso in guia.Pasos)
                Pasos.Add(new PasoVm(numero++, paso));

            Errores.Clear();
            foreach (var error in guia.ErroresComunes)
                Errores.Add(error);
            HayErrores = Errores.Count > 0;
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
