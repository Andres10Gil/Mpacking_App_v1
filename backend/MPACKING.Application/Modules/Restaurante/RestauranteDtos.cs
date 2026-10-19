namespace MPACKING.Application.Modules.Restaurante;

public record ResumenRestauranteResponse(
    string NombreRestaurante,
    string Direccion,
    decimal RecibidoHoyCop,
    int RedencionesHoy,
    int EcopesosValidadosHoy,
    IReadOnlyList<RedencionRecienteResponse> Ultimas);

public record RedencionRecienteResponse(
    string Usuario, string Detalle, string Estado, string MontoTexto);

public record BeneficioRestauranteResponse(
    int Id, string Descripcion, int EcopesosCosto, bool Activo);

public record CrearBeneficioRequest(string Descripcion, int EcopesosCosto);

public record HistorialPagosResponse(
    decimal TotalMesCop, int RedencionesMes,
    IReadOnlyList<PagoHistorialResponse> Pagos);

public record PagoHistorialResponse(
    string Usuario, string Detalle, string MontoTexto, string Estado);
