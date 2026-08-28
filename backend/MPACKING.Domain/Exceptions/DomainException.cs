namespace MPACKING.Domain.Exceptions;

/// <summary>
/// Excepción base para todas las violaciones de reglas de negocio.
/// El middleware la traduce a una respuesta HTTP con el código adecuado.
/// </summary>
public abstract class DomainException : Exception
{
    public abstract int StatusCode { get; }
    protected DomainException(string mensaje) : base(mensaje) { }
}

/// <summary>El recurso solicitado no existe. Se traduce a HTTP 404.</summary>
public sealed class NoEncontradoException : DomainException
{
    public override int StatusCode => 404;
    public NoEncontradoException(string recurso, object id)
        : base($"No se encontró {recurso} con identificador {id}.") { }
}

/// <summary>La operación viola una regla de negocio. Se traduce a HTTP 409.</summary>
public sealed class ReglaNegocioException : DomainException
{
    public override int StatusCode => 409;
    public ReglaNegocioException(string mensaje) : base(mensaje) { }
}

/// <summary>Los datos recibidos no son válidos. Se traduce a HTTP 400.</summary>
public sealed class DatosInvalidosException : DomainException
{
    public override int StatusCode => 400;
    public DatosInvalidosException(string mensaje) : base(mensaje) { }
}

/// <summary>Credenciales incorrectas o cuenta bloqueada. Se traduce a HTTP 401.</summary>
public sealed class AutenticacionException : DomainException
{
    public override int StatusCode => 401;
    public AutenticacionException(string mensaje) : base(mensaje) { }
}
