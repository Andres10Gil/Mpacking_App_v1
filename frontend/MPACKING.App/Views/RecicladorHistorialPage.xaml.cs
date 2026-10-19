using MPACKING.App.ViewModels;

namespace MPACKING.App.Views;

public partial class RecicladorHistorialPage : ContentPage
{
    private readonly RecicladorHistorialViewModel _viewModel;

    public RecicladorHistorialPage(RecicladorHistorialViewModel viewModel)
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
