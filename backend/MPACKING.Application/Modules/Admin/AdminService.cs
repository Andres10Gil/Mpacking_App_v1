using MPACKING.Application.Common.Interfaces;

namespace MPACKING.Application.Modules.Admin;

/// <summary>
/// Orquesta las consultas agregadas del panel de administración.
/// Solo lectura: no toca ningún trigger ni modifica datos.
/// </summary>
public class AdminService : IAdminService
{
    /// <summary>Un árbol adulto absorbe en promedio 21 kg de CO2 al año.</summary>
    private const decimal KgCo2PorArbol = 21m;

    private readonly IAdminRepository _admin;
    private readonly IRelojSistema _reloj;

    public AdminService(IAdminRepository admin, IRelojSistema reloj)
    {
        _admin = admin;
        _reloj = reloj;
    }

    public async Task<ResumenAdminResponse> ObtenerResumenAsync(CancellationToken ct = default)
    {
        var usuariosActivos = await _admin.ContarUsuariosActivosAsync(ct);
        var qrEscaneados = await _admin.ContarTransaccionesAsync(ct);
        var ecopesosDados = await _admin.SumarEcopesosDadosAsync(ct);
        var materialKg = await _admin.SumarKgRecicladosAsync(ct);
        var co2Mes = await _admin.SumarKgRecicladosEnMesAsync(
            _reloj.Ahora.Year, _reloj.Ahora.Month, ct);

        var pendientes = await _admin.ListarRedencionesPendientesAsync(4, ct);
        var alertas = pendientes
            .Select(r => new AlertaResponse(
                r.Usuario.Nombre,
                $"{r.Beneficio.Descripcion} · {r.EcopesosUsados} ecopesos",
                "REVISAR"))
            .ToList();

        return new ResumenAdminResponse(
            usuariosActivos, qrEscaneados, ecopesosDados, materialKg, co2Mes, alertas);
    }

    public async Task<IReadOnlyList<UsuarioAdminResponse>> ListarUsuariosAsync(
        string? busqueda, int pagina, int tamano, CancellationToken ct = default)
    {
        var usuarios = await _admin.ListarUsuariosAsync(busqueda, pagina, tamano, ct);
        return usuarios
            .Select(u => new UsuarioAdminResponse(
                u.IdUsuario, u.Nombre, u.Correo, u.Rol.ToString(),
                u.Activo ? "Activo" : "Bloqueado", u.EcopesosTotal))
            .ToList();
    }

    public async Task<ReporteAmbientalResponse> ObtenerReporteAsync(CancellationToken ct = default)
    {
        var totales = await _admin.TotalesPorMaterialAsync(ct);
        var co2Total = totales.Sum(t => t.Co2Total);
        var kgTotal = totales.Sum(t => t.KgTotal);

        var distribucion = totales
            .Select(t => new DistribucionMaterialResponse(
                t.Material, kgTotal > 0 ? Math.Round(t.KgTotal / kgTotal, 2) : 0))
            .OrderByDescending(d => d.Porcentaje)
            .ToList();

        var materiales = totales
            .Select(t => new TotalMaterialResponse(t.Material, t.KgTotal, t.EcopesosTotal))
            .OrderByDescending(t => t.KgTotal)
            .ToList();

        return new ReporteAmbientalResponse(
            co2Total, (int)(co2Total / KgCo2PorArbol), distribucion, materiales);
    }
}
