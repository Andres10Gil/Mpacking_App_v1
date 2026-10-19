namespace MPACKING.App.ViewModels;

public partial class BeneficioRestauranteVm : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
{
    public int Id { get; }
    public string Descripcion { get; }
    public string CostoTexto { get; }

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private bool activo;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private string estadoTexto;

    public BeneficioRestauranteVm(int id, string descripcion, string costoTexto, bool activo)
    {
        Id = id;
        Descripcion = descripcion;
        CostoTexto = costoTexto;
        this.activo = activo;
        estadoTexto = activo ? "Activo" : "Pausado";
    }
}
