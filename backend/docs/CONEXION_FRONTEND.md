# Conexión del frontend con el backend de MPACKING

Guía para quien construya la app móvil: cómo levantar el backend, qué URL usar según dónde corra la app, cómo autenticarse, y el listado completo de endpoints disponibles hoy.

---

## 1. Levantar el backend

```bash
cd backend
cp MPACKING.API/appsettings.Development.json.ejemplo MPACKING.API/appsettings.Development.json
# editar ese archivo con la cadena de conexión real a PostgreSQL y una clave JWT de 32+ caracteres
dotnet run --project MPACKING.API
```

Queda escuchando en `http://localhost:5080`. Swagger (para explorar y probar la API a mano) en `http://localhost:5080/swagger/index.html`.

CORS ya está abierto para cualquier origen en desarrollo (`AppMovil` policy en `Program.cs`), así que no hay que configurar nada adicional del lado del backend para que la app se conecte.

---

## 2. Qué URL base usar según dónde corra la app

| Dónde corre la app | URL base a usar |
|---|---|
| Emulador Android | `http://10.0.2.2:5080` (el emulador no ve `localhost` de tu PC; `10.0.2.2` es la IP especial que apunta al host) |
| Simulador iOS | `http://localhost:5080` (el simulador de iOS sí comparte la red del Mac) |
| Celular físico, misma red Wi-Fi que el PC | `http://192.168.1.14:5080` (IP actual del PC en la red local — verificar con `ipconfig` si cambia; en Windows puede hacer falta permitir el puerto 5080 en el firewall) |

---

## 3. Autenticación

1. `POST /api/auth/registro` o `POST /api/auth/login` devuelven un `accessToken` (JWT) válido por **15 minutos** y un `refreshToken`.
2. En cada request a un endpoint protegido, mandar el header:
   ```
   Authorization: Bearer {accessToken}
   ```
3. **Importante — hueco conocido:** el `refreshToken` se genera pero todavía no hay un endpoint `/api/auth/refresh` que lo reciba y devuelva un token nuevo. Hoy, cuando el `accessToken` expira (15 min), la única opción es volver a hacer login. Está en la lista de pendientes del backend.

---

## 4. Endpoints disponibles

### Auth (`/api/auth`) — públicos

| Método | Ruta | Body | Devuelve |
|---|---|---|---|
| POST | `/api/auth/registro` | `{ nombre, correo, password, rol }` (`rol`: `Usuario`\|`Restaurante`\|`Reciclador`\|`Admin`) | `TokenResponse` |
| POST | `/api/auth/login` | `{ correo, password }` | `TokenResponse` |

`TokenResponse`: `{ accessToken, refreshToken, expiraEnSegundos, usuario: { id, nombre, correo, rol, ecopesosTotal } }`

### Reciclaje (`/api/reciclaje`) — requiere token

| Método | Ruta | Body / Query | Devuelve |
|---|---|---|---|
| GET | `/api/reciclaje/qr/{codigoHash}` | — | `QrInfoResponse` (material, peso, ecopesos estimados, CO2, si es canjeable) |
| POST | `/api/reciclaje/confirmar` | `{ codigoHash, idReciclador, pesoConfirmadoKg }` | `ReciclajeResponse` (ecopesos ganados, saldo actual) |
| GET | `/api/reciclaje/billetera?pagina=1&tamano=20` | — | `BilleteraResponse` (saldo, equivalente en COP, kg reciclados, movimientos) |

### Redenciones (`/api/redenciones`) — requiere token

| Método | Ruta | Body | Devuelve |
|---|---|---|---|
| POST | `/api/redenciones` | `{ idBeneficio }` | `RedencionResponse` (código único, estado Pendiente) |
| POST | `/api/redenciones/{codigoUnico}/aprobar` | — (solo rol `Restaurante` o `Admin`) | `RedencionResponse` (cobra vía pasarela y aprueba) |
| GET | `/api/redenciones/mias` | — | Lista de `RedencionResponse` |

### Catálogo (`/api/catalogo`) — públicos

| Método | Ruta | Devuelve |
|---|---|---|
| GET | `/api/catalogo/materiales` | Lista de materiales con precios y ecopesos por kg |
| GET | `/api/catalogo/materiales/{idMaterial}/guia` | Guía de limpieza del material |

### Geolocalización (`/api/geo`) — requiere token

| Método | Ruta | Body / Query | Devuelve |
|---|---|---|---|
| POST | `/api/geo/posicion` | `{ latitud, longitud, precisionM?, velocidadKmh? }` (solo rol `Reciclador`) | `204 No Content` |
| GET | `/api/geo/reciclador/{idReciclador}` | — | `PosicionResponse` o `404` si no está en ruta |
| GET | `/api/geo/restaurantes/cercanos?lat=&lng=&radioKm=5` | — | Lista de restaurantes cercanos con distancia en km |

### Tiempo real (SignalR)

Hub en `/hubs/gps`. El token JWT va por query string (`?access_token=...`), no por header, porque los WebSocket no lo permiten.

Métodos que la app puede invocar: `SeguirReciclador(idReciclador)`, `DejarDeSeguir(idReciclador)`.
Evento que la app debe escuchar: `PosicionActualizada` → `{ idReciclador, latitud, longitud, timestamp }`.

---

## 5. Códigos de error

Todas las respuestas de error tienen esta forma:

```json
{ "exito": false, "mensaje": "texto explicando qué pasó", "codigo": 409 }
```

| Código | Significa |
|---|---|
| 400 | Datos inválidos (ej. contraseña muy corta, peso ≤ 0) |
| 401 | Credenciales incorrectas, cuenta bloqueada, o token inválido/expirado |
| 404 | El recurso no existe (QR, beneficio, redención, etc.) |
| 409 | La operación viola una regla de negocio (ej. QR ya usado, saldo insuficiente) |
| 500 | Error inesperado del servidor |
