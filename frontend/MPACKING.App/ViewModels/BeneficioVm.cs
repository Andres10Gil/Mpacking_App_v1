using CommunityToolkit.Mvvm.ComponentModel;

namespace MPACKING.App.ViewModels;

public partial class BeneficioVm : ObservableObject
{
    public int Id { get; }
    public string Descripcion { get; }
    public int EcopesosCosto { get; }
    public string EcopesosTexto { get; }

    [ObservableProperty]
    private bool estaSeleccionado;

    public BeneficioVm(int id, string descripcion, int ecopesosCosto)
    {
        Id = id;
        Descripcion = descripcion;
        EcopesosCosto = ecopesosCosto;
        EcopesosTexto = $"{ecopesosCosto} ecp";
    }
}
