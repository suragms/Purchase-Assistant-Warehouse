using System;

namespace PurchaseAssistant.Application.Interfaces
{
    public interface IEntityNormalizationService
    {
        string NormalizeName(string name);
        string NormalizeItemCode(string itemCode);
        string? NormalizeBarcode(string? barcode);
        string NormalizePhone(string? phone);
    }
}
