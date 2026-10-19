using MPACKING.App.ViewModels;

namespace MPACKING.App.Views;

public partial class RecicladorConfirmarPage : ContentPage
{
    private readonly RecicladorConfirmarViewModel _viewModel;

    public RecicladorConfirmarPage(RecicladorConfirmarViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
}
