using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Graphics;
using MPACKING.App.Services;

namespace MPACKING.App.ViewModels;

public partial class TablaValoresViewModel : BaseViewModel
{
    private static readonly CultureInfo CulturaCo = new("es-CO");

    // Colores de la paleta MPACKING, uno por categoria, en orden alfabetico.
    private static readonly Color[] ColoresCategoria =
    {
        Color.FromArgb("#FFE2C7"), // Combinados
        Color.FromArgb("#FFE0E3"), // Especiales
        Color.FromArgb("#FFF3C4"), // Metales
        Color.FromArgb("#E8F6E1"), // Papel
        Color.FromArgb("#D8ECFB"), // Plasticos
        Color.FromArgb("#EBE3FA"), // Vidrio
    };

    private readonly CatalogoApiService _catalogoApi;

    public ObservableCollection<CategoriaMaterialesVm> Categorias { get; } = new();

    public TablaValoresViewModel(CatalogoApiService catalogoApi)
    {
        _catalogoApi = catalogoApi;
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        MensajeError = string.Empty;
        EstaOcupado = true;
        try
        {
            var materiales = await _catalogoApi.ListarMaterialesAsync();

            Categorias.Clear();
            var grupos = materiales
                .GroupBy(m => m.Categoria)
                .OrderBy(g => g.Key);

            int indiceColor = 0;
            foreach (var grupo in grupos)
            {
                var filas = grupo
                    .OrderBy(m => m.Nombre)
                    .Select(m => new MaterialFilaVm(
                        m.Id,
                        m.Nombre,
                        $"{m.PrecioMinCopKg.ToString("C0", CulturaCo)}-{m.PrecioMaxCopKg.ToString("C0", CulturaCo)}",
                        $"{m.EcopesosMinKg}-{m.EcopesosMaxKg} ecp"));

                var color = ColoresCategoria[indiceColor % ColoresCategoria.Length];
                indiceColor++;

                Categorias.Add(new CategoriaMaterialesVm(grupo.Key, color, filas));
            }
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }
        finally
        {
            EstaOcupado = false;
        }
    }

    [RelayCommand]
    private static async Task VerGuiaAsync(MaterialFilaVm fila) =>
        await Shell.Current.GoToAsync($"{nameof(Views.GuiaLimpiezaPage)}?idMaterial={fila.Id}");

    [RelayCommand]
    private static async Task VolverAsync() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private static async Task IrEscanearAsync()
    {
        await Shell.Current.GoToAsync("..");
        await Shell.Current.GoToAsync(nameof(Views.EscanearPage));
    }

    [RelayCommand]
    private static async Task IrMapaAsync()
    {
        await Shell.Current.GoToAsync("..");
        await Shell.Current.GoToAsync(nameof(Views.MapaPage));
    }

    [RelayCommand]
    private static async Task IrBilleteraAsync()
    {
        await Shell.Current.GoToAsync("..");
        await Shell.Current.GoToAsync(nameof(Views.BilleteraPage));
    }
}
