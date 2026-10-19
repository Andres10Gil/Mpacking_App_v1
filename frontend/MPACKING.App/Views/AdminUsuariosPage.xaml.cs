using MPACKING.App.ViewModels;

namespace MPACKING.App.Views;

public partial class AdminUsuariosPage : ContentPage
{
    private readonly AdminUsuariosViewModel _viewModel;

    public AdminUsuariosPage(AdminUsuariosViewModel viewModel)
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
