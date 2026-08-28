using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MPACKING.API;
using MPACKING.API.Middleware;
using MPACKING.Application;
using MPACKING.Application.Common.Interfaces;
using MPACKING.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ---------- Capas de la aplicación ----------
builder.Services.AgregarApplication();
builder.Services.AgregarInfrastructure(builder.Configuration);

// SignalR vive en la capa API porque es un detalle de transporte
builder.Services.AddSignalR();
builder.Services.AddScoped<INotificadorUbicacion, NotificadorSignalR>();

// ---------- Autenticación JWT ----------
var claveJwt = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Falta configurar Jwt:Key.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opciones =>
    {
        opciones.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(claveJwt)),
            ClockSkew = TimeSpan.Zero
        };

        // SignalR envía el token por query string porque los WebSocket
        // no admiten cabeceras personalizadas
        opciones.Events = new JwtBearerEvents
        {
            OnMessageReceived = contexto =>
            {
                var token = contexto.Request.Query["access_token"];
                var ruta = contexto.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(token) && ruta.StartsWithSegments("/hubs/gps"))
                    contexto.Token = token;

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// ---------- CORS para la app móvil ----------
const string PoliticaCors = "AppMovil";
builder.Services.AddCors(opciones =>
    opciones.AddPolicy(PoliticaCors, politica => politica
        .AllowAnyHeader()
        .AllowAnyMethod()
        .SetIsOriginAllowed(_ => true)
        .AllowCredentials()));

// ---------- Controllers y Swagger ----------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MPACKING API",
        Version = "v1",
        Description = "Sistema inteligente de reciclaje con ecopesos"
    });

    opciones.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Pega aquí el token JWT (sin escribir 'Bearer').",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    opciones.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ---------- Pipeline ----------
app.UseMiddleware<ManejadorExcepciones>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(PoliticaCors);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<GpsHub>("/hubs/gps");

app.MapGet("/", () => Results.Ok(new
{
    servicio = "MPACKING API",
    version = "1.0",
    estado = "en línea"
}));

app.Run();
