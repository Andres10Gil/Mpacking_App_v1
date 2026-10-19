using System.Windows.Input;

namespace MPACKING.App.Views.Componentes;

public partial class BarraInferiorAdmin : ContentView
{
    public static readonly BindableProperty TabActivaProperty =
        BindableProperty.Create(nameof(TabActiva), typeof(string), typeof(BarraInferiorAdmin), string.Empty);

    public static readonly BindableProperty ComandoInicioProperty =
        BindableProperty.Create(nameof(ComandoInicio), typeof(ICommand), typeof(BarraInferiorAdmin));

    public static readonly BindableProperty ComandoUsuariosProperty =
        BindableProperty.Create(nameof(ComandoUsuarios), typeof(ICommand), typeof(BarraInferiorAdmin));

    public static readonly BindableProperty ComandoReportesProperty =
        BindableProperty.Create(nameof(ComandoReportes), typeof(ICommand), typeof(BarraInferiorAdmin));

    public static readonly BindableProperty ComandoConfigProperty =
        BindableProperty.Create(nameof(ComandoConfig), typeof(ICommand), typeof(BarraInferiorAdmin));

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

    public ICommand ComandoUsuarios
    {
        get => (ICommand)GetValue(ComandoUsuariosProperty);
        set => SetValue(ComandoUsuariosProperty, value);
    }

    public ICommand ComandoReportes
    {
        get => (ICommand)GetValue(ComandoReportesProperty);
        set => SetValue(ComandoReportesProperty, value);
    }

    public ICommand ComandoConfig
    {
        get => (ICommand)GetValue(ComandoConfigProperty);
        set => SetValue(ComandoConfigProperty, value);
    }

    public BarraInferiorAdmin()
    {
        InitializeComponent();
    }

    private void OnTapInicio(object? sender, TappedEventArgs e) => ComandoInicio?.Execute(null);
    private void OnTapUsuarios(object? sender, TappedEventArgs e) => ComandoUsuarios?.Execute(null);
    private void OnTapReportes(object? sender, TappedEventArgs e) => ComandoReportes?.Execute(null);
    private void OnTapConfig(object? sender, TappedEventArgs e) => ComandoConfig?.Execute(null);
}
