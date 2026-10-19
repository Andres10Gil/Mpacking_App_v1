using System.Windows.Input;

namespace MPACKING.App.Views.Componentes;

public partial class BarraInferiorRestaurante : ContentView
{
    public static readonly BindableProperty TabActivaProperty =
        BindableProperty.Create(nameof(TabActiva), typeof(string), typeof(BarraInferiorRestaurante), string.Empty);

    public static readonly BindableProperty ComandoInicioProperty =
        BindableProperty.Create(nameof(ComandoInicio), typeof(ICommand), typeof(BarraInferiorRestaurante));

    public static readonly BindableProperty ComandoBeneficiosProperty =
        BindableProperty.Create(nameof(ComandoBeneficios), typeof(ICommand), typeof(BarraInferiorRestaurante));

    public static readonly BindableProperty ComandoHistorialProperty =
        BindableProperty.Create(nameof(ComandoHistorial), typeof(ICommand), typeof(BarraInferiorRestaurante));

    public static readonly BindableProperty ComandoPerfilProperty =
        BindableProperty.Create(nameof(ComandoPerfil), typeof(ICommand), typeof(BarraInferiorRestaurante));

    public string TabActiva
    {
        get => (string)GetValue(TabActivaProperty);
        set => SetValue(TabActivaProperty, value);
    }

    public ICommand ComandoInicio
    {
        get => (ICommand)GetValue(ComandoInicioProperty);
        set => SetValue(ComandoInicioProperty, value);
    }

    public ICommand ComandoBeneficios
    {
        get => (ICommand)GetValue(ComandoBeneficiosProperty);
        set => SetValue(ComandoBeneficiosProperty, value);
    }

    public ICommand ComandoHistorial
    {
        get => (ICommand)GetValue(ComandoHistorialProperty);
        set => SetValue(ComandoHistorialProperty, value);
    }

    public ICommand ComandoPerfil
    {
        get => (ICommand)GetValue(ComandoPerfilProperty);
        set => SetValue(ComandoPerfilProperty, value);
    }

    public BarraInferiorRestaurante()
    {
        InitializeComponent();
    }

    private void OnTapInicio(object? sender, TappedEventArgs e) => ComandoInicio?.Execute(null);
    private void OnTapBeneficios(object? sender, TappedEventArgs e) => ComandoBeneficios?.Execute(null);
    private void OnTapHistorial(object? sender, TappedEventArgs e) => ComandoHistorial?.Execute(null);
    private void OnTapPerfil(object? sender, TappedEventArgs e) => ComandoPerfil?.Execute(null);
}
