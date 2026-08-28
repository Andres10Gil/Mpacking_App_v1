namespace MPACKING.Application.Modules.Catalogo;

public record MaterialResponse(int Id, string Nombre, string Categoria,
                               decimal PrecioMinCopKg, decimal PrecioMaxCopKg,
                               int EcopesosMinKg, int EcopesosMaxKg,
                               decimal ReduccionCo2Kg);

public record GuiaLimpiezaResponse(string Material, IReadOnlyList<string> Pasos,
                                   IReadOnlyList<string> ErroresComunes,
                                   string? DatoCurioso, string? ImagenUrl);
