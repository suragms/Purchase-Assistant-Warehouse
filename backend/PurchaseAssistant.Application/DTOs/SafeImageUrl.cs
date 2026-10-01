namespace PurchaseAssistant.Application.DTOs;
public static class SafeImageUrl
{
    public static string? Validate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        if (!Uri.TryCreate(value.Trim(), UriKind.Absolute, out var uri) || uri.Scheme != "https" || !string.IsNullOrEmpty(uri.UserInfo)
            || uri.IsLoopback || uri.Host.EndsWith(".local", StringComparison.OrdinalIgnoreCase) || !uri.Host.Contains('.')
            || System.Net.IPAddress.TryParse(uri.Host, out _)) throw new ArgumentException("Use a public HTTPS image URL.");
        return uri.AbsoluteUri;
    }
}
