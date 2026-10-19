namespace MPACKING.Application.Modules.Admin;

public record ResumenAdminResponse(
    int UsuariosActivos,
    int QrEscaneados,
    int EcopesosDados,
    decimal MaterialKgTotal,
    decimal Co2EvitadoMes,
    IReadOnlyList<AlertaResponse> Alertas);

public record AlertaResponse(string Titulo, string Detalle, string Etiqueta);

public record UsuarioAdminResponse(
    int Id, string Nombre, string Correo, string Rol,
    string Estado, int EcopesosTotal);

public record ReporteAmbientalResponse(
    decimal Co2Total,
    int ArbolesEquivalentes,
    IReadOnlyList<DistribucionMaterialResponse> Distribucion,
    IReadOnlyList<TotalMaterialResponse> Totales);

public record DistribucionMaterialResponse(string Material, decimal Porcentaje);

public record TotalMaterialResponse(string Material, decimal KgTotal, int EcopesosTotal);

/// <summary>Dato crudo que arma el repositorio al agrupar transacciones por material.</summary>
public record MaterialTotalDato(string Material, decimal KgTotal, int EcopesosTotal, decimal Co2Total);
