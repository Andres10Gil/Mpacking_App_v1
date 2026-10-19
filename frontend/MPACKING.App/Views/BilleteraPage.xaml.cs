using MPACKING.App.ViewModels;

namespace MPACKING.App.Views;

public partial class BilleteraPage : ContentPage
{
    private readonly BilleteraViewModel _viewModel;

    public BilleteraPage(BilleteraViewModel viewModel)
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
