namespace MPACKING.App.Models;

public record MaterialResponse(
    int Id,
    string Nombre,
    string Categoria,
    decimal PrecioMinCopKg,
    decimal PrecioMaxCopKg,
    int EcopesosMinKg,
    int EcopesosMaxKg,
    decimal ReduccionCo2Kg);

public record GuiaLimpiezaResponse(
    string Material,
    List<string> Pasos,
    List<string> ErroresComunes,
    string? DatoCurioso,
    string? ImagenUrl);
