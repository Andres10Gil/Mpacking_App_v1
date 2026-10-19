using System.Collections.ObjectModel;
using Microsoft.Maui.Graphics;

namespace MPACKING.App.ViewModels;

public record MaterialFilaVm(int Id, string Nombre, string PrecioTexto, string EcopesosTexto);

public class CategoriaMaterialesVm
{
    public string Nombre { get; }
    public Color ColorIcono { get; }
    public ObservableCollection<MaterialFilaVm> Materiales { get; }

    public CategoriaMaterialesVm(string nombre, Color colorIcono, IEnumerable<MaterialFilaVm> materiales)
    {
        Nombre = nombre;
        ColorIcono = colorIcono;
        Materiales = new ObservableCollection<MaterialFilaVm>(materiales);
    }
}
