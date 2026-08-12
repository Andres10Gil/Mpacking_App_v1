# Cronograma — MPACKING

Plan de 25 semanas con colchón de contingencia del 20%. MVP funcional en la semana 17.

Leyenda: ✅ completado · 🔄 en curso · ⏳ pendiente

---

## Fase 1 — Diseño y modelado (S1–S4) ✅

| Sem | Actividad | Entregable | Estado |
|---|---|---|---|
| S1 | Levantamiento de requisitos | RF-01 a RF-24 con criterios de aceptación | ✅ |
| S2 | Diseño UML, MER y arquitectura | Diagramas aprobados | ✅ |
| S3 | Diseño UI/UX | 19 mockups y prototipo Figma | ✅ |
| S4 | Base de datos | 12 tablas, 4 triggers, 10 índices verificados | ✅ |

**Evidencias en el repositorio**
- `docs/01-requisitos/MPACKING_Informe_Completo.docx`
- `docs/02-diseno/MPACKING_MER_Completo.docx`
- `docs/03-mockups/mpacking_figma_completo.js`
- `database/01_schema.sql` a `database/05_pruebas_triggers.sql`
- `docs/evidencias/semana-04/`

---

## Fase 2 — Backend (S5–S11) ⏳

| Sem | Actividad | Entregable | Estado |
|---|---|---|---|
| S5–S6 | Auth JWT, usuarios, QR, ecopesos, antifraude | API core con pruebas unitarias | ⏳ |
| S7 | Geolocalización con SignalR | Endpoint de posición en tiempo real | ⏳ |
| S8 | Mapa de restaurantes + Google APIs | Endpoint `/restaurantes/cercanos` | ⏳ |
| S9–S10 | Pasarela Wompi y estados de pago | Flujo de pago completo en sandbox | ⏳ |
| S11 | Tabla de valores, guía de limpieza, reportes | Módulos educativos y de consulta | ⏳ |

---

## Fase 3 — Frontend y app móvil (S12–S16) ⏳

| Sem | Actividad | Entregable | Estado |
|---|---|---|---|
| S12–S13 | Panel web: dashboard, admin, mapa | Panel web completo | ⏳ |
| S14–S15 | App móvil: QR, mapa, tabla, guía | App funcional en simulador | ⏳ |
| S16 | Wompi en móvil y notificaciones push | Redención con pago completo | ⏳ |

---

## Fase 4 — Integración y pruebas (S17–S19) ⏳

| Sem | Actividad | Entregable | Estado |
|---|---|---|---|
| S17 | Integración end-to-end | **MVP funcional verificado** | ⏳ |
| S18–S19 | Pruebas de carga, seguridad y regresión | Informe de pruebas | ⏳ |

---

## Fase 5 — Cierre (S20–S22) ⏳

| Sem | Actividad | Entregable | Estado |
|---|---|---|---|
| S20–S21 | Colchón de contingencia (20%) | Buffer de correcciones | ⏳ |
| S22 | Documentación final y despliegue | Sistema en producción | ⏳ |

---

## Checklist de entregables por semana

### S1 — Requisitos
- [x] Informe técnico completo
- [x] Documento de avances
- [x] Tabla RF-01 a RF-24 con criterios medibles
- [x] Descripción de los 4 actores

### S2 — Diseño
- [x] Diagrama MER
- [x] Arquitectura de 5 capas
- [x] Modelo de datos (12 tablas documentadas)
- [x] Stack tecnológico justificado

### S3 — Mockups
- [x] 19 mockups (Usuario 10, Admin 3, Restaurante 3, Reciclador 3)
- [x] Plugin de Figma editable
- [x] Exportables SVG y PNG
- [x] Logo y mascota oficiales

### S4 — Base de datos
- [x] Script de las 12 tablas
- [x] Script de los 4 triggers
- [x] Script de los 10 índices
- [x] Datos reales: 18 materiales, 8 restaurantes, 1 reciclador, 15 beneficios
- [x] Suite de pruebas de triggers
- [ ] Capturas de evidencia en `docs/evidencias/semana-04/`

### S5 — Backend (siguiente)
- [ ] Visual Studio instalado
- [ ] Proyecto ASP.NET Core 8 creado
- [ ] Entity Framework Core 8 configurado
- [ ] Cadena de conexión a `mpacking_db` funcionando
- [ ] Primer endpoint de prueba respondiendo

---

## Capturas pendientes para la semana 4

Ejecutar en pgAdmin y guardar en `docs/evidencias/semana-04/`:

| Archivo | Cómo obtenerlo |
|---|---|
| `01-tablas-creadas.png` | Panel izquierdo → `mpacking_db` → Schemas → public → Tables |
| `02-triggers-listados.png` | Bloque 9.1 de `05_pruebas_triggers.sql` |
| `03-conteo-datos.png` | Bloque 9.2 de `05_pruebas_triggers.sql` |
| `04-trigger-T1.png` | Bloque 4 — saldo en 225 ecopesos |
| `05-trigger-T3.png` | Bloque 5 — QR en estado USADO |
| `06-trigger-T4.png` | Bloque 6 — error "QR no disponible" |
