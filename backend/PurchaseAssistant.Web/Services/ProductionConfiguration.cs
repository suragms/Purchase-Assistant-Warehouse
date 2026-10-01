using Microsoft.Extensions.Configuration;

namespace PurchaseAssistant.Web.Services;

public static class ProductionConfiguration
{
    public static void Validate(IConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection")))
            throw new InvalidOperationException("Configure ConnectionStrings:DefaultConnection before starting production.");
        var keyPath = configuration["DataProtection:KeyRingPath"];
        if (string.IsNullOrWhiteSpace(keyPath) || !Path.IsPathFullyQualified(keyPath))
            throw new InvalidOperationException("Configure an absolute DataProtection:KeyRingPath for production.");
        if (!OperatingSystem.IsWindows() && string.IsNullOrWhiteSpace(configuration["DataProtection:CertificatePath"]))
            throw new InvalidOperationException("Configure DataProtection:CertificatePath to encrypt production keys on this platform.");
        var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        if (origins == null || origins.Length == 0 || origins.Any(origin =>
            !Uri.TryCreate(origin, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps
            || uri.UserInfo.Length > 0 || uri.Host.Contains('*') || uri.AbsolutePath != "/"
            || uri.Query.Length > 0 || uri.Fragment.Length > 0))
            throw new InvalidOperationException("Configure explicit HTTPS Cors:AllowedOrigins for production.");
    }
}
