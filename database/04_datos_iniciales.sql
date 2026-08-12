-- ============================================================
-- MPACKING - Datos iniciales de produccion
-- Motor: PostgreSQL 17
--
-- ORDEN DE EJECUCION: 4 de 5
--
-- Fuente: BASE_DATOS_APP_RECICLA.xlsx
-- 18 materiales · 8 restaurantes reales de Bogota · 1 reciclador
-- ============================================================

-- ------------------------------------------------------------
-- MATERIALES (18 materiales con precios de mercado)
-- ------------------------------------------------------------
INSERT INTO MATERIAL (nombre, categoria, precio_min_cop_kg, precio_max_cop_kg,
                      ecopesos_min_kg, ecopesos_max_kg, reduccion_co2_kg) VALUES
('Papel blanco',  'Papel',      900,  1100, 90,  110, 0.900),
('Papel',         'Papel',      600,  800,  60,  80,  0.800),
('Aceite',        'Especiales', 450,  550,  45,  55,  2.500),
('Cobre',         'Metales',    900,  1100, 90,  110, 3.500),
('Hierro',        'Metales',    900,  1100, 90,  110, 1.800),
('Bronce',        'Metales',    900,  1100, 90,  110, 2.000),
('Aluminio',      'Metales',    4500, 5500, 450, 550, 9.000),
('Carton',        'Papel',      400,  500,  40,  50,  1.100),
('Vidrio',        'Vidrio',     80,   120,  8,   12,  0.300),
('Madera',        'Especiales', 450,  550,  45,  55,  0.600),
('Tela',          'Especiales', 550,  650,  55,  65,  0.400),
('Tetra Pak',     'Combinados', 700,  900,  70,  90,  0.700),
('PET Soplado',   'Plasticos',  800,  1000, 80,  100, 1.500),
('PET Blanco',    'Plasticos',  1100, 1300, 110, 130, 1.600),
('PET Oscuro',    'Plasticos',  500,  700,  50,  70,  1.200),
('PET',           'Plasticos',  1300, 1500, 130, 150, 1.500),
('PVC',           'Plasticos',  450,  550,  45,  55,  0.800),
('Vasijas',       'Especiales', 800,  1000, 80,  100, 0.500);


-- ------------------------------------------------------------
-- USUARIO ADMINISTRADOR
-- ------------------------------------------------------------
INSERT INTO USUARIO (nombre, correo, password_hash, rol) VALUES
('Admin MPACKING', 'admin@mpacking.co', '$2b$12$placeholder_admin', 'ADMIN');


-- ------------------------------------------------------------
-- RESTAURANTES ALIADOS (8 reales de Bogota)
-- Paso 1: cuentas de usuario
-- ------------------------------------------------------------
INSERT INTO USUARIO (nombre, correo, password_hash, rol) VALUES
('Mi Parrilla Boyacense - Normandia',   'normandia@miparrilla.com',        '$2b$12$placeholder', 'RESTAURANTE'),
('Mi Parrilla Boyacense - Chapinero',   'chapinero@miparrilla.com',        '$2b$12$placeholder', 'RESTAURANTE'),
('Mi Parrilla Boyacense - Usaquen',     'usaquen@miparrilla.com',          '$2b$12$placeholder', 'RESTAURANTE'),
('Mi Parrilla Boyacense - B. Unidos',   'bunidos@miparrilla.com',          '$2b$12$placeholder', 'RESTAURANTE'),
('Mi Parrilla Boyacense - Toberin',     'toberin@miparrilla.com',          '$2b$12$placeholder', 'RESTAURANTE'),
('Mi Parrilla Boyacense - Chapinero 2', 'chapinero2@miparrilla.com',       '$2b$12$placeholder', 'RESTAURANTE'),
('Hamburguesas Jiret',                  'contacto@hamburgueasjiret.com',   '$2b$12$placeholder', 'RESTAURANTE'),
('Panaderia Alejandria',                'contacto@panaderiaalejandia.com', '$2b$12$placeholder', 'RESTAURANTE');

-- Paso 2: perfiles con direccion y coordenadas GPS
INSERT INTO RESTAURANTE (id_usuario, nombre, direccion, latitud, longitud) VALUES
((SELECT id_usuario FROM USUARIO WHERE correo='normandia@miparrilla.com'),
 'Mi Parrilla Boyacense', 'Av el Dorado No 62-49 locales 267', 4.6589, -74.1089),

((SELECT id_usuario FROM USUARIO WHERE correo='chapinero@miparrilla.com'),
 'Mi Parrilla Boyacense', 'Calle 71A No 30-53', 4.6612, -74.0589),

((SELECT id_usuario FROM USUARIO WHERE correo='usaquen@miparrilla.com'),
 'Mi Parrilla Boyacense', 'CC Portobelo Calle 93b #11a-84 Local 101', 4.6762, -74.0489),

((SELECT id_usuario FROM USUARIO WHERE correo='bunidos@miparrilla.com'),
 'Mi Parrilla Boyacense', 'Ak 7 #67-64', 4.6487, -74.0589),

((SELECT id_usuario FROM USUARIO WHERE correo='toberin@miparrilla.com'),
 'Mi Parrilla Boyacense', 'Cl. 162 #22-68', 4.7489, -74.0789),

((SELECT id_usuario FROM USUARIO WHERE correo='chapinero2@miparrilla.com'),
 'Mi Parrilla Boyacense', 'Av. Carrera 86 #55A-75 Local 130', 4.6512, -74.1189),

((SELECT id_usuario FROM USUARIO WHERE correo='contacto@hamburgueasjiret.com'),
 'Hamburguesas Jiret', 'CRA 30 #3-01', 4.5987, -74.0989),

((SELECT id_usuario FROM USUARIO WHERE correo='contacto@panaderiaalejandia.com'),
 'Panaderia Alejandria', 'Cra. 7 #12-62', 4.5989, -74.0789);


-- ------------------------------------------------------------
-- RECICLADOR ALIADO
-- ------------------------------------------------------------
INSERT INTO USUARIO (nombre, correo, password_hash, rol) VALUES
('Ecoreciclajes F-C', 'contacto@ecoreciclajes.com', '$2b$12$placeholder', 'RECICLADOR');

INSERT INTO RECICLADOR (id_usuario, nombre, telefono, zona_asignada) VALUES
((SELECT id_usuario FROM USUARIO WHERE correo='contacto@ecoreciclajes.com'),
 'Ecoreciclajes F-C', '3103224523', 'Tunjuelito - KR 12D #55A-34 Sur');


-- ------------------------------------------------------------
-- BENEFICIOS DE LOS RESTAURANTES
-- ------------------------------------------------------------
INSERT INTO BENEFICIO (id_restaurante, descripcion, ecopesos_costo) VALUES
(1, 'Descuento 20% en cualquier plato', 150),
(1, 'Bebida gratis con tu pedido',        80),
(1, 'Almuerzo ejecutivo gratis',         400),
(2, 'Descuento 15% en el menu del dia',  120),
(2, 'Postre gratis',                      90),
(3, 'Descuento 20% en asados',           150),
(3, 'Jugo natural gratis',                70),
(4, 'Descuento 25% en almuerzo',         180),
(5, 'Cafe gratis con tu compra',          50),
(5, 'Descuento 10% en toda la carta',    100),
(6, 'Combo hamburguesa mas papas',       350),
(7, 'Hamburguesa sencilla gratis',       300),
(7, 'Descuento 20% en combos',           160),
(8, 'Pan artesanal gratis',               60),
(8, 'Descuento 15% en panaderia',        110);


-- ------------------------------------------------------------
-- VERIFICACION
-- ------------------------------------------------------------
SELECT 'materiales'   AS tabla, COUNT(*) AS total FROM MATERIAL
UNION ALL SELECT 'restaurantes', COUNT(*) FROM RESTAURANTE
UNION ALL SELECT 'recicladores', COUNT(*) FROM RECICLADOR
UNION ALL SELECT 'beneficios',   COUNT(*) FROM BENEFICIO
UNION ALL SELECT 'usuarios',     COUNT(*) FROM USUARIO;
