using MPACKING.App.ViewModels;

namespace MPACKING.App.Views;

public partial class RedimirPage : ContentPage
{
    private readonly RedimirViewModel _viewModel;

    public RedimirPage(RedimirViewModel viewModel)
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
