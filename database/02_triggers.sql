-- ============================================================
-- MPACKING - Triggers automaticos
-- Motor: PostgreSQL 17
--
-- ORDEN DE EJECUCION: 2 de 5
--
-- Los triggers funcionan como red de seguridad: garantizan la
-- integridad del sistema aunque el backend tenga un error.
-- ============================================================

-- ------------------------------------------------------------
-- T1: Acredita ecopesos al usuario al registrar un reciclaje
-- ------------------------------------------------------------
CREATE OR REPLACE FUNCTION fn_sumar_ecopesos()
RETURNS TRIGGER AS $$
BEGIN
  UPDATE USUARIO
  SET ecopesos_total = ecopesos_total + NEW.ecopesos_ganados
  WHERE id_usuario = NEW.id_usuario;
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER T1_sumar_ecopesos
AFTER INSERT ON TRANSACCION
FOR EACH ROW EXECUTE FUNCTION fn_sumar_ecopesos();


-- ------------------------------------------------------------
-- T2: Descuenta ecopesos al aprobar una redencion
--     y los reintegra si la redencion es devuelta
-- ------------------------------------------------------------
CREATE OR REPLACE FUNCTION fn_gestionar_ecopesos_redencion()
RETURNS TRIGGER AS $$
BEGIN
  IF NEW.estado = 'APROBADA' AND OLD.estado = 'PENDIENTE' THEN
    UPDATE USUARIO
    SET ecopesos_total = ecopesos_total - NEW.ecopesos_usados
    WHERE id_usuario = NEW.id_usuario;
  END IF;

  IF NEW.estado = 'DEVUELTA' AND OLD.estado = 'APROBADA' THEN
    UPDATE USUARIO
    SET ecopesos_total = ecopesos_total + NEW.ecopesos_usados
    WHERE id_usuario = NEW.id_usuario;
  END IF;

  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER T2_gestionar_redencion
AFTER UPDATE ON REDENCION
FOR EACH ROW EXECUTE FUNCTION fn_gestionar_ecopesos_redencion();


-- ------------------------------------------------------------
-- T3: Marca el QR como USADO para prevenir doble escaneo
-- ------------------------------------------------------------
CREATE OR REPLACE FUNCTION fn_marcar_qr_usado()
RETURNS TRIGGER AS $$
BEGIN
  UPDATE QR SET estado = 'USADO'
  WHERE id_qr = NEW.id_qr;
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER T3_marcar_qr_usado
AFTER INSERT ON TRANSACCION
FOR EACH ROW EXECUTE FUNCTION fn_marcar_qr_usado();


-- ------------------------------------------------------------
-- T4: Valida que el QR este activo y no expirado.
--     Bloquea la insercion lanzando una excepcion.
-- ------------------------------------------------------------
CREATE OR REPLACE FUNCTION fn_validar_qr()
RETURNS TRIGGER AS $$
DECLARE
  v_estado VARCHAR(20);
  v_expira TIMESTAMP;
BEGIN
  SELECT estado, fecha_expiracion
  INTO v_estado, v_expira
  FROM QR WHERE id_qr = NEW.id_qr;

  IF v_expira < NOW() THEN
    RAISE EXCEPTION 'QR expirado (id_qr: %)', NEW.id_qr;
  END IF;

  IF v_estado NOT IN ('ACTIVO','PENDIENTE') THEN
    RAISE EXCEPTION 'QR no disponible, estado: %', v_estado;
  END IF;

  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER T4_validar_qr
BEFORE INSERT ON TRANSACCION
FOR EACH ROW EXECUTE FUNCTION fn_validar_qr();
