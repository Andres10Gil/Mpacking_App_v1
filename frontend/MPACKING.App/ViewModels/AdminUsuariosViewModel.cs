using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPACKING.App.Services;

namespace MPACKING.App.ViewModels;

public partial class AdminUsuariosViewModel : BaseViewModel
{
    private static readonly CultureInfo CulturaCo = new("es-CO");
    private static readonly Color ColorActivo = Color.FromArgb("#E8F6E1");
    private static readonly Color ColorBloqueado = Color.FromArgb("#FFE0E3");

    private readonly AdminApiService _adminApi;

    [ObservableProperty]
    private string textoBusqueda = string.Empty;

    public ObservableCollection<UsuarioAdminVm> Usuarios { get; } = new();

    public AdminUsuariosViewModel(AdminApiService adminApi)
    {
        _adminApi = adminApi;
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        MensajeError = string.Empty;
        EstaOcupado = true;
        try
        {
            var usuarios = await _adminApi.ListarUsuariosAsync(TextoBusqueda);

            Usuarios.Clear();
            foreach (var u in usuarios)
            {
                var valorTexto = u.Rol == "Usuario"
                    ? $"{u.EcopesosTotal.ToString("N0", CulturaCo)} ecp"
                    : "—";
                var color = u.Estado == "Activo" ? ColorActivo : ColorBloqueado;
                Usuarios.Add(new UsuarioAdminVm(
                    u.Nombre, $"{u.Rol} · {u.Estado}", valorTexto, color));
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
    private static async Task VolverAsync() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private static async Task IrAReportesAsync()
    {
        await Shell.Current.GoToAsync("..");
        await Shell.Current.GoToAsync(nameof(Views.AdminReportesPage));
    }

    [RelayCommand]
    private static async Task IrAConfigAsync() =>
        await Shell.Current.DisplayAlertAsync("Configuracion", "Esta seccion estara disponible pronto.", "Listo");
}
