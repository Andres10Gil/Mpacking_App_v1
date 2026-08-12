# Backend — ASP.NET Core 8

Pendiente (semanas 5 a 11 del cronograma).

## Primeros pasos

1. Instalar Visual Studio 2022 con la carga de trabajo "ASP.NET y desarrollo web"
2. Crear el proyecto: `ASP.NET Core Web API` sobre .NET 8
3. Instalar el paquete `Npgsql.EntityFrameworkCore.PostgreSQL`
4. Configurar la cadena de conexión en `appsettings.json`

```
Host=localhost;Port=5432;Database=mpacking_db;Username=postgres;Password=TU_PASSWORD
```

> La cadena con la contraseña real va en `appsettings.Development.json`, que está excluido del control de versiones.

## Módulos a desarrollar

- Autenticación JWT con refresh token
- Escaneo y validación de QR
- Cálculo y acreditación de ecopesos
- Geolocalización con SignalR
- Mapa de restaurantes cercanos
- Redención y pasarela Wompi
