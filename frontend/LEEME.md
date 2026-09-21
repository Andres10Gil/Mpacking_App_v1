# App móvil — .NET MAUI

Entorno ya preparado: .NET SDK + workload `maui`, C# Dev Kit, Android Studio, SDK de Android, JDK 21 y un emulador (`Pixel_6_API_34`, Android 14) instalados y probados. Proyecto base creado en `MPACKING.App/` (plantilla por defecto, sin pantallas de MPACKING todavía).

Cómo conectarse al backend (URLs, autenticación, endpoints disponibles): ver `../backend/docs/CONEXION_FRONTEND.md`.

Las 19 pantallas están diseñadas en `docs/mockups/mpacking_figma_completo.js` (mockups originalmente pensados para React Native — sirven como referencia visual, hay que adaptarlos a XAML), y también en Figma: https://www.figma.com/design/oePdsoBXSmEz1VIDZC96uu/mpaking?node-id=1010-64

## Pantallas por rol

- **Usuario (10):** Login, Registro, Inicio, Escanear QR, Ecopesos ganados, Mapa, Billetera, Tabla de valores, Guía de limpieza, Redención
- **Administrador (3):** Panel, Usuarios, Reportes
- **Restaurante (3):** Inicio, Beneficios, Historial de pagos
- **Reciclador (3):** Ruta, Confirmar material, Historial
