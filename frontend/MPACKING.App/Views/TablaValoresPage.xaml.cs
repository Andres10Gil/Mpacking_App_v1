using MPACKING.App.ViewModels;

namespace MPACKING.App.Views;

public partial class TablaValoresPage : ContentPage
{
    private readonly TablaValoresViewModel _viewModel;

    public TablaValoresPage(TablaValoresViewModel viewModel)
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
