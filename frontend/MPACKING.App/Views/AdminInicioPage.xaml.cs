using MPACKING.App.ViewModels;

namespace MPACKING.App.Views;

public partial class AdminInicioPage : ContentPage
{
    private readonly AdminInicioViewModel _viewModel;

    public AdminInicioPage(AdminInicioViewModel viewModel)
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
