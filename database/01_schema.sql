-- ============================================================
-- MPACKING - Esquema de base de datos
-- Motor: PostgreSQL 17
-- 12 tablas normalizadas en Tercera Forma Normal (3FN)
--
-- ORDEN DE EJECUCION: 1 de 5
-- Ejecutar en pgAdmin sobre la base de datos mpacking_db
-- ============================================================

-- Tabla central de todos los actores del sistema
CREATE TABLE USUARIO (
  id_usuario      SERIAL PRIMARY KEY,
  nombre          VARCHAR(120) NOT NULL,
  correo          VARCHAR(150) NOT NULL UNIQUE,
  password_hash   VARCHAR(255) NOT NULL,
  rol             VARCHAR(20)  NOT NULL
                  CHECK (rol IN ('USUARIO','RESTAURANTE','RECICLADOR','ADMIN')),
  ecopesos_total  INTEGER      NOT NULL DEFAULT 0,
  activo          BOOLEAN      NOT NULL DEFAULT TRUE,
  intentos_login  SMALLINT     NOT NULL DEFAULT 0,
  bloqueado_hasta TIMESTAMP    NULL,
  fecha_registro  TIMESTAMP    NOT NULL DEFAULT NOW()
);

-- Perfil extendido del reciclador
CREATE TABLE RECICLADOR (
  id_reciclador SERIAL PRIMARY KEY,
  id_usuario    INTEGER NOT NULL UNIQUE REFERENCES USUARIO(id_usuario),
  nombre        VARCHAR(120) NOT NULL,
  telefono      VARCHAR(20),
  zona_asignada VARCHAR(100),
  activo        BOOLEAN NOT NULL DEFAULT TRUE
);

-- Perfil extendido del restaurante aliado
CREATE TABLE RESTAURANTE (
  id_restaurante   SERIAL PRIMARY KEY,
  id_usuario       INTEGER NOT NULL UNIQUE REFERENCES USUARIO(id_usuario),
  nombre           VARCHAR(120) NOT NULL,
  direccion        VARCHAR(250) NOT NULL,
  latitud          DECIMAL(10,8),
  longitud         DECIMAL(11,8),
  horario_apertura TIME,
  horario_cierre   TIME,
  dias_atencion    VARCHAR(50),
  activo           BOOLEAN NOT NULL DEFAULT TRUE
);

-- Catalogo de materiales reciclables con precios
CREATE TABLE MATERIAL (
  id_material          SERIAL PRIMARY KEY,
  nombre               VARCHAR(100) NOT NULL,
  categoria            VARCHAR(60)  NOT NULL,
  precio_min_cop_kg    DECIMAL(10,2) NOT NULL,
  precio_max_cop_kg    DECIMAL(10,2) NOT NULL,
  ecopesos_min_kg      INTEGER NOT NULL,
  ecopesos_max_kg      INTEGER NOT NULL,
  reduccion_co2_kg     DECIMAL(6,3) NOT NULL,
  icono_url            VARCHAR(300),
  activo               BOOLEAN NOT NULL DEFAULT TRUE,
  ultima_actualizacion DATE
);

-- Auditoria inmutable de cambios de precio
CREATE TABLE HISTORIAL_PRECIO (
  id_historial  SERIAL PRIMARY KEY,
  id_material   INTEGER NOT NULL REFERENCES MATERIAL(id_material),
  cambiado_por  INTEGER NOT NULL REFERENCES USUARIO(id_usuario),
  precio_cop_kg DECIMAL(10,2) NOT NULL,
  ecopesos_kg   INTEGER NOT NULL,
  fecha_inicio  DATE NOT NULL,
  fecha_fin     DATE NULL
);

-- Ficha educativa de limpieza por material
CREATE TABLE GUIA_LIMPIEZA (
  id_guia         SERIAL PRIMARY KEY,
  id_material     INTEGER NOT NULL UNIQUE REFERENCES MATERIAL(id_material),
  pasos_limpieza  JSONB NOT NULL,
  errores_comunes JSONB NOT NULL,
  dato_curioso    VARCHAR(300),
  imagen_url      VARCHAR(300),
  video_url       VARCHAR(300),
  activo          BOOLEAN NOT NULL DEFAULT TRUE
);

-- Codigo QR de un solo uso impreso en cada empaque
CREATE TABLE QR (
  id_qr            SERIAL PRIMARY KEY,
  id_restaurante   INTEGER NOT NULL REFERENCES RESTAURANTE(id_restaurante),
  id_material      INTEGER NOT NULL REFERENCES MATERIAL(id_material),
  codigo_hash      VARCHAR(255) NOT NULL UNIQUE,
  peso_kg          DECIMAL(8,3) NOT NULL,
  estado           VARCHAR(20) NOT NULL DEFAULT 'ACTIVO'
                   CHECK (estado IN ('ACTIVO','PENDIENTE','USADO','EXPIRADO')),
  fecha_expiracion TIMESTAMP NOT NULL,
  fecha_creacion   TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Registro inmutable de cada reciclaje realizado
CREATE TABLE TRANSACCION (
  id_transaccion   SERIAL PRIMARY KEY,
  id_usuario       INTEGER NOT NULL REFERENCES USUARIO(id_usuario),
  id_qr            INTEGER NOT NULL UNIQUE REFERENCES QR(id_qr),
  id_reciclador    INTEGER NOT NULL REFERENCES RECICLADOR(id_reciclador),
  ecopesos_ganados INTEGER NOT NULL DEFAULT 0,
  peso_kg          DECIMAL(8,3) NOT NULL,
  precio_cop_kg    DECIMAL(10,2) NOT NULL,
  fecha            TIMESTAMP NOT NULL DEFAULT NOW(),
  ip_escaneo       VARCHAR(45)
);

-- Ofertas que el restaurante pone a disposicion
CREATE TABLE BENEFICIO (
  id_beneficio     SERIAL PRIMARY KEY,
  id_restaurante   INTEGER NOT NULL REFERENCES RESTAURANTE(id_restaurante),
  descripcion      VARCHAR(200) NOT NULL,
  ecopesos_costo   INTEGER NOT NULL,
  activo           BOOLEAN NOT NULL DEFAULT TRUE,
  fecha_expiracion DATE NULL
);

-- Canje de ecopesos por un beneficio
CREATE TABLE REDENCION (
  id_redencion     SERIAL PRIMARY KEY,
  id_usuario       INTEGER NOT NULL REFERENCES USUARIO(id_usuario),
  id_beneficio     INTEGER NOT NULL REFERENCES BENEFICIO(id_beneficio),
  codigo_unico     VARCHAR(100) NOT NULL UNIQUE,
  ecopesos_usados  INTEGER NOT NULL DEFAULT 0,
  estado           VARCHAR(20) NOT NULL DEFAULT 'PENDIENTE'
                   CHECK (estado IN ('PENDIENTE','APROBADA','RECHAZADA','DEVUELTA')),
  fecha_solicitud  TIMESTAMP NOT NULL DEFAULT NOW(),
  fecha_validacion TIMESTAMP NULL
);

-- Pago automatico al restaurante via pasarela Wompi
CREATE TABLE PAGO (
  id_pago           SERIAL PRIMARY KEY,
  id_redencion      INTEGER NOT NULL UNIQUE REFERENCES REDENCION(id_redencion),
  monto_cop         DECIMAL(12,2) NOT NULL,
  comision_mpacking DECIMAL(12,2) NOT NULL,
  comision_pasarela DECIMAL(12,2) NOT NULL,
  monto_neto        DECIMAL(12,2) NOT NULL,
  wompi_tx_id       VARCHAR(100),
  estado            VARCHAR(20) NOT NULL DEFAULT 'PENDIENTE'
                    CHECK (estado IN ('PENDIENTE','APROBADO','RECHAZADO','DEVUELTO')),
  fecha_pago        TIMESTAMP NOT NULL DEFAULT NOW(),
  fecha_liquidacion DATE NULL
);

-- Historico de posiciones GPS del reciclador
CREATE TABLE UBICACION_RECICLADOR (
  id_ubicacion  BIGSERIAL PRIMARY KEY,
  id_reciclador INTEGER NOT NULL REFERENCES RECICLADOR(id_reciclador),
  latitud       DECIMAL(10,8) NOT NULL,
  longitud      DECIMAL(11,8) NOT NULL,
  precision_m   INTEGER NULL,
  velocidad_kmh DECIMAL(5,2) NULL,
  timestamp     TIMESTAMP NOT NULL DEFAULT NOW()
);
