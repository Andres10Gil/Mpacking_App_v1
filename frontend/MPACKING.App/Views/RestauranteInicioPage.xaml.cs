using MPACKING.App.ViewModels;

namespace MPACKING.App.Views;

public partial class RestauranteInicioPage : ContentPage
{
    private readonly RestauranteInicioViewModel _viewModel;

    public RestauranteInicioPage(RestauranteInicioViewModel viewModel)
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
