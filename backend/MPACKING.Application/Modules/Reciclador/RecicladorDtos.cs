namespace MPACKING.Application.Modules.Reciclador;

public record PerfilRecicladorResponse(
    int IdReciclador, string Nombre, string? Zona,
    decimal KgHoy, decimal KgMes, bool EnRuta,
    decimal? Latitud, decimal? Longitud);

public record MovimientoRecicladorResponse(
    string Usuario, string Material, decimal PesoKg, int Ecopesos, DateTime Fecha);

public record HistorialRecicladorResponse(
    decimal KgMes, int RecoleccionesMes,
    IReadOnlyList<MovimientoRecicladorResponse> Movimientos);

/// <summary>
/// El reciclador identifica al cliente por su correo porque es el, y no el
/// cliente, quien confirma la recoleccion desde su propia cuenta.
/// </summary>
public record ConfirmarRecoleccionRequest(
    string CodigoHash, string CorreoCliente, decimal PesoConfirmadoKg);
