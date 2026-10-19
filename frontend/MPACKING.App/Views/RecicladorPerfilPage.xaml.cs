using MPACKING.App.ViewModels;

namespace MPACKING.App.Views;

public partial class RecicladorPerfilPage : ContentPage
{
    private readonly RecicladorPerfilViewModel _viewModel;

    public RecicladorPerfilPage(RecicladorPerfilViewModel viewModel)
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
