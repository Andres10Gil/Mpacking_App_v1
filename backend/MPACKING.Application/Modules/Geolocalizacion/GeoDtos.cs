namespace MPACKING.Application.Modules.Geolocalizacion;

public record ReportarPosicionRequest(decimal Latitud, decimal Longitud,
                                      int? PrecisionM, decimal? VelocidadKmh);

public record PosicionResponse(decimal Latitud, decimal Longitud,
                               DateTime Timestamp, bool PrecisionConfiable);

public record RestauranteCercanoDto(int Id, string Nombre, string Direccion,
                                    decimal Latitud, decimal Longitud,
                                    double DistanciaKm);
