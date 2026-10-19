using MPACKING.App.ViewModels;

namespace MPACKING.App.Views;

public partial class RestaurantePerfilPage : ContentPage
{
    private readonly RestaurantePerfilViewModel _viewModel;

    public RestaurantePerfilPage(RestaurantePerfilViewModel viewModel)
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
