namespace MPACKING.Application.Modules.Redencion;

public record BeneficioResponse(int Id, string Descripcion, int EcopesosCosto,
                                bool Disponible);

public record SolicitarRedencionRequest(int IdBeneficio);

public record RedencionResponse(int Id, string CodigoUnico, string Beneficio,
                                string Restaurante, int EcopesosUsados,
                                decimal EquivalenteCop, string Estado,
                                int SaldoRestante);
