using MPACKING.App.ViewModels;

namespace MPACKING.App.Views;

public partial class RecicladorRutaPage : ContentPage
{
    private readonly RecicladorRutaViewModel _viewModel;

    public RecicladorRutaPage(RecicladorRutaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.CargarCommand.Execute(null);
    }
}
