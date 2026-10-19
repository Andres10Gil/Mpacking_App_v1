using System.Windows.Input;

namespace MPACKING.App.Views.Componentes;

public partial class BarraInferiorReciclador : ContentView
{
    public static readonly BindableProperty TabActivaProperty =
        BindableProperty.Create(nameof(TabActiva), typeof(string), typeof(BarraInferiorReciclador), string.Empty);

    public static readonly BindableProperty ComandoRutaProperty =
        BindableProperty.Create(nameof(ComandoRuta), typeof(ICommand), typeof(BarraInferiorReciclador));

    public static readonly BindableProperty ComandoConfirmarProperty =
        BindableProperty.Create(nameof(ComandoConfirmar), typeof(ICommand), typeof(BarraInferiorReciclador));

    public static readonly BindableProperty ComandoHistorialProperty =
        BindableProperty.Create(nameof(ComandoHistorial), typeof(ICommand), typeof(BarraInferiorReciclador));

    public static readonly BindableProperty ComandoPerfilProperty =
        BindableProperty.Create(nameof(ComandoPerfil), typeof(ICommand), typeof(BarraInferiorReciclador));

    public string TabActiva
    {
        get => (string)GetValue(TabActivaProperty);
        set => SetValue(TabActivaProperty, value);
    }

    public ICommand ComandoRuta
    {
        get => (ICommand)GetValue(ComandoRutaProperty);
        set => SetValue(ComandoRutaProperty, value);
    }

    public ICommand ComandoConfirmar
    {
        get => (ICommand)GetValue(ComandoConfirmarProperty);
        set => SetValue(ComandoConfirmarProperty, value);
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

    public BarraInferiorReciclador()
    {
        InitializeComponent();
    }

    private void OnTapRuta(object? sender, TappedEventArgs e) => ComandoRuta?.Execute(null);
    private void OnTapConfirmar(object? sender, TappedEventArgs e) => ComandoConfirmar?.Execute(null);
    private void OnTapHistorial(object? sender, TappedEventArgs e) => ComandoHistorial?.Execute(null);
    private void OnTapPerfil(object? sender, TappedEventArgs e) => ComandoPerfil?.Execute(null);
}
