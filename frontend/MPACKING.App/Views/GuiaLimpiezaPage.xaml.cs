using MPACKING.App.ViewModels;

namespace MPACKING.App.Views;

public partial class GuiaLimpiezaPage : ContentPage
{
    private readonly GuiaLimpiezaViewModel _viewModel;

    public GuiaLimpiezaPage(GuiaLimpiezaViewModel viewModel)
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
