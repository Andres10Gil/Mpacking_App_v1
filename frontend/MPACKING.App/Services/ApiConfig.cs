namespace MPACKING.App.Services;

public static class ApiConfig
{
#if ANDROID
    public const string BaseUrl = "http://10.0.2.2:5080";
#else
    public const string BaseUrl = "http://localhost:5080";
#endif
}

public class ApiException : Exception
{
    public ApiException(string mensaje) : base(mensaje) { }
}
