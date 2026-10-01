using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PurchaseAssistant.Application.DTOs.AI;
using PurchaseAssistant.Application.DTOs.Purchase;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Application.Interfaces.AI;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Domain.Enums;
using PurchaseAssistant.Infrastructure.Data;
using PurchaseAssistant.Infrastructure.Services;
using PurchaseAssistant.Infrastructure.Services.AI;
using PurchaseAssistant.UnitTests.Services;

namespace PurchaseAssistant.UnitTests.AI;

public class Phase3SafetyTests : IDisposable
{
    private readonly AppDbContext db;
    private readonly StubCurrentUserService user;
    private readonly Mock<IAIRoutingService> routing = new();
    private readonly Mock<IStockService> stock = new(MockBehavior.Strict);
    private readonly PurchaseParsingService parser;
    private readonly PurchaseService purchases;
    private readonly Guid tenant = Guid.NewGuid();
    private readonly Guid foreignTenant = Guid.NewGuid();
    private readonly Supplier supplier;
    private readonly Supplier foreignSupplier;
    private readonly CatalogItem item;
    private readonly CatalogItem foreignItem;

    public Phase3SafetyTests()
    {
        db = new(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options,
            new StubTenantProvider { BusinessId = tenant });
        user = new() { BusinessId = tenant, UserId = Guid.NewGuid() };
        supplier = new() { BusinessId = tenant, Name = "Supplier A" };
        foreignSupplier = new() { BusinessId = foreignTenant, Name = "Foreign Supplier" };
        var category = new Category { BusinessId = tenant, Name = "Food" };
        var foreignCategory = new Category { BusinessId = foreignTenant, Name = "Other" };
        item = new() { BusinessId = tenant, Name = "Rice", ItemCode = "R1", Category = category, CurrentStock = 20 };
        foreignItem = new() { BusinessId = foreignTenant, Name = "Foreign Rice", ItemCode = "F1", Category = foreignCategory, CurrentStock = 100 };
        db.AddRange(supplier, foreignSupplier, category, foreignCategory, item, foreignItem);
        db.SaveChanges();
        var normalization = new EntityNormalizationService();
        parser = new(routing.Object, new CatalogService(db, normalization, user), new SupplierService(db, normalization, user), NullLogger<PurchaseParsingService>.Instance);
        purchases = new(db, user, stock.Object);
    }
    private void Respond(object value) => routing.Setup(r => r.ExecuteWithFailoverAsync(It.IsAny<AIRequest>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(new AIResponse(true, JsonSerializer.Serialize(value), null, "Mock", "Mock", 0));
    private UpsertPurchaseOrderDto Order(decimal quantity = 1.25m, decimal price = 2m) => new()
    {
        SupplierId = supplier.Id,
        Items = new() { new() { CatalogItemId = item.Id, OrderedQuantity = quantity, UnitPrice = price } }
    };

    [Fact]
    public async Task Parse_IsReadOnly_AndStripsUntrustedFields()
    {
        Respond(new { SupplierName = supplier.Name, BusinessId = foreignTenant, Status = "Success", TaxTotal = 999,
            PaymentState = "Paid", Items = new[] { new { CatalogItemName = "Rice", RequestedQuantity = 1.25m,
                CatalogItemId = foreignItem.Id, UnitPrice = 999, CurrentStock = 999, IsAmbiguous = true,
                Options = new[] { new { CatalogItemId = foreignItem.Id, Name = "forged" } } } } });
        var result = await parser.ParseAsync("Buy rice");
        Assert.Equal(IntentStatus.Success, result.Status);
        Assert.Equal(supplier.Id, result.SupplierId);
        Assert.Equal(item.Id, result.Items.Single().CatalogItemId);
        Assert.Null(result.Items[0].Options);
        Assert.Empty(db.Purchases); Assert.Empty(db.StockMovements);
        Assert.Equal(2, db.CatalogItems.IgnoreQueryFilters().Count());
        Assert.Equal(2, db.Suppliers.IgnoreQueryFilters().Count());
        Assert.Equal(20, item.CurrentStock);
        Assert.False(db.ChangeTracker.HasChanges());
        stock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("Unknown")]
    [InlineData("Foreign Supplier")]
    [InlineData(null)]
    public async Task MissingOrForeignSupplier_ClearsForgedIds_StillValidatesItems(string? name)
    {
        Respond(new { SupplierName = name, SupplierId = foreignSupplier.Id,
            Items = new[] { new { CatalogItemName = "Foreign Rice", CatalogItemId = foreignItem.Id, RequestedQuantity = 1 } } });
        var result = await parser.ParseAsync("request");
        Assert.Null(result.SupplierId); Assert.Null(result.Items[0].CatalogItemId);
        Assert.Empty(result.Items[0].Options!);
        Assert.Equal(IntentStatus.MissingInformation, result.Status);
    }

    [Fact]
    public async Task DuplicateSupplierNames_AreNeverAutoSelected()
    {
        db.Suppliers.Add(new() { BusinessId = tenant, Name = supplier.Name }); await db.SaveChangesAsync();
        Respond(new { SupplierName = supplier.Name, Items = new[] { new { ItemCode = "R1", RequestedQuantity = 1 } } });
        var result = await parser.ParseAsync("request");
        Assert.Null(result.SupplierId); Assert.Contains(result.Warnings!, w => w.Contains("ambiguous"));
    }

    [Theory]
    [InlineData("Ri", true)]
    [InlineData("unknown", false)]
    [InlineData("", false)]
    public async Task NonExactOrMissingItem_IsNeverSilentlySubstituted(string name, bool ambiguous)
    {
        Respond(new { SupplierName = supplier.Name, Items = new[] { new { CatalogItemName = name, CatalogItemId = foreignItem.Id, RequestedQuantity = 1 } } });
        var result = await parser.ParseAsync("request");
        Assert.Null(result.Items[0].CatalogItemId); Assert.Equal(ambiguous, result.Items[0].IsAmbiguous);
    }

    [Fact]
    public async Task DuplicateCatalogNames_RequireChoice()
    {
        db.CatalogItems.Add(new() { BusinessId = tenant, Name = "Rice", ItemCode = "R2", CategoryId = item.CategoryId }); await db.SaveChangesAsync();
        Respond(new { SupplierName = supplier.Name, Items = new[] { new { CatalogItemName = "Rice", RequestedQuantity = 1 } } });
        var result = await parser.ParseAsync("request");
        Assert.Equal(IntentStatus.AmbiguousMatch, result.Status);
        Assert.Null(result.Items[0].CatalogItemId); Assert.Equal(2, result.Items[0].Options!.Count);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("0.00001")]
    [InlineData("100000000000000")]
    public async Task InvalidQuantities_AreFlaggedAndRejectedOnCreateAndUpdate(string value)
    {
        var quantity = decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        Respond(new { SupplierName = supplier.Name, Items = new[] { new { ItemCode = "R1", RequestedQuantity = quantity } } });
        Assert.Equal(IntentStatus.MissingInformation, (await parser.ParseAsync("request")).Status);
        await Assert.ThrowsAsync<ArgumentException>(() => purchases.CreatePurchaseOrderAsync(Order(quantity)));
        var created = await purchases.CreatePurchaseOrderAsync(Order());
        await Assert.ThrowsAsync<ArgumentException>(() => purchases.UpdatePurchaseOrderAsync(created.Id, Order(quantity)));
        Assert.Equal(1.25m, (await purchases.GetPurchaseOrderByIdAsync(created.Id)).Items[0].OrderedQuantity);
        stock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("null")]
    [InlineData("{}")]
    [InlineData("{\"Items\":null}")]
    [InlineData("{\"Items\":[null]}")]
    [InlineData("{\"Items\":[{\"RequestedQuantity\":\"NaN\"}]}")]
    [InlineData("not-json")]
    public async Task MalformedResponses_AreControlled(string json)
    {
        routing.Setup(r => r.ExecuteWithFailoverAsync(It.IsAny<AIRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AIResponse(true, json, null, "Mock", "Mock", 0));
        var result = await parser.ParseAsync("request");
        Assert.Equal(IntentStatus.Error, result.Status); Assert.Empty(result.Items);
    }

    [Theory]
    [InlineData("AI_DISABLED", "AI_DISABLED")]
    [InlineData("secret-provider-internals", "AI_UNAVAILABLE")]
    public async Task ProviderFailures_DoNotLeakDetails(string error, string expected)
    {
        routing.Setup(r => r.ExecuteWithFailoverAsync(It.IsAny<AIRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AIResponse(false, null, error, "Mock", "Mock", 0));
        Assert.Equal(expected, (await parser.ParseAsync("request")).Message);
    }

    [Fact]
    public async Task ManualPurchase_ComputesTotalsAndDraftState_WithoutStockChanges()
    {
        var order = Order();
        order.Items[0].TaxPercent = 40m;
        var result = await purchases.CreatePurchaseOrderAsync(order);
        Assert.Equal(2.5m, result.Subtotal); Assert.Equal(1m, result.TaxTotal); Assert.Equal(3.5m, result.GrandTotal);
        Assert.Equal(PurchaseStatus.Draft, result.Status); Assert.Equal(PaymentState.Pending, result.PaymentState);
        Assert.Equal(DeliveryState.Pending, result.DeliveryState); Assert.Equal(20, item.CurrentStock);
        stock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task FinalPurchase_RevalidatesTenantIds()
    {
        var dto = Order(); dto.SupplierId = foreignSupplier.Id;
        await Assert.ThrowsAsync<ArgumentException>(() => purchases.CreatePurchaseOrderAsync(dto));
        dto = Order(); dto.Items[0].CatalogItemId = foreignItem.Id;
        await Assert.ThrowsAsync<ArgumentException>(() => purchases.CreatePurchaseOrderAsync(dto));
        Assert.Empty(db.Purchases); stock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(-1, 1, 0)]
    [InlineData(1, -1, 0)]
    [InlineData(99999999999999, 2, 0)]
    [InlineData(1, 1, -1)]
    public async Task InvalidFinancialInputs_CannotPersist(double quantity, double price, double tax)
    {
        var dto = Order((decimal)quantity, (decimal)price); dto.Items[0].TaxPercent = (decimal)tax;
        await Assert.ThrowsAsync<ArgumentException>(() => purchases.CreatePurchaseOrderAsync(dto));
        Assert.Empty(db.Purchases);
    }

    [Fact]
    public async Task RetryWithSameOrderNumber_DoesNotCreateDuplicate()
    {
        var dto = Order(); dto.OrderNumber = "PO-retry";
        await purchases.CreatePurchaseOrderAsync(dto);
        await Assert.ThrowsAsync<InvalidOperationException>(() => purchases.CreatePurchaseOrderAsync(dto));
        Assert.Single(db.Purchases);
    }
    public void Dispose() => db.Dispose();
}
