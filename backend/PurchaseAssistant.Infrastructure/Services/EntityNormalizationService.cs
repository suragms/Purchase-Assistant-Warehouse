using PurchaseAssistant.Application.Interfaces;
using System.Text.RegularExpressions;

namespace PurchaseAssistant.Infrastructure.Services
{
    public class EntityNormalizationService : IEntityNormalizationService
    {
        public string NormalizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return string.Empty;
            return name.Trim().ToLowerInvariant();
        }

        public string NormalizeItemCode(string itemCode)
        {
            if (string.IsNullOrWhiteSpace(itemCode)) return string.Empty;
            return Regex.Replace(itemCode.Trim().ToUpperInvariant(), @"\s+", "");
        }

        public string? NormalizeBarcode(string? barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode)) return null;
            return barcode.Trim();
        }

        public string NormalizePhone(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return string.Empty;
            return Regex.Replace(phone.Trim(), @"[^\d+]", "");
        }
    }
}
