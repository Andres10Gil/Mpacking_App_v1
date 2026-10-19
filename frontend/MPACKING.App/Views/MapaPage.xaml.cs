using MPACKING.App.ViewModels;

namespace MPACKING.App.Views;

public partial class MapaPage : ContentPage
{
    private readonly MapaViewModel _viewModel;

    public MapaPage(MapaViewModel viewModel)
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
