using MPACKING.Domain.Enums;
using MPACKING.Domain.Exceptions;

namespace MPACKING.Domain.Entities;

/// <summary>
/// Tabla central del sistema. Los cuatro actores (usuario, restaurante,
/// reciclador y administrador) se distinguen por el campo Rol.
/// </summary>
public class Usuario
{
    private const int MaxIntentosLogin = 5;
    private const int MinutosBloqueo = 30;

    public int IdUsuario { get; private set; }
    public string Nombre { get; private set; } = null!;
    public string Correo { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public RolUsuario Rol { get; private set; }
    public int EcopesosTotal { get; private set; }
    public bool Activo { get; private set; } = true;
    public short IntentosLogin { get; private set; }
    public DateTime? BloqueadoHasta { get; private set; }
    public DateTime FechaRegistro { get; private set; } = DateTime.UtcNow;

    // EF Core necesita un constructor sin parámetros
    private Usuario() { }

    public Usuario(string nombre, string correo, string passwordHash, RolUsuario rol)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DatosInvalidosException("El nombre es obligatorio.");
        if (string.IsNullOrWhiteSpace(correo) || !correo.Contains('@'))
            throw new DatosInvalidosException("El correo no tiene un formato válido.");

        Nombre = nombre.Trim();
        Correo = correo.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        Rol = rol;
    }

    /// <summary>Indica si la cuenta está bloqueada por intentos fallidos.</summary>
    public bool EstaBloqueado =>
        BloqueadoHasta.HasValue && BloqueadoHasta.Value > DateTime.UtcNow;

    /// <summary>
    /// Registra un intento fallido. Al llegar al máximo bloquea la cuenta.
    /// Implementa el RF-03 del documento de requisitos.
    /// </summary>
    public void RegistrarIntentoFallido()
    {
        IntentosLogin++;
        if (IntentosLogin >= MaxIntentosLogin)
        {
            BloqueadoHasta = DateTime.UtcNow.AddMinutes(MinutosBloqueo);
            IntentosLogin = 0;
        }
    }

    public void RegistrarLoginExitoso()
    {
        IntentosLogin = 0;
        BloqueadoHasta = null;
    }

    /// <summary>
    /// Refresca el saldo tras una operación. El cálculo real lo hacen los
    /// triggers T1 y T2 en PostgreSQL; este método solo sincroniza la entidad.
    /// </summary>
    public void SincronizarSaldo(int saldoEnBaseDatos) => EcopesosTotal = saldoEnBaseDatos;

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;
}
