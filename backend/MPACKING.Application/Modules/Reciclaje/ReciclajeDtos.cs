namespace MPACKING.Application.Modules.Reciclaje;

public record EscanearQrRequest(string CodigoHash);

public record QrInfoResponse(int IdQr, string Material, string Categoria,
                             decimal PesoKg, int EcopesosEstimados,
                             decimal Co2Evitado, string Restaurante,
                             DateTime FechaExpiracion, bool EsCanjeable);

public record ConfirmarReciclajeRequest(string CodigoHash, int IdReciclador,
                                        decimal PesoConfirmadoKg);

public record ReciclajeResponse(int IdTransaccion, int EcopesosGanados,
                                int SaldoActual, decimal Co2Evitado,
                                string Material, decimal PesoKg);

public record MovimientoResponse(int Id, string Descripcion, decimal PesoKg,
                                 int Ecopesos, DateTime Fecha);

public record BilleteraResponse(int SaldoEcopesos, decimal EquivalenteCop,
                                decimal TotalKgReciclados,
                                IReadOnlyList<MovimientoResponse> Movimientos);
