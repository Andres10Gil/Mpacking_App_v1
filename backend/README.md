# Backend — MPACKING API

API REST en ASP.NET Core 8 sobre PostgreSQL 17. Monolito modular organizado por funcionalidad.

---

## Arquitectura

Cuatro proyectos con dependencias en una sola dirección. El compilador impide violar la regla: `Domain` no puede referenciar `Infrastructure` porque no existe la referencia.

```
MPACKING.API  ──►  MPACKING.Infrastructure  ──►  MPACKING.Application  ──►  MPACKING.Domain
```

| Proyecto | Contiene | Depende de |
|---|---|---|
| `MPACKING.Domain` | Entidades, enums, excepciones de negocio | nada |
| `MPACKING.Application` | Servicios, DTOs, interfaces de repositorio | Domain |
| `MPACKING.Infrastructure` | EF Core, repositorios, bcrypt, JWT, caché | Application |
| `MPACKING.API` | Controllers, middleware, SignalR | Infrastructure |

---

## Organización por módulos

En lugar de una carpeta `Services/` con todo mezclado, cada funcionalidad vive en su propio módulo:

```
MPACKING.Application/Modules/
├── Auth/              registro, login, bloqueo por intentos
├── Reciclaje/         escaneo QR, transacciones, billetera
├── Redencion/         beneficios, canjes, pagos
├── Catalogo/          tabla de valores, guías de limpieza
└── Geolocalizacion/   GPS del reciclador, restaurantes cercanos
```

Cada módulo tiene sus DTOs, su interfaz y su implementación. Si algún día un módulo necesita extraerse a un servicio aparte, ya está aislado.

---

## Decisión importante: los triggers no se duplican

La base de datos tiene cuatro triggers que ya hacen trabajo de negocio:

| Trigger | Qué hace |
|---|---|
| T1 | Acredita ecopesos al insertar una transacción |
| T2 | Descuenta o reintegra al cambiar el estado de una redención |
| T3 | Marca el QR como usado |
| T4 | Bloquea QR usados o expirados |

**Los servicios no repiten esa lógica.** `ReciclajeService` solo inserta la transacción y después relee el saldo que el trigger ya actualizó. Si el servicio también sumara ecopesos, se contarían dos veces.

La validación en `Qr.ValidarCanjeable()` sí existe, pero con otro propósito: dar un mensaje claro al usuario antes de llegar a la base. El trigger T4 sigue siendo la garantía real.

---

## Requisitos

- .NET 8 SDK
- PostgreSQL 17 con la base `mpacking_db` creada
- Los scripts de `database/` ejecutados en orden

---

## Configuración

Copiar el archivo de ejemplo y poner las credenciales reales:

```bash
cd MPACKING.API
cp appsettings.Development.json.ejemplo appsettings.Development.json
```

Editar `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "MpackingDb": "Host=localhost;Port=5432;Database=mpacking_db;Username=postgres;Password=TU_PASSWORD"
  },
  "Jwt": {
    "Key": "una-clave-de-al-menos-32-caracteres-aqui"
  }
}
```

> Ese archivo está en `.gitignore`. Nunca se sube al repositorio.

---

## Ejecutar

```bash
dotnet restore
dotnet build
dotnet run --project MPACKING.API
```

La API queda en `http://localhost:5080` y Swagger en `http://localhost:5080/swagger`.

---

## Probar el flujo completo

**1. Registrar un usuario**

```
POST /api/auth/registro
{
  "nombre": "Juan García",
  "correo": "juan@test.com",
  "password": "Password123",
  "rol": "Usuario"
}
```

Devuelve el token JWT. En Swagger, pegarlo en el botón **Authorize**.

**2. Consultar el catálogo** (no requiere token)

```
GET /api/catalogo/materiales
```

**3. Consultar un QR**

```
GET /api/reciclaje/qr/QR-CARLOS-001
```

**4. Confirmar el reciclaje**

```
POST /api/reciclaje/confirmar
{
  "codigoHash": "QR-CARLOS-001",
  "idReciclador": 1,
  "pesoConfirmadoKg": 2.5
}
```

Aquí se disparan T4, T1 y T3. La respuesta trae el saldo ya actualizado.

**5. Ver la billetera**

```
GET /api/reciclaje/billetera
```

**6. Reintentar el mismo QR** — debe devolver 409 con el mensaje del trigger T4.

---

## Endpoints

| Método | Ruta | Requisito | RF |
|---|---|---|---|
| POST | `/api/auth/registro` | público | RF-01 |
| POST | `/api/auth/login` | público | RF-02, RF-03 |
| GET | `/api/catalogo/materiales` | público | RF-21 |
| GET | `/api/catalogo/materiales/{id}/guia` | público | RF-22 |
| GET | `/api/reciclaje/qr/{codigo}` | token | RF-05 |
| POST | `/api/reciclaje/confirmar` | token | RF-06, RF-07, RF-08 |
| GET | `/api/reciclaje/billetera` | token | RF-09 |
| POST | `/api/redenciones` | token | RF-11 |
| POST | `/api/redenciones/{codigo}/aprobar` | rol Restaurante | RF-10, RF-14, RF-20 |
| GET | `/api/redenciones/mias` | token | RF-13 |
| POST | `/api/geo/posicion` | rol Reciclador | RF-18 |
| GET | `/api/geo/reciclador/{id}` | token | RF-18 |
| GET | `/api/geo/restaurantes/cercanos` | token | RF-19 |
| WS | `/hubs/gps` | token en query string | RF-18, RF-23 |

---

## Implementaciones intercambiables

Tres servicios están detrás de una interfaz para poder desarrollar sin depender de cuentas externas:

| Interfaz | Ahora | En producción |
|---|---|---|
| `IPasarelaPago` | `PasarelaPagoSimulada` | `WompiPasarelaPago` |
| `ICacheService` | `CacheEnMemoria` | `RedisCacheService` |
| `INotificadorUbicacion` | `NotificadorSignalR` | igual |

Cambiar de una a otra es una línea en `Infrastructure/DependencyInjection.cs`. Ningún servicio de negocio se entera.

Para probar el manejo de error de pago (RF-12), `PasarelaPagoSimulada` rechaza montos menores o iguales a cero: la redención queda pendiente y los ecopesos no se descuentan.

---

## Siguientes pasos

- [ ] Restaurar paquetes y verificar que compila
- [ ] Probar el flujo de reciclaje contra la base real
- [ ] Implementar refresh token con persistencia
- [ ] Controller de administración (RF-16, RF-17)
- [ ] Reportes ambientales en PDF (RF-24)
- [ ] Sustituir la pasarela simulada por Wompi sandbox
- [ ] Pruebas unitarias de los servicios
