using SkiaSharp;
namespace PurchaseAssistant.Infrastructure.Services;
public static class LogoImageValidator
{
    public const int MaxBytes = 2 * 1024 * 1024;
    public static byte[] ValidateAndNormalize(byte[] bytes, string mime, string filename)
    {
        if (bytes.Length is 0 or > MaxBytes || string.IsNullOrWhiteSpace(filename) || filename.Length > 255 || filename.Any(c => char.IsControl(c) || c is '/' or '\\' or ':') || filename.Contains(".."))
            throw new ArgumentException("Invalid image size or filename.");
        var extension = Path.GetExtension(filename).ToLowerInvariant();
        using var data = SKData.CreateCopy(bytes); using var codec = SKCodec.Create(data);
        if (codec == null || codec.Info.Width < 1 || codec.Info.Height < 1 || codec.Info.Width > 4096 || codec.Info.Height > 4096 || (long)codec.Info.Width * codec.Info.Height > 16000000 || codec.FrameCount > 1)
            throw new ArgumentException("Invalid image or unsupported dimensions.");
        var valid = codec.EncodedFormat switch {
            SKEncodedImageFormat.Png => mime == "image/png" && extension == ".png",
            SKEncodedImageFormat.Jpeg => mime == "image/jpeg" && extension is ".jpg" or ".jpeg",
            SKEncodedImageFormat.Webp => mime == "image/webp" && extension == ".webp", _ => false };
        if (!valid) throw new ArgumentException("Image content, type and extension must match JPEG, PNG or WebP.");
        using var bitmap = new SKBitmap(codec.Info.Width, codec.Info.Height);
        if (codec.GetPixels(bitmap.Info, bitmap.GetPixels()) != SKCodecResult.Success) throw new ArgumentException("Image is malformed or incomplete.");
        using var image = SKImage.FromBitmap(bitmap); using var png = image.Encode(SKEncodedImageFormat.Png, 100);
        return png.ToArray(); // Strip filenames, metadata and appended executable content.
    }
}
