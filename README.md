# MPACKING

Sistema inteligente de reciclaje con recompensas digitales (ecopesos).

Plataforma de economía circular que incentiva el reciclaje mediante empaques sostenibles con códigos QR de un solo uso. El usuario recicla, gana ecopesos y los canjea por beneficios en restaurantes aliados, que reciben el pago automáticamente.

---

## El problema

En Colombia solo se recicla el 17% de los residuos sólidos (DANE, 2023). La causa principal no es falta de voluntad, sino ausencia de incentivos tangibles y de información accesible sobre cómo preparar el material.

MPACKING cierra el ciclo: **QR en el empaque → reciclaje verificado → recompensa digital → canje en restaurante**.

---

## Actores del sistema

| Actor | Función | Plataforma |
|---|---|---|
| Usuario | Recicla empaques QR, acumula ecopesos, los canjea | App móvil |
| Restaurante | Ofrece beneficios, recibe pago automático | App móvil + Web |
| Reciclador | Recolecta material, confirma peso, comparte GPS | App móvil |
| Administrador | Gestiona usuarios, precios, reportes y alertas | Panel web |

---

## Stack tecnológico

| Área | Tecnología |
|---|---|
| App móvil | React Native (Expo) |
| Frontend web | React 18 |
| Backend API | ASP.NET Core 8 |
| Base de datos | PostgreSQL 17 |
| Caché / GPS | Redis 7 |
| Tiempo real | SignalR (WebSocket) |
| Mapas | Google Maps Platform |
| Pagos | Wompi (Colombia) |
| Push | Firebase Cloud Messaging |
| Hosting | Azure App Service |
| CI/CD | GitHub Actions |

---

## Estructura del repositorio

```
mpacking/
├── docs/
│   ├── 01-requisitos/     Informe técnico, avances, presentaciones
│   ├── 02-diseno/         MER, diagramas UML y arquitectura
│   ├── 03-mockups/        19 mockups, plugin Figma, logo y mascota
│   └── evidencias/        Capturas de avance por semana
├── database/              Scripts SQL numerados en orden de ejecución
├── backend/               ASP.NET Core 8 (en desarrollo)
└── mobile/                React Native (en desarrollo)
```

---

## Base de datos

12 tablas normalizadas en Tercera Forma Normal, 4 triggers automáticos y 10 índices de rendimiento.

### Instalación

Crear la base de datos en pgAdmin y ejecutar los scripts **en orden**:

```
database/01_schema.sql            12 tablas
database/02_triggers.sql          4 triggers
database/03_indices.sql           10 índices
database/04_datos_iniciales.sql   18 materiales, 8 restaurantes, 15 beneficios
database/05_pruebas_triggers.sql  Suite de pruebas funcionales
```

> pgAdmin puede truncar scripts largos al pegarlos. Si aparece `error de sintaxis al final de la entrada`, pegar el archivo en bloques más pequeños.

### Cadena de conexión

```
Host=localhost;Port=5432;Database=mpacking_db;Username=postgres;Password=TU_PASSWORD
```

### Triggers

| ID | Evento | Acción automática |
|---|---|---|
| T1 | `AFTER INSERT` en TRANSACCION | Acredita los ecopesos ganados al usuario |
| T2 | `AFTER UPDATE` en REDENCION | Descuenta al aprobar, reintegra al devolver |
| T3 | `AFTER INSERT` en TRANSACCION | Marca el QR como USADO |
| T4 | `BEFORE INSERT` en TRANSACCION | Bloquea si el QR está usado o expirado |

Los triggers actúan como red de seguridad: la integridad se mantiene aunque el backend falle.

---

## Estado del proyecto

| Área | Avance |
|---|---|
| Diseño y documentación | 100% |
| Mockups (19 pantallas, 4 roles) | 100% |
| Base de datos y triggers | 100% |
| Datos reales cargados | 100% |
| Backend ASP.NET Core | 0% |
| App móvil | 0% |
| Integraciones externas | 0% |

Ver [CRONOGRAMA.md](CRONOGRAMA.md) para el detalle de las 25 semanas.

---

## Mockups

Los 19 mockups están disponibles como plugin de Figma en `docs/03-mockups/mpacking_figma_completo.js`.

Para abrirlos:

1. Crear un archivo nuevo en figma.com
2. Abrir la consola con `Ctrl+Alt+I` (Windows) o `Cmd+Option+I` (Mac)
3. Pegar el contenido del archivo `.js` y presionar Enter

Se generan 19 frames editables de 390×844 px organizados en cuatro secciones: Usuario (10), Administrador (3), Restaurante (3) y Reciclador (3).

---

## Convención de commits

| Prefijo | Uso |
|---|---|
| `docs:` | Documentación |
| `feat:` | Funcionalidad nueva |
| `fix:` | Corrección de errores |
| `db:` | Cambios en base de datos |
| `chore:` | Configuración y mantenimiento |

---

## Seguridad

- Contraseñas cifradas con bcrypt (costo 12)
- JWT con expiración de 15 minutos y refresh token rotativo
- Bloqueo de cuenta tras 5 intentos fallidos
- QR de un solo uso con validación a nivel de base de datos
- Pagos vía Wompi (PCI DSS): ningún dato de tarjeta toca los servidores
- Cumplimiento de la Ley 1581 de 2012 (protección de datos personales)

Las credenciales nunca se versionan. Ver `.gitignore`.
