-- ============================================================
-- MPACKING - Pruebas funcionales de los triggers
-- Motor: PostgreSQL 17
--
-- ORDEN DE EJECUCION: 5 de 5
--
-- IMPORTANTE: ejecutar cada bloque por separado con F5 y
-- capturar el resultado como evidencia del avance.
--
-- NOTA: los id_material inician en 9 si se eliminaron los
-- materiales de prueba previos. Verificar con:
--   SELECT id_material, nombre FROM MATERIAL ORDER BY id_material;
-- ============================================================


-- ============================================================
-- BLOQUE 1 - Crear usuario de prueba
-- ============================================================
INSERT INTO USUARIO (nombre, correo, password_hash, rol)
VALUES ('Carlos Perez', 'carlos.perez@gmail.com', '$2b$12$hash_prueba', 'USUARIO');

SELECT nombre, ecopesos_total
FROM USUARIO
WHERE correo = 'carlos.perez@gmail.com';
-- RESULTADO ESPERADO: ecopesos_total = 0


-- ============================================================
-- BLOQUE 2 - Crear un QR valido
-- ============================================================
INSERT INTO QR (id_restaurante, id_material, codigo_hash, peso_kg, fecha_expiracion)
VALUES (1, 9, 'QR-CARLOS-001', 2.5, NOW() + INTERVAL '30 days');

SELECT id_qr, codigo_hash, estado, peso_kg
FROM QR
WHERE codigo_hash = 'QR-CARLOS-001';
-- RESULTADO ESPERADO: estado = ACTIVO


-- ============================================================
-- BLOQUE 3 - Registrar la transaccion
-- Dispara T4 (valida), T1 (suma ecopesos) y T3 (marca QR usado)
-- ============================================================
INSERT INTO TRANSACCION (id_usuario, id_qr, id_reciclador,
                         ecopesos_ganados, peso_kg, precio_cop_kg)
VALUES (
  (SELECT id_usuario FROM USUARIO WHERE correo = 'carlos.perez@gmail.com'),
  (SELECT id_qr FROM QR WHERE codigo_hash = 'QR-CARLOS-001'),
  1, 225, 2.5, 900.00
);
-- RESULTADO ESPERADO: INSERT 0 1 · Query returned successfully


-- ============================================================
-- BLOQUE 4 - Verificar T1 (acreditacion automatica)
-- ============================================================
SELECT nombre, ecopesos_total
FROM USUARIO
WHERE correo = 'carlos.perez@gmail.com';
-- RESULTADO ESPERADO: ecopesos_total = 225  [T1 VERIFICADO]


-- ============================================================
-- BLOQUE 5 - Verificar T3 (QR marcado como usado)
-- ============================================================
SELECT codigo_hash, estado
FROM QR
WHERE codigo_hash = 'QR-CARLOS-001';
-- RESULTADO ESPERADO: estado = USADO  [T3 VERIFICADO]


-- ============================================================
-- BLOQUE 6 - Verificar T4 con QR ya usado
-- Debe LANZAR ERROR. El error es el resultado correcto.
-- ============================================================
INSERT INTO TRANSACCION (id_usuario, id_qr, id_reciclador,
                         ecopesos_ganados, peso_kg, precio_cop_kg)
VALUES (
  (SELECT id_usuario FROM USUARIO WHERE correo = 'carlos.perez@gmail.com'),
  (SELECT id_qr FROM QR WHERE codigo_hash = 'QR-CARLOS-001'),
  1, 225, 2.5, 900.00
);
-- RESULTADO ESPERADO:
--   ERROR: QR no disponible, estado: USADO
--   SQL state: P0001                        [T4 VERIFICADO]


-- ============================================================
-- BLOQUE 7 - Verificar T4 con QR expirado
-- Debe LANZAR ERROR. El error es el resultado correcto.
-- ============================================================
INSERT INTO QR (id_restaurante, id_material, codigo_hash, peso_kg, fecha_expiracion)
VALUES (1, 9, 'QR-EXPIRADO-001', 1.0, NOW() - INTERVAL '2 hours');

INSERT INTO TRANSACCION (id_usuario, id_qr, id_reciclador,
                         ecopesos_ganados, peso_kg, precio_cop_kg)
VALUES (
  (SELECT id_usuario FROM USUARIO WHERE correo = 'carlos.perez@gmail.com'),
  (SELECT id_qr FROM QR WHERE codigo_hash = 'QR-EXPIRADO-001'),
  1, 90, 1.0, 900.00
);
-- RESULTADO ESPERADO:
--   ERROR: QR expirado (id_qr: X)           [T4 VERIFICADO]


-- ============================================================
-- BLOQUE 8 - Verificar T2 (descuento y reintegro)
-- ============================================================
-- 8.1 Crear la redencion en estado PENDIENTE
INSERT INTO REDENCION (id_usuario, id_beneficio, codigo_unico, ecopesos_usados)
VALUES (
  (SELECT id_usuario FROM USUARIO WHERE correo = 'carlos.perez@gmail.com'),
  1, 'RED-CARLOS-001', 100
);

-- 8.2 Aprobar: T2 debe descontar 100 ecopesos
UPDATE REDENCION SET estado = 'APROBADA'
WHERE codigo_unico = 'RED-CARLOS-001';

SELECT nombre, ecopesos_total FROM USUARIO
WHERE correo = 'carlos.perez@gmail.com';
-- RESULTADO ESPERADO: 125  (225 - 100)

-- 8.3 Devolver: T2 debe reintegrar los 100 ecopesos
UPDATE REDENCION SET estado = 'DEVUELTA'
WHERE codigo_unico = 'RED-CARLOS-001';

SELECT nombre, ecopesos_total FROM USUARIO
WHERE correo = 'carlos.perez@gmail.com';
-- RESULTADO ESPERADO: 225                   [T2 VERIFICADO]


-- ============================================================
-- BLOQUE 9 - Evidencias para el informe de avance
-- ============================================================
-- 9.1 Listado de los 4 triggers activos
SELECT trigger_name, event_manipulation, event_object_table
FROM information_schema.triggers
WHERE trigger_schema = 'public'
ORDER BY trigger_name;

-- 9.2 Conteo de registros por tabla
SELECT 'materiales'   AS tabla, COUNT(*) AS total FROM MATERIAL
UNION ALL SELECT 'restaurantes', COUNT(*) FROM RESTAURANTE
UNION ALL SELECT 'recicladores', COUNT(*) FROM RECICLADOR
UNION ALL SELECT 'beneficios',   COUNT(*) FROM BENEFICIO
UNION ALL SELECT 'usuarios',     COUNT(*) FROM USUARIO;

-- 9.3 Listado de las 12 tablas creadas
SELECT table_name
FROM information_schema.tables
WHERE table_schema = 'public'
ORDER BY table_name;


-- ============================================================
-- BLOQUE 10 - Limpieza de datos de prueba (opcional)
-- ============================================================
-- DELETE FROM REDENCION WHERE codigo_unico = 'RED-CARLOS-001';
-- DELETE FROM TRANSACCION WHERE id_usuario =
--   (SELECT id_usuario FROM USUARIO WHERE correo = 'carlos.perez@gmail.com');
-- DELETE FROM QR WHERE codigo_hash IN ('QR-CARLOS-001','QR-EXPIRADO-001');
-- DELETE FROM USUARIO WHERE correo = 'carlos.perez@gmail.com';
