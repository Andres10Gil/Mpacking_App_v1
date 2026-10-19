using CommunityToolkit.Mvvm.ComponentModel;

namespace MPACKING.App.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool estaOcupado;

    [ObservableProperty]
    private string mensajeError = string.Empty;
}
