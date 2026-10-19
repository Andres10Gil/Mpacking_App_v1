using MPACKING.App.ViewModels;

namespace MPACKING.App.Views;

public partial class RegistroPage : ContentPage
{
    public RegistroPage(RegistroViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
