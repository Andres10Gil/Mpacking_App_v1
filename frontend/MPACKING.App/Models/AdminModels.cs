namespace MPACKING.App.Models;

public record ResumenAdminResponse(
    int UsuariosActivos, int QrEscaneados, int EcopesosDados,
    decimal MaterialKgTotal, decimal Co2EvitadoMes,
    List<AlertaResponse> Alertas);

public record AlertaResponse(string Titulo, string Detalle, string Etiqueta);

public record UsuarioAdminResponse(
    int Id, string Nombre, string Correo, string Rol,
    string Estado, int EcopesosTotal);

public record ReporteAmbientalResponse(
    decimal Co2Total, int ArbolesEquivalentes,
    List<DistribucionMaterialResponse> Distribucion,
    List<TotalMaterialResponse> Totales);

public record DistribucionMaterialResponse(string Material, decimal Porcentaje);

public record TotalMaterialResponse(string Material, decimal KgTotal, int EcopesosTotal);
