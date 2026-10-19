using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MPACKING.App.ViewModels;

[QueryProperty(nameof(Ecopesos), "ecopesos")]
[QueryProperty(nameof(Material), "material")]
[QueryProperty(nameof(PesoTexto), "peso")]
[QueryProperty(nameof(Co2Texto), "co2")]
public partial class EcopesosGanadosViewModel : ObservableObject
{
    private static readonly CultureInfo CulturaCo = new("es-CO");

    [ObservableProperty]
    private int ecopesos;

    [ObservableProperty]
    private string material = string.Empty;

    [ObservableProperty]
    private string pesoTexto = string.Empty;

    [ObservableProperty]
    private string co2Texto = string.Empty;

    [RelayCommand]
    private static async Task IrABilleteraAsync()
    {
        await Shell.Current.GoToAsync("..");
        await Shell.Current.GoToAsync(nameof(Views.BilleteraPage));
    }

    [RelayCommand]
    private static async Task IrAInicioAsync() => await Shell.Current.GoToAsync("..");
}
