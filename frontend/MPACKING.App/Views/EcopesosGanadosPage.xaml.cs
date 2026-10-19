using MPACKING.App.ViewModels;

namespace MPACKING.App.Views;

public partial class EcopesosGanadosPage : ContentPage
{
    public EcopesosGanadosPage(EcopesosGanadosViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
