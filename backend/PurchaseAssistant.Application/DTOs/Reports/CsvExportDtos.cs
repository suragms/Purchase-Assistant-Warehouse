using PurchaseAssistant.Application.DTOs;
namespace PurchaseAssistant.Application.DTOs.Reports;

// Projections only: missing historical source fields are not reconstructed from present-day values.
public record PurchaseCsvLine(Guid PurchaseId, string OrderNumber, Guid SupplierId, string Supplier,
    Guid ItemId, string Item, [property: OperationalNumeric] decimal Quantity, string Unit,
    [property: FinancialField] decimal UnitPrice, [property: OperationalNumeric] decimal? KgPerUnit,
    [property: FinancialField] decimal? PerKgRate, [property: FinancialField] decimal LineTotal);
public record SupplierCsvRow(string Supplier, [property: OperationalNumeric] decimal Bags,
    [property: OperationalNumeric] decimal? BagKg, [property: FinancialField] decimal Amount);
public record ItemCsvRow(string Item, [property: OperationalNumeric] decimal? Kg, [property: OperationalNumeric] decimal Bags,
    [property: OperationalNumeric] decimal Boxes, [property: OperationalNumeric] decimal Tins, [property: FinancialField] decimal Amount, int Purchases);
