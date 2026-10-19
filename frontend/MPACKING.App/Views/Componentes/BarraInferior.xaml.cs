using System.Windows.Input;

namespace MPACKING.App.Views.Componentes;

public partial class BarraInferior : ContentView
{
    public static readonly BindableProperty TabActivaProperty =
        BindableProperty.Create(nameof(TabActiva), typeof(string), typeof(BarraInferior), string.Empty);

    public static readonly BindableProperty ComandoInicioProperty =
        BindableProperty.Create(nameof(ComandoInicio), typeof(ICommand), typeof(BarraInferior));

    public static readonly BindableProperty ComandoEscanearProperty =
        BindableProperty.Create(nameof(ComandoEscanear), typeof(ICommand), typeof(BarraInferior));

    public static readonly BindableProperty ComandoMapaProperty =
        BindableProperty.Create(nameof(ComandoMapa), typeof(ICommand), typeof(BarraInferior));

    public static readonly BindableProperty ComandoBilleteraProperty =
        BindableProperty.Create(nameof(ComandoBilletera), typeof(ICommand), typeof(BarraInferior));

    public static readonly BindableProperty ComandoPerfilProperty =
        BindableProperty.Create(nameof(ComandoPerfil), typeof(ICommand), typeof(BarraInferior));

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

    public ICommand ComandoEscanear
    {
        get => (ICommand)GetValue(ComandoEscanearProperty);
        set => SetValue(ComandoEscanearProperty, value);
    }

    public ICommand ComandoMapa
    {
        get => (ICommand)GetValue(ComandoMapaProperty);
        set => SetValue(ComandoMapaProperty, value);
    }

    public ICommand ComandoBilletera
    {
        get => (ICommand)GetValue(ComandoBilleteraProperty);
        set => SetValue(ComandoBilleteraProperty, value);
    }

    public ICommand ComandoPerfil
    {
        get => (ICommand)GetValue(ComandoPerfilProperty);
        set => SetValue(ComandoPerfilProperty, value);
    }

    public BarraInferior()
    {
        InitializeComponent();
    }

    private void OnTapInicio(object? sender, TappedEventArgs e) => ComandoInicio?.Execute(null);
    private void OnTapEscanear(object? sender, TappedEventArgs e) => ComandoEscanear?.Execute(null);
    private void OnTapMapa(object? sender, TappedEventArgs e) => ComandoMapa?.Execute(null);
    private void OnTapBilletera(object? sender, TappedEventArgs e) => ComandoBilletera?.Execute(null);
    private void OnTapPerfil(object? sender, TappedEventArgs e) => ComandoPerfil?.Execute(null);
}
