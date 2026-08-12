# Evidencias — Semana 4 (Base de datos)

Guardar aquí las capturas de pantalla de pgAdmin con estos nombres:

| Archivo | Cómo obtenerlo |
|---|---|
| `01-tablas-creadas.png` | Panel izquierdo → mpacking_db → Schemas → public → Tables (12 tablas) |
| `02-triggers-listados.png` | Ejecutar el bloque 9.1 de `database/05_pruebas_triggers.sql` |
| `03-conteo-datos.png` | Ejecutar el bloque 9.2 de `database/05_pruebas_triggers.sql` |
| `04-trigger-T1.png` | Bloque 4 — el saldo debe mostrar 225 ecopesos |
| `05-trigger-T3.png` | Bloque 5 — el QR debe mostrar estado USADO |
| `06-trigger-T4.png` | Bloque 6 — debe mostrar el error "QR no disponible" |

El error del bloque 6 es el resultado correcto: demuestra que el trigger bloquea el fraude por doble escaneo.
