using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MPACKING.Domain.Entities;
using MPACKING.Domain.Enums;

namespace MPACKING.Infrastructure.Persistence.Configurations;

/// <summary>
/// Mapeo entre las entidades del dominio y las tablas de PostgreSQL.
/// Los nombres de tabla y columna van en minúsculas porque así los
/// crearon los scripts SQL del proyecto.
/// </summary>
public class UsuarioConfig : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> b)
    {
        b.ToTable("usuario");
        b.HasKey(x => x.IdUsuario);
        b.Property(x => x.IdUsuario).HasColumnName("id_usuario");
        b.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(120).IsRequired();
        b.Property(x => x.Correo).HasColumnName("correo").HasMaxLength(150).IsRequired();
        b.Property(x => x.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
        b.Property(x => x.EcopesosTotal).HasColumnName("ecopesos_total");
        b.Property(x => x.Activo).HasColumnName("activo");
        b.Property(x => x.IntentosLogin).HasColumnName("intentos_login");
        b.Property(x => x.BloqueadoHasta).HasColumnName("bloqueado_hasta");
        b.Property(x => x.FechaRegistro).HasColumnName("fecha_registro");

        // El enum se guarda como texto para que coincida con el CHECK de la tabla
        b.Property(x => x.Rol)
            .HasColumnName("rol")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString().ToUpperInvariant(),
                v => Enum.Parse<RolUsuario>(v, true));

        b.HasIndex(x => x.Correo).IsUnique();
    }
}

public class RecicladorConfig : IEntityTypeConfiguration<Reciclador>
{
    public void Configure(EntityTypeBuilder<Reciclador> b)
    {
        b.ToTable("reciclador");
        b.HasKey(x => x.IdReciclador);
        b.Property(x => x.IdReciclador).HasColumnName("id_reciclador");
        b.Property(x => x.IdUsuario).HasColumnName("id_usuario");
        b.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(120);
        b.Property(x => x.Telefono).HasColumnName("telefono").HasMaxLength(20);
        b.Property(x => x.ZonaAsignada).HasColumnName("zona_asignada").HasMaxLength(100);
        b.Property(x => x.Activo).HasColumnName("activo");

        b.HasOne(x => x.Usuario).WithOne()
            .HasForeignKey<Reciclador>(x => x.IdUsuario);
    }
}

public class RestauranteConfig : IEntityTypeConfiguration<Restaurante>
{
    public void Configure(EntityTypeBuilder<Restaurante> b)
    {
        b.ToTable("restaurante");
        b.HasKey(x => x.IdRestaurante);
        b.Property(x => x.IdRestaurante).HasColumnName("id_restaurante");
        b.Property(x => x.IdUsuario).HasColumnName("id_usuario");
        b.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(120);
        b.Property(x => x.Direccion).HasColumnName("direccion").HasMaxLength(250);
        b.Property(x => x.Latitud).HasColumnName("latitud").HasPrecision(10, 8);
        b.Property(x => x.Longitud).HasColumnName("longitud").HasPrecision(11, 8);
        b.Property(x => x.HorarioApertura).HasColumnName("horario_apertura");
        b.Property(x => x.HorarioCierre).HasColumnName("horario_cierre");
        b.Property(x => x.DiasAtencion).HasColumnName("dias_atencion").HasMaxLength(50);
        b.Property(x => x.Activo).HasColumnName("activo");

        b.HasOne(x => x.Usuario).WithOne()
            .HasForeignKey<Restaurante>(x => x.IdUsuario);
        b.HasMany(x => x.Beneficios).WithOne(x => x.Restaurante)
            .HasForeignKey(x => x.IdRestaurante);
    }
}

public class MaterialConfig : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> b)
    {
        b.ToTable("material");
        b.HasKey(x => x.IdMaterial);
        b.Property(x => x.IdMaterial).HasColumnName("id_material");
        b.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(100);
        b.Property(x => x.Categoria).HasColumnName("categoria").HasMaxLength(60);
        b.Property(x => x.PrecioMinCopKg).HasColumnName("precio_min_cop_kg").HasPrecision(10, 2);
        b.Property(x => x.PrecioMaxCopKg).HasColumnName("precio_max_cop_kg").HasPrecision(10, 2);
        b.Property(x => x.EcopesosMinKg).HasColumnName("ecopesos_min_kg");
        b.Property(x => x.EcopesosMaxKg).HasColumnName("ecopesos_max_kg");
        b.Property(x => x.ReduccionCo2Kg).HasColumnName("reduccion_co2_kg").HasPrecision(6, 3);
        b.Property(x => x.IconoUrl).HasColumnName("icono_url").HasMaxLength(300);
        b.Property(x => x.Activo).HasColumnName("activo");
        b.Property(x => x.UltimaActualizacion).HasColumnName("ultima_actualizacion");
    }
}

public class QrConfig : IEntityTypeConfiguration<Qr>
{
    public void Configure(EntityTypeBuilder<Qr> b)
    {
        b.ToTable("qr");
        b.HasKey(x => x.IdQr);
        b.Property(x => x.IdQr).HasColumnName("id_qr");
        b.Property(x => x.IdRestaurante).HasColumnName("id_restaurante");
        b.Property(x => x.IdMaterial).HasColumnName("id_material");
        b.Property(x => x.CodigoHash).HasColumnName("codigo_hash").HasMaxLength(255);
        b.Property(x => x.PesoKg).HasColumnName("peso_kg").HasPrecision(8, 3);
        b.Property(x => x.FechaExpiracion).HasColumnName("fecha_expiracion");
        b.Property(x => x.FechaCreacion).HasColumnName("fecha_creacion");

        b.Property(x => x.Estado)
            .HasColumnName("estado")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString().ToUpperInvariant(),
                v => Enum.Parse<EstadoQr>(v, true));

        b.HasIndex(x => x.CodigoHash).IsUnique();
        b.HasOne(x => x.Restaurante).WithMany().HasForeignKey(x => x.IdRestaurante);
        b.HasOne(x => x.Material).WithMany().HasForeignKey(x => x.IdMaterial);
    }
}

public class TransaccionConfig : IEntityTypeConfiguration<Transaccion>
{
    public void Configure(EntityTypeBuilder<Transaccion> b)
    {
        b.ToTable("transaccion");
        b.HasKey(x => x.IdTransaccion);
        b.Property(x => x.IdTransaccion).HasColumnName("id_transaccion");
        b.Property(x => x.IdUsuario).HasColumnName("id_usuario");
        b.Property(x => x.IdQr).HasColumnName("id_qr");
        b.Property(x => x.IdReciclador).HasColumnName("id_reciclador");
        b.Property(x => x.EcopesosGanados).HasColumnName("ecopesos_ganados");
        b.Property(x => x.PesoKg).HasColumnName("peso_kg").HasPrecision(8, 3);
        b.Property(x => x.PrecioCopKg).HasColumnName("precio_cop_kg").HasPrecision(10, 2);
        b.Property(x => x.Fecha).HasColumnName("fecha");
        b.Property(x => x.IpEscaneo).HasColumnName("ip_escaneo").HasMaxLength(45);

        b.HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.IdUsuario);
        b.HasOne(x => x.Qr).WithOne().HasForeignKey<Transaccion>(x => x.IdQr);
        b.HasOne(x => x.Reciclador).WithMany().HasForeignKey(x => x.IdReciclador);
    }
}

public class BeneficioConfig : IEntityTypeConfiguration<Beneficio>
{
    public void Configure(EntityTypeBuilder<Beneficio> b)
    {
        b.ToTable("beneficio");
        b.HasKey(x => x.IdBeneficio);
        b.Property(x => x.IdBeneficio).HasColumnName("id_beneficio");
        b.Property(x => x.IdRestaurante).HasColumnName("id_restaurante");
        b.Property(x => x.Descripcion).HasColumnName("descripcion").HasMaxLength(200);
        b.Property(x => x.EcopesosCosto).HasColumnName("ecopesos_costo");
        b.Property(x => x.Activo).HasColumnName("activo");
        b.Property(x => x.FechaExpiracion).HasColumnName("fecha_expiracion");
    }
}

public class RedencionConfig : IEntityTypeConfiguration<Redencion>
{
    public void Configure(EntityTypeBuilder<Redencion> b)
    {
        b.ToTable("redencion");
        b.HasKey(x => x.IdRedencion);
        b.Property(x => x.IdRedencion).HasColumnName("id_redencion");
        b.Property(x => x.IdUsuario).HasColumnName("id_usuario");
        b.Property(x => x.IdBeneficio).HasColumnName("id_beneficio");
        b.Property(x => x.CodigoUnico).HasColumnName("codigo_unico").HasMaxLength(100);
        b.Property(x => x.EcopesosUsados).HasColumnName("ecopesos_usados");
        b.Property(x => x.FechaSolicitud).HasColumnName("fecha_solicitud");
        b.Property(x => x.FechaValidacion).HasColumnName("fecha_validacion");

        b.Property(x => x.Estado)
            .HasColumnName("estado")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString().ToUpperInvariant(),
                v => Enum.Parse<EstadoRedencion>(v, true));

        b.HasIndex(x => x.CodigoUnico).IsUnique();
        b.HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.IdUsuario);
        b.HasOne(x => x.Beneficio).WithMany().HasForeignKey(x => x.IdBeneficio);
    }
}

public class PagoConfig : IEntityTypeConfiguration<Pago>
{
    public void Configure(EntityTypeBuilder<Pago> b)
    {
        b.ToTable("pago");
        b.HasKey(x => x.IdPago);
        b.Property(x => x.IdPago).HasColumnName("id_pago");
        b.Property(x => x.IdRedencion).HasColumnName("id_redencion");
        b.Property(x => x.MontoCop).HasColumnName("monto_cop").HasPrecision(12, 2);
        b.Property(x => x.ComisionMpacking).HasColumnName("comision_mpacking").HasPrecision(12, 2);
        b.Property(x => x.ComisionPasarela).HasColumnName("comision_pasarela").HasPrecision(12, 2);
        b.Property(x => x.MontoNeto).HasColumnName("monto_neto").HasPrecision(12, 2);
        b.Property(x => x.WompiTxId).HasColumnName("wompi_tx_id").HasMaxLength(100);
        b.Property(x => x.FechaPago).HasColumnName("fecha_pago");
        b.Property(x => x.FechaLiquidacion).HasColumnName("fecha_liquidacion");

        b.Property(x => x.Estado)
            .HasColumnName("estado")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString().ToUpperInvariant(),
                v => Enum.Parse<EstadoPago>(v, true));

        b.HasOne(x => x.Redencion).WithOne().HasForeignKey<Pago>(x => x.IdRedencion);
    }
}

public class UbicacionConfig : IEntityTypeConfiguration<UbicacionReciclador>
{
    public void Configure(EntityTypeBuilder<UbicacionReciclador> b)
    {
        b.ToTable("ubicacion_reciclador");
        b.HasKey(x => x.IdUbicacion);
        b.Property(x => x.IdUbicacion).HasColumnName("id_ubicacion");
        b.Property(x => x.IdReciclador).HasColumnName("id_reciclador");
        b.Property(x => x.Latitud).HasColumnName("latitud").HasPrecision(10, 8);
        b.Property(x => x.Longitud).HasColumnName("longitud").HasPrecision(11, 8);
        b.Property(x => x.PrecisionM).HasColumnName("precision_m");
        b.Property(x => x.VelocidadKmh).HasColumnName("velocidad_kmh").HasPrecision(5, 2);
        b.Property(x => x.Timestamp).HasColumnName("timestamp");

        b.HasOne(x => x.Reciclador).WithMany().HasForeignKey(x => x.IdReciclador);
    }
}

public class GuiaLimpiezaConfig : IEntityTypeConfiguration<GuiaLimpieza>
{
    public void Configure(EntityTypeBuilder<GuiaLimpieza> b)
    {
        b.ToTable("guia_limpieza");
        b.HasKey(x => x.IdGuia);
        b.Property(x => x.IdGuia).HasColumnName("id_guia");
        b.Property(x => x.IdMaterial).HasColumnName("id_material");
        b.Property(x => x.DatoCurioso).HasColumnName("dato_curioso").HasMaxLength(300);
        b.Property(x => x.ImagenUrl).HasColumnName("imagen_url").HasMaxLength(300);
        b.Property(x => x.VideoUrl).HasColumnName("video_url").HasMaxLength(300);
        b.Property(x => x.Activo).HasColumnName("activo");

        // Las listas se guardan como JSONB, igual que en el script SQL.
        // EF necesita un conversor explícito para serializarlas.
        var conversorLista = new ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)
                 ?? new List<string>());

        var comparadorLista = new ValueComparer<List<string>>(
            (a, c) => a != null && c != null && a.SequenceEqual(c),
            v => v.Aggregate(0, (h, s) => HashCode.Combine(h, s.GetHashCode())),
            v => v.ToList());

        b.Property(x => x.PasosLimpieza)
            .HasColumnName("pasos_limpieza").HasColumnType("jsonb")
            .HasConversion(conversorLista).Metadata.SetValueComparer(comparadorLista);

        b.Property(x => x.ErroresComunes)
            .HasColumnName("errores_comunes").HasColumnType("jsonb")
            .HasConversion(conversorLista).Metadata.SetValueComparer(comparadorLista);

        b.HasOne(x => x.Material).WithOne().HasForeignKey<GuiaLimpieza>(x => x.IdMaterial);
    }
}

public class HistorialPrecioConfig : IEntityTypeConfiguration<HistorialPrecio>
{
    public void Configure(EntityTypeBuilder<HistorialPrecio> b)
    {
        b.ToTable("historial_precio");
        b.HasKey(x => x.IdHistorial);
        b.Property(x => x.IdHistorial).HasColumnName("id_historial");
        b.Property(x => x.IdMaterial).HasColumnName("id_material");
        b.Property(x => x.CambiadoPor).HasColumnName("cambiado_por");
        b.Property(x => x.PrecioCopKg).HasColumnName("precio_cop_kg").HasPrecision(10, 2);
        b.Property(x => x.EcopesosKg).HasColumnName("ecopesos_kg");
        b.Property(x => x.FechaInicio).HasColumnName("fecha_inicio");
        b.Property(x => x.FechaFin).HasColumnName("fecha_fin");

        b.HasOne(x => x.Material).WithMany().HasForeignKey(x => x.IdMaterial);
        b.HasOne(x => x.Responsable).WithMany().HasForeignKey(x => x.CambiadoPor);
    }
}
