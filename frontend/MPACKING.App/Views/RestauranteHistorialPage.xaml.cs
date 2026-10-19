using MPACKING.App.ViewModels;

namespace MPACKING.App.Views;

public partial class RestauranteHistorialPage : ContentPage
{
    private readonly RestauranteHistorialViewModel _viewModel;

    public RestauranteHistorialPage(RestauranteHistorialViewModel viewModel)
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
