namespace MPACKING.App.Models;

public record PerfilRecicladorResponse(
    int IdReciclador, string Nombre, string? Zona,
    decimal KgHoy, decimal KgMes, bool EnRuta,
    decimal? Latitud, decimal? Longitud);

public record MovimientoRecicladorResponse(
    string Usuario, string Material, decimal PesoKg, int Ecopesos, DateTime Fecha);

public record HistorialRecicladorResponse(
    decimal KgMes, int RecoleccionesMes, List<MovimientoRecicladorResponse> Movimientos);
