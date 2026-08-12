-- ============================================================
-- MPACKING - Indices de rendimiento
-- Motor: PostgreSQL 17
--
-- ORDEN DE EJECUCION: 3 de 5
--
-- Garantizan tiempos de respuesta menores a 3 segundos en las
-- consultas mas frecuentes de la app movil.
-- ============================================================

-- Busqueda de restaurantes cercanos por coordenadas GPS
CREATE INDEX idx_rest_coords    ON RESTAURANTE(latitud, longitud);

-- Filtrado de QR por estado sin escanear toda la tabla
CREATE INDEX idx_qr_estado      ON QR(estado);

-- Historial de transacciones de un usuario ordenado por fecha
CREATE INDEX idx_tx_usuario     ON TRANSACCION(id_usuario, fecha);

-- Redenciones de un usuario filtradas por estado
CREATE INDEX idx_red_usuario    ON REDENCION(id_usuario, estado);

-- Ultima posicion GPS del reciclador en tiempo real
CREATE INDEX idx_ubic_recicl    ON UBICACION_RECICLADOR(id_reciclador, timestamp);

-- Beneficios activos de un restaurante
CREATE INDEX idx_benef_rest     ON BENEFICIO(id_restaurante, activo);

-- Auditoria del historial de precios por material
CREATE INDEX idx_histprecio_mat ON HISTORIAL_PRECIO(id_material, fecha_inicio);

-- Login por correo electronico
CREATE INDEX idx_usuario_correo ON USUARIO(correo);

-- Decodificacion del QR escaneado (operacion critica)
CREATE INDEX idx_qr_hash        ON QR(codigo_hash);

-- Validacion del codigo de redencion por el restaurante
CREATE INDEX idx_redencion_cod  ON REDENCION(codigo_unico);
