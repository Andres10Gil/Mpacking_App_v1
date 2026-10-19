namespace MPACKING.App.Models;

public record RestauranteCercanoDto(
    int Id,
    string Nombre,
    string Direccion,
    decimal Latitud,
    decimal Longitud,
    double DistanciaKm);

public record PosicionResponse(
    decimal Latitud,
    decimal Longitud,
    DateTime Timestamp,
    bool PrecisionConfiable);
