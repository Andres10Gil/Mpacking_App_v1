# MPACKING

Sistema de reciclaje con recompensas digitales (ecopesos). Monolito modular en ASP.NET Core 8 sobre PostgreSQL 17.

El usuario escanea un QR impreso en un empaque, el reciclador confirma el peso, el usuario gana ecopesos y los canjea en restaurantes aliados que reciben el pago automáticamente.

---

## Estado actual

| Área | Estado |
|---|---|
| Documentación y requisitos (RF-01 a RF-24) | completo |
| Base de datos: 12 tablas, 4 triggers, 10 índices | completo y probado |
| Datos reales cargados | 18 materiales, 8 restaurantes, 1 reciclador, 15 beneficios |
| Backend | esqueleto generado, **sin compilar todavía** |
| App móvil | no iniciada |

**Lo primero que hay que hacer:** `dotnet build` y corregir los errores. El código se escribió sin acceso a un compilador.

---

## Arquitectura

Cuatro proyectos, dependencias en una sola dirección:

```
MPACKING.API ──► MPACKING.Infrastructure ──► MPACKING.Application ──► MPACKING.Domain
```

`Domain` no referencia a nadie. Eso hace imposible violar la inversión de dependencias: el compilador lo impide.

| Proyecto | Contiene |
|---|---|
| `Domain` | Entidades con lógica de negocio, enums, excepciones |
| `Application` | Servicios, DTOs, interfaces de repositorio |
| `Infrastructure` | EF Core, repositorios, bcrypt, JWT, caché |
| `API` | Controllers, middleware, SignalR |

Los módulos de negocio viven en `Application/Modules/`: `Auth`, `Reciclaje`, `Redencion`, `Catalogo`, `Geolocalizacion`. Cada uno con sus DTOs, su interfaz y su implementación.

---

## Regla crítica: no duplicar los triggers

La base de datos tiene cuatro triggers que **ya hacen trabajo de negocio**. El código de C# no debe repetirlo.

| Trigger | Cuándo | Qué hace |
|---|---|---|
| T1 | `AFTER INSERT` en `transaccion` | Suma los ecopesos al usuario |
| T2 | `AFTER UPDATE` en `redencion` | Descuenta al aprobar, reintegra al devolver |
| T3 | `AFTER INSERT` en `transaccion` | Marca el QR como `USADO` |
| T4 | `BEFORE INSERT` en `transaccion` | Lanza excepción si el QR está usado o expirado |

Patrón correcto en `ReciclajeService.ConfirmarAsync`:

```csharp
await _transacciones.AgregarAsync(transaccion, ct);
await _uow.GuardarCambiosAsync(ct);          // aquí corren T4, T1, T3

var saldo = await _usuarios.ObtenerSaldoAsync(idUsuario, ct);  // relee
```

Si un servicio también sumara ecopesos, se contarían dos veces. `ObtenerSaldoAsync` usa `AsNoTracking()` porque el trigger modificó la fila fuera del contexto de EF.

`Qr.ValidarCanjeable()` sí duplica la lógica de T4, pero solo para dar un mensaje claro antes de llegar a la base. El trigger sigue siendo la garantía real.

---

## Flujo de pago (RF-12)

En `RedencionService.AprobarAsync` el orden importa:

1. Se cobra por la pasarela
2. Si falla: se registra el pago rechazado y se lanza excepción. La redención queda `Pendiente`, T2 no corre, los ecopesos **no se descuentan**
3. Si funciona: `redencion.Aprobar()` y T2 descuenta

Nunca invertir ese orden.

---

## Cosas del entorno

- Los `id_material` en la base empiezan en **9**, no en 1 (se borraron materiales de prueba)
- pgAdmin trunca scripts SQL largos al pegarlos; si aparece `error de sintaxis al final de la entrada`, pegar por bloques
- Los enums se guardan como texto en mayúsculas para coincidir con los `CHECK` de las tablas
- Las columnas de la base están en minúsculas (`id_usuario`, `ecopesos_total`); el mapeo está en `Persistence/Configurations/`

---

## Comandos

```bash
# Backend
cd backend
dotnet restore
dotnet build
dotnet run --project MPACKING.API      # http://localhost:5080/swagger

# Base de datos: ejecutar en pgAdmin en este orden
database/01_schema.sql
database/02_triggers.sql
database/03_indices.sql
database/04_datos_iniciales.sql
database/05_pruebas_triggers.sql
```

---

## Configuración

`appsettings.Development.json` está en `.gitignore` y no existe en el repo. Copiarlo del ejemplo:

```bash
cd backend/MPACKING.API
cp appsettings.Development.json.ejemplo appsettings.Development.json
```

Necesita `ConnectionStrings:MpackingDb` y `Jwt:Key` (mínimo 32 caracteres).

**Nunca subir credenciales al repositorio.**

---

## Implementaciones intercambiables

Tres servicios están detrás de interfaces para desarrollar sin cuentas externas. Cambiarlos es una línea en `Infrastructure/DependencyInjection.cs`:

| Interfaz | Ahora | Destino |
|---|---|---|
| `IPasarelaPago` | `PasarelaPagoSimulada` | Wompi |
| `ICacheService` | `CacheEnMemoria` | Redis |

`PasarelaPagoSimulada` rechaza montos ≤ 0, lo que permite probar el camino de error del RF-12.

---

## Git

- `main`: entregables estables
- `develop`: trabajo diario
- Ramas por tarea: `feature/`, `fix/`, `db/`, `docs/`

Mensajes de commit con prefijo: `feat:`, `fix:`, `docs:`, `db:`, `chore:`.

Detalles en `docs/RAMAS.md`.

---

## Convenciones de código

- Nombres de dominio en español (`Usuario`, `Reciclaje`, `EcopesosGanados`) — el equipo y la documentación están en español
- Entidades con setters privados y constructores que validan
- Las reglas de negocio viven en las entidades, no en los servicios
- Los servicios orquestan; no escriben SQL
- Excepciones de dominio (`ReglaNegocioException`, `NoEncontradoException`) que el middleware traduce a códigos HTTP

---

## Pendiente

- [ ] Compilar y corregir errores
- [ ] Probar el flujo de reciclaje contra la base real
- [ ] Refresh token con persistencia
- [ ] Controller de administración (RF-16, RF-17)
- [ ] Reportes ambientales en PDF (RF-24)
- [ ] Pruebas unitarias
- [ ] App móvil React Native
