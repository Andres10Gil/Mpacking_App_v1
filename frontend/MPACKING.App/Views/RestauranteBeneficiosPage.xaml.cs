using MPACKING.App.ViewModels;

namespace MPACKING.App.Views;

public partial class RestauranteBeneficiosPage : ContentPage
{
    private readonly RestauranteBeneficiosViewModel _viewModel;

    public RestauranteBeneficiosPage(RestauranteBeneficiosViewModel viewModel)
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
