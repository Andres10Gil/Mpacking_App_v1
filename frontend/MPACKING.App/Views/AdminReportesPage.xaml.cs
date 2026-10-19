using MPACKING.App.ViewModels;

namespace MPACKING.App.Views;

public partial class AdminReportesPage : ContentPage
{
    private readonly AdminReportesViewModel _viewModel;

    public AdminReportesPage(AdminReportesViewModel viewModel)
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
