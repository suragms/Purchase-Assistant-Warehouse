namespace PurchaseAssistant.Application.DTOs.Purchase;

public static class PurchaseInputLimits
{
    // Existing database columns are numeric(18,4).
    public const decimal MaxValue = 99999999999999m;
    public static bool IsQuantityValid(decimal value) =>
        value > 0 && value <= MaxValue && decimal.Round(value, 4) == value;

    public static void Validate(UpsertPurchaseOrderDto dto)
    {
        if (dto.PaymentDays is < 0 or > 3650)
            throw new ArgumentException("Payment terms must be between 0 and 3650 days.");
        if (dto.Items == null || dto.Items.Count == 0 || dto.Items.Count > 200)
            throw new ArgumentException("Provide between 1 and 200 purchase items.");
        foreach (var value in new[] { dto.HeaderDiscountPercent, dto.FreightAmount, dto.DeliveredCharge, dto.BilltyCharge, dto.CommissionPercent, dto.CommissionAmount })
            ValidateMoney(value);
        if (dto.HeaderDiscountPercent > 100 || dto.CommissionPercent > 100 || dto.FreightType is not ("included" or "separate")
            || dto.CommissionMode is not ("percent" or "flat_invoice" or "flat_kg" or "flat_bag" or "flat_box" or "flat_tin")) throw new ArgumentException("Invalid purchase charges.");
        decimal total = 0;
        foreach (var item in dto.Items)
        {
            if (item == null || !IsQuantityValid(item.OrderedQuantity))
                throw new ArgumentException("Quantity must be positive, within range, and have at most four decimal places.");
            if (item.UnitPrice < 0 || item.UnitPrice > MaxValue || decimal.Round(item.UnitPrice, 4) != item.UnitPrice)
                throw new ArgumentException("Invalid unit price.");
            if (item.DiscountPercent < 0 || item.DiscountPercent > 100 || decimal.Round(item.DiscountPercent, 2) != item.DiscountPercent
                || item.TaxPercent < 0 || item.TaxPercent > 1000 || decimal.Round(item.TaxPercent, 2) != item.TaxPercent)
                throw new ArgumentException("Enter a discount from 0 to 100% and tax from 0 to 1000%, with at most two decimal places.");
            foreach (var value in new[] { item.FreightAmount, item.DeliveredCharge, item.BilltyCharge }) if (value.HasValue) ValidateMoney(value.Value);
            if (item.FreightType is not ("included" or "separate") || string.IsNullOrWhiteSpace(item.Unit) || item.Unit.Length > 32) throw new ArgumentException("Invalid line charge or unit.");
            // Both operands are bounded first, so decimal multiplication cannot overflow.
            total += LineTotal(item);
            if (total > MaxValue)
                throw new ArgumentException("Purchase total exceeds the supported range.");
        }
    }

    public static decimal LineTotal(UpsertPurchaseItemDto item)
    {
        var amount = LineMoney(item);
        if (amount > MaxValue) throw new ArgumentException("Purchase line total exceeds the supported range.");
        return decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
    }

    public static decimal LineSubtotal(UpsertPurchaseItemDto item) =>
        decimal.Round(LineGross(item) * (1 - item.DiscountPercent / 100), 2, MidpointRounding.AwayFromZero);

    public static decimal LineTax(UpsertPurchaseItemDto item) => LineTotal(item) - LineSubtotal(item);

    private static void ValidateMoney(decimal value)
    {
        if (value < 0 || value > MaxValue || decimal.Round(value, 2) != value) throw new ArgumentException("Amounts must be nonnegative with at most two decimal places.");
    }
    private static decimal LineMoney(UpsertPurchaseItemDto item) => LineGross(item) * (1 - item.DiscountPercent / 100) * (1 + item.TaxPercent / 100);
    public static decimal LineCharges(UpsertPurchaseItemDto i) => (i.FreightType == "separate" ? i.FreightAmount ?? 0 : 0) + (i.DeliveredCharge ?? 0) + (i.BilltyCharge ?? 0);
    public static decimal GrandTotal(UpsertPurchaseOrderDto dto)
    {
        var sum = dto.Items.Sum(i => LineMoney(i) + LineCharges(i));
        var afterDiscount = sum * (1 - dto.HeaderDiscountPercent / 100m);
        var lineCharges = dto.Items.Any(i => i.FreightAmount.HasValue || i.DeliveredCharge.HasValue || i.BilltyCharge.HasValue);
        var commission = dto.CommissionMode switch {
            "percent" => afterDiscount * dto.CommissionPercent / 100m,
            "flat_invoice" => dto.CommissionAmount,
            "flat_kg" => dto.CommissionAmount * dto.Items.Sum(i => i.Unit.Equals("KG", StringComparison.OrdinalIgnoreCase) ? i.OrderedQuantity : i.OrderedQuantity * (i.KgPerUnit ?? 0)),
            "flat_bag" => dto.CommissionAmount * dto.Items.Where(i => i.Unit.ToLowerInvariant() is "bag" or "sack").Sum(i => i.OrderedQuantity),
            "flat_box" => dto.CommissionAmount * dto.Items.Where(i => i.Unit.Equals("BOX", StringComparison.OrdinalIgnoreCase)).Sum(i => i.OrderedQuantity),
            "flat_tin" => dto.CommissionAmount * dto.Items.Where(i => i.Unit.Equals("TIN", StringComparison.OrdinalIgnoreCase)).Sum(i => i.OrderedQuantity), _ => 0m };
        var total = afterDiscount + decimal.Round(commission, 2, MidpointRounding.AwayFromZero)
            + (lineCharges ? 0 : (dto.FreightType == "included" ? 0 : dto.FreightAmount) + dto.DeliveredCharge + dto.BilltyCharge);
        if (total < 0 || total > MaxValue) throw new ArgumentException("Purchase total exceeds supported bounds.");
        return decimal.Round(total, 2, MidpointRounding.AwayFromZero);
    }
    private static decimal LineGross(UpsertPurchaseItemDto item)
    {
        if (item.KgPerUnit.HasValue != item.LandingCostPerKg.HasValue)
            throw new ArgumentException("Enter both kg per unit and landing cost per kg for a weight-priced line.");
        var unitCost = item.UnitPrice;
        if (item.KgPerUnit is decimal kg && item.LandingCostPerKg is decimal rate)
        {
            if (!IsQuantityValid(kg) || !IsQuantityValid(rate))
                throw new ArgumentException("Weight pricing fields must be positive, within range, and have at most four decimal places.");
            var derived = kg * rate;
            if (derived > MaxValue) throw new ArgumentException("Weight unit cost exceeds the supported range.");
            // Exact reference line_gross_base rule: inconsistent snapshots fall back to manual unit landing cost.
            if (Math.Abs(derived - unitCost) <= 0.05m) unitCost = derived;
        }
        var amount = item.OrderedQuantity * unitCost;
        if (amount > MaxValue) throw new ArgumentException("Purchase line total exceeds the supported range.");
        return amount;
    }
}
