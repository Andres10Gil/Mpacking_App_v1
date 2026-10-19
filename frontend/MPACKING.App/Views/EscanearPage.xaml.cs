using MPACKING.App.ViewModels;

namespace MPACKING.App.Views;

public partial class EscanearPage : ContentPage
{
    public EscanearPage(EscanearViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
