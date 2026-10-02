using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Moq;
using PurchaseAssistant.Application.DTOs.Catalog;
using PurchaseAssistant.Application.DTOs.Purchase;
using PurchaseAssistant.Application.DTOs.Stock;
using PurchaseAssistant.Application.DTOs.Users;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Domain.Enums;
using PurchaseAssistant.Infrastructure.Data;
using PurchaseAssistant.Infrastructure.Services;
using PurchaseAssistant.Web.Authorization;
using PurchaseAssistant.Web.Services;

namespace PurchaseAssistant.UnitTests.Services;

public class ReferenceAuditSafetyTests : IDisposable
{
    private readonly Guid tenant = Guid.NewGuid(), foreignTenant = Guid.NewGuid(), sessionId = Guid.NewGuid();
    private readonly AppDbContext db;
    private readonly Actor actor;
    private readonly Supplier supplier, foreignSupplier;
    private readonly Category category, foreignCategory;
    private readonly CatalogItem item;
    private readonly Mock<IStockService> stock = new(MockBehavior.Strict);
    private PurchaseService Purchases => new(db, actor, stock.Object);

    public ReferenceAuditSafetyTests()
    {
        actor = new Actor { BusinessId = tenant, UserId = Guid.NewGuid() };
        db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options,
            new StubTenantProvider { BusinessId = tenant });
        category = new() { BusinessId = tenant, Name = "Food" };
        foreignCategory = new() { BusinessId = foreignTenant, Name = "Other food" };
        supplier = new() { BusinessId = tenant, Name = "Local supplier" };
        foreignSupplier = new() { BusinessId = foreignTenant, Name = "Foreign supplier" };
        item = new() { BusinessId = tenant, CategoryId = category.Id, Name = "Rice", ItemCode = "R1", CurrentStock = 20, RowVersion = Guid.NewGuid() };
        db.AddRange(new Business { Id = tenant, Name = "Here", IsActive = true }, new Business { Id = foreignTenant, Name = "There", IsActive = true },
            new User { Id = actor.UserId!.Value, Name = "Staff", Email = "audit@test.local", Status = UserStatus.Active },
            new Membership { BusinessId = tenant, UserId = actor.UserId.Value, Role = Role.Staff, PermissionsJson = "[\"stock.view\"]" },
            category, foreignCategory, supplier, foreignSupplier, item,
            new RefreshToken { UserId = actor.UserId.Value, FamilyId = sessionId, TokenHash = "fixture-only", ExpiresAt = DateTime.UtcNow.AddDays(1) });
        db.SaveChanges();
    }

    public void Dispose() => db.Dispose();
    private UpsertPurchaseOrderDto Input() => new() { SupplierId = supplier.Id,
        Items = [new() { CatalogItemId = item.Id, OrderedQuantity = 10, UnitPrice = 4 }] };
    private async Task<PurchaseOrderDto> Draft() => await Purchases.CreatePurchaseOrderAsync(Input());

    [Theory]
    [InlineData(0, PaymentState.Pending)]
    [InlineData(10, PaymentState.Partial)]
    [InlineData(40, PaymentState.Paid)]
    [InlineData(100, PaymentState.Paid)]
    public async Task ExplicitPaymentDerivesBalanceAndStateWithoutStockOrDeliveryChanges(decimal paid, PaymentState state)
    {
        actor.Role = "Owner"; var order = await Draft();
        await Purchases.UpdateStatusAsync(order.Id, PurchaseStatus.Confirmed);
        var result = await Purchases.UpdatePaymentAsync(order.Id, new() { PaidAmount = paid, ExpectedVersion = order.Version });
        Assert.Equal(Math.Min(paid, 40), result.PaidAmount); Assert.Equal(40 - result.PaidAmount, result.RemainingAmount);
        Assert.Equal(state, result.PaymentState); Assert.Equal(PurchaseStatus.Confirmed, result.Status);
        Assert.Equal(DeliveryState.Pending, result.DeliveryState); Assert.NotNull(result.PaidAt);
        Assert.Contains(await Purchases.GetActivityAsync(order.Id), a => a.EventType == "PurchasePaymentUpdated");
        Assert.Equal(20, item.CurrentStock); stock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(0, 0, PaymentState.DueSoon)]
    [InlineData(3, 0, PaymentState.DueSoon)]
    [InlineData(4, 0, PaymentState.Partial)]
    [InlineData(0, -1, PaymentState.Overdue)]
    public async Task PaymentDueStatesMatchReferencePriority(int days, int createdOffset, PaymentState expected)
    {
        actor.Role = "Owner"; var input = Input(); input.PaymentDays = days;
        var order = await Purchases.CreatePurchaseOrderAsync(input);
        var stored = await db.Purchases.SingleAsync(); stored.CreatedAt = DateTime.UtcNow.Date.AddDays(createdOffset); await db.SaveChangesAsync();
        await Purchases.UpdateStatusAsync(order.Id, PurchaseStatus.Confirmed);
        var paid = await Purchases.UpdatePaymentAsync(order.Id, new() { PaidAmount = 10, ExpectedVersion = order.Version });
        Assert.Equal(expected, paid.PaymentState);
        Assert.Equal(DateOnly.FromDateTime(stored.CreatedAt).AddDays(days), paid.DueDate);
        var settled = await Purchases.UpdatePaymentAsync(order.Id, new() { PaidAmount = 40, ExpectedVersion = order.Version });
        Assert.Equal(PaymentState.Paid, settled.PaymentState);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0.00001)]
    [InlineData(100000000000000)]
    public async Task InvalidPaymentAmountCannotWrite(decimal paid)
    {
        actor.Role = "Owner"; var order = await Draft(); await Purchases.UpdateStatusAsync(order.Id, PurchaseStatus.Confirmed);
        await Assert.ThrowsAsync<ArgumentException>(() => Purchases.UpdatePaymentAsync(order.Id, new() { PaidAmount = paid, ExpectedVersion = order.Version }));
        Assert.Equal(0, (await db.Purchases.SingleAsync()).PaidAmount);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DraftAndCancelledPurchasesRejectPayment(bool cancelled)
    {
        actor.Role = "Owner"; var order = await Draft();
        if (cancelled) await Purchases.UpdateStatusAsync(order.Id, PurchaseStatus.Cancelled);
        await Assert.ThrowsAsync<InvalidOperationException>(() => Purchases.UpdatePaymentAsync(order.Id, new() { PaidAmount = 10, ExpectedVersion = order.Version }));
        Assert.Equal(0, (await db.Purchases.SingleAsync()).PaidAmount); stock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task PaymentRejectsNonownerForeignPurchaseMissingAndStaleVersions()
    {
        var order = await Draft(); await Purchases.UpdateStatusAsync(order.Id, PurchaseStatus.Confirmed);
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Purchases.UpdatePaymentAsync(order.Id, new() { PaidAmount = 10, ExpectedVersion = order.Version }));
        actor.Role = "Owner";
        await Assert.ThrowsAsync<ArgumentException>(() => Purchases.UpdatePaymentAsync(order.Id, new() { PaidAmount = 10 }));
        await Assert.ThrowsAsync<InvalidOperationException>(() => Purchases.UpdatePaymentAsync(order.Id, new() { PaidAmount = 10, ExpectedVersion = order.Version + 1 }));
        var other = new PurchaseOrder { BusinessId = foreignTenant, SupplierId = foreignSupplier.Id, OrderNumber = "FOREIGN", Status = PurchaseStatus.Confirmed };
        db.Purchases.Add(other); await db.SaveChangesAsync();
        await Assert.ThrowsAsync<KeyNotFoundException>(() => Purchases.UpdatePaymentAsync(other.Id, new() { PaidAmount = 10, ExpectedVersion = other.Version }));
        Assert.Equal(0, other.PaidAmount); Assert.Equal(0, (await db.Purchases.SingleAsync(p => p.Id == order.Id)).PaidAmount);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(3651)]
    public async Task InvalidPaymentTermsCannotCreatePurchase(int days)
    {
        var input = Input(); input.PaymentDays = days;
        await Assert.ThrowsAsync<ArgumentException>(() => Purchases.CreatePurchaseOrderAsync(input));
        Assert.Empty(await db.Purchases.ToListAsync());
    }

    [Fact]
    public async Task VariantsCanBeCreatedReadUpdatedAndDeletedWithoutStockChanges()
    {
        actor.Role = "Owner";
        var service = new CatalogService(db, new EntityNormalizationService(), actor);
        var created = await service.CreateVariantAsync(item.Id, new() { Name = " Small bag ", KgPerUnit = 5 });
        Assert.Equal("Small bag", created.Name); Assert.Equal(5, created.KgPerUnit);
        Assert.Single((await service.GetByIdAsync(item.Id))!.Variants);
        created.Name = "Large bag"; created.KgPerUnit = 25;
        var updated = await service.UpdateVariantAsync(item.Id, created.Id, created);
        Assert.NotEqual(created.RowVersion, updated.RowVersion);
        Assert.Equal(25, (await service.GetVariantsAsync(item.Id))[0].KgPerUnit);
        await service.DeleteVariantAsync(item.Id, updated.Id, updated.RowVersion);
        Assert.Empty(await service.GetVariantsAsync(item.Id)); Assert.Equal(20, item.CurrentStock);
        Assert.Empty(await db.StockMovements.ToListAsync());
    }

    [Theory]
    [InlineData("bag")]
    [InlineData(" BAG ")]
    public async Task VariantDuplicateNamesAreCaseAndWhitespaceInsensitive(string duplicate)
    {
        var service = new CatalogService(db, new EntityNormalizationService(), actor);
        await service.CreateVariantAsync(item.Id, new() { Name = "Bag" });
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateVariantAsync(item.Id, new() { Name = duplicate }));
        Assert.Single(await db.CatalogVariants.ToListAsync());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(0.00001)]
    public async Task VariantInvalidWeightDoesNotWrite(decimal weight)
    {
        var service = new CatalogService(db, new EntityNormalizationService(), actor);
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateVariantAsync(item.Id, new() { Name = "Bag", KgPerUnit = weight }));
        Assert.Empty(await db.CatalogVariants.ToListAsync());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task VariantRequiresTrimmedNonemptyName(string name)
    {
        var service = new CatalogService(db, new EntityNormalizationService(), actor);
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateVariantAsync(item.Id, new() { Name = name }));
        Assert.Empty(await db.CatalogVariants.ToListAsync());
    }

    [Fact]
    public async Task VariantForeignTrackedRowsAndWrongParentAreIsolated()
    {
        var other = new CatalogItem { BusinessId = foreignTenant, CategoryId = foreignCategory.Id, Name = "Other", ItemCode = "OTHER" };
        var foreign = new CatalogVariant { BusinessId = foreignTenant, CatalogItemId = other.Id, Name = "Foreign" };
        db.AddRange(other, foreign); await db.SaveChangesAsync();
        var service = new CatalogService(db, new EntityNormalizationService(), actor);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetVariantsAsync(other.Id));
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.CreateVariantAsync(other.Id, new() { Name = "Bad" }));
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateVariantAsync(item.Id, foreign.Id, new() { Name = "Changed", RowVersion = foreign.RowVersion }));
        actor.Role = "Owner";
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.DeleteVariantAsync(item.Id, foreign.Id, foreign.RowVersion));
        Assert.Equal("Foreign", foreign.Name);
    }

    [Fact]
    public async Task VariantStaleWritesAndNonownerDeletionFail()
    {
        var service = new CatalogService(db, new EntityNormalizationService(), actor);
        var original = await service.CreateVariantAsync(item.Id, new() { Name = "Bag" });
        await service.UpdateVariantAsync(item.Id, original.Id, new() { Name = "Updated", RowVersion = original.RowVersion });
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateVariantAsync(item.Id, original.Id, original));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.DeleteVariantAsync(item.Id, original.Id, original.RowVersion));
        actor.Role = "Owner";
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteVariantAsync(item.Id, original.Id, original.RowVersion));
        Assert.Equal("Updated", (await service.GetVariantsAsync(item.Id))[0].Name);
    }

    [Fact]
    public async Task VariantWeightUsesSchemaRangeWithoutAnInventedBusinessMaximum()
    {
        var service = new CatalogService(db, new EntityNormalizationService(), actor);
        var variant = await service.CreateVariantAsync(item.Id, new() { Name = "Weighted", KgPerUnit = 2000000 });
        Assert.Equal(2000000, variant.KgPerUnit);
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateVariantAsync(item.Id, new() { Name = "Overflow", KgPerUnit = 100000000000000 }));
        Assert.Single(await service.GetVariantsAsync(item.Id));
    }

    [Fact]
    public async Task ArchivePreservesItemStockAndPurchaseHistoryAndRejectsStaleWrite()
    {
        await Draft(); item.IsActive = true; await db.SaveChangesAsync();
        var originalVersion = item.RowVersion;
        var service = new CatalogService(db, new EntityNormalizationService(), actor);
        await service.ArchiveAsync(item.Id, originalVersion);
        Assert.False(item.IsActive); Assert.Equal(20, item.CurrentStock); Assert.Single(await db.PurchaseItems.ToListAsync());
        Assert.Single(await db.CatalogItems.ToListAsync()); Assert.Empty(await db.StockMovements.ToListAsync());
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.ArchiveAsync(item.Id, originalVersion));
    }

    [Fact]
    public async Task CatalogCreateRejectsForeignCategory()
    {
        var service = new CatalogService(db, new EntityNormalizationService(), actor);
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(new CatalogItemDto { Name = "Bad", ItemCode = "BAD", CategoryId = foreignCategory.Id }));
        Assert.Single(await db.CatalogItems.ToListAsync());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CatalogRejectsForeignOrMismatchedType(bool foreign)
    {
        var type = new CategoryType { BusinessId = foreign ? foreignTenant : tenant, CategoryId = foreignCategory.Id, Name = "Other type" };
        db.CategoryTypes.Add(type); await db.SaveChangesAsync();
        var service = new CatalogService(db, new EntityNormalizationService(), actor);
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(new CatalogItemDto { Name = "Bad", ItemCode = "BAD", CategoryId = category.Id, TypeId = type.Id }));
    }

    [Theory]
    [InlineData(PurchaseStatus.Completed)]
    [InlineData(PurchaseStatus.Verified)]
    [InlineData(PurchaseStatus.Arrived)]
    [InlineData(PurchaseStatus.Dispatched)]
    [InlineData((PurchaseStatus)999)]
    public async Task DraftCannotSkipLifecycle(PurchaseStatus status)
    {
        var draft = await Draft();
        await Assert.ThrowsAsync<InvalidOperationException>(() => Purchases.UpdateStatusAsync(draft.Id, status));
        Assert.Equal(PurchaseStatus.Draft, (await db.Purchases.SingleAsync()).Status);
        stock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task PurchaseVersionRejectsStaleStatus()
    {
        var draft = await Draft();
        await Assert.ThrowsAsync<InvalidOperationException>(() => Purchases.UpdateStatusAsync(draft.Id, PurchaseStatus.Confirmed, draft.Version + 1));
        Assert.Equal(PurchaseStatus.Draft, (await db.Purchases.SingleAsync()).Status);
    }

    [Fact]
    public async Task ForeignPurchaseAndSupplierAreIsolated()
    {
        var input = Input(); input.SupplierId = foreignSupplier.Id;
        await Assert.ThrowsAsync<ArgumentException>(() => Purchases.CreatePurchaseOrderAsync(input));
        var other = new PurchaseOrder { BusinessId = foreignTenant, SupplierId = foreignSupplier.Id, OrderNumber = "OTHER" };
        db.Purchases.Add(other); await db.SaveChangesAsync();
        await Assert.ThrowsAsync<KeyNotFoundException>(() => Purchases.GetPurchaseOrderByIdAsync(other.Id));
        await Assert.ThrowsAsync<KeyNotFoundException>(() => Purchases.UpdateStatusAsync(other.Id, PurchaseStatus.Confirmed));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(0.00001)]
    public async Task ReceivingInvalidQuantityDoesNotTouchStock(decimal quantity)
    {
        var draft = await Draft();
        await Assert.ThrowsAsync<ArgumentException>(() => Purchases.ReceiveItemsAsync(draft.Id, new() {
            Items = [new() { PurchaseItemId = draft.Items[0].Id, ReceivedQuantityDelta = quantity }] }));
        stock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task DuplicateOrExcessReceiptDoesNotTouchStock(bool duplicate)
    {
        var draft = await Draft();
        await Purchases.UpdateStatusAsync(draft.Id, PurchaseStatus.Confirmed);
        await Purchases.UpdateStatusAsync(draft.Id, PurchaseStatus.Dispatched);
        await Purchases.UpdateStatusAsync(draft.Id, PurchaseStatus.Arrived);
        await Purchases.UpdateStatusAsync(draft.Id, PurchaseStatus.Verified);
        var request = new ReceivePurchaseDto { Items = [new() { PurchaseItemId = draft.Items[0].Id, ReceivedQuantityDelta = duplicate ? 1 : 11 }] };
        if (duplicate) request.Items.Add(new() { PurchaseItemId = draft.Items[0].Id, ReceivedQuantityDelta = 1 });
        if (duplicate) await Assert.ThrowsAsync<ArgumentException>(() => Purchases.ReceiveItemsAsync(draft.Id, request));
        else await Assert.ThrowsAsync<InvalidOperationException>(() => Purchases.ReceiveItemsAsync(draft.Id, request));
        Assert.Equal(0, (await db.PurchaseItems.SingleAsync()).ReceivedQuantity);
        stock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0.00001)]
    public async Task PhysicalCountRejectsInvalidQuantities(decimal quantity)
    {
        var service = new StockService(db, actor);
        await Assert.ThrowsAsync<ArgumentException>(() => service.UpdatePhysicalStockAsync(item.Id, new() { PhysicalStock = quantity, ExpectedVersion = item.RowVersion }));
        Assert.Empty(await db.StockMovements.ToListAsync());
    }

    [Fact]
    public async Task ReportsExcludeDraftsAndUseRealCostForValuation()
    {
        var confirmed = await Draft(); await Purchases.UpdateStatusAsync(confirmed.Id, PurchaseStatus.Confirmed);
        var expensiveDraft = Input(); expensiveDraft.Items[0].UnitPrice = 500;
        await Purchases.CreatePurchaseOrderAsync(expensiveDraft);
        var report = new ReportService(db);
        var spend = await report.GetSpendAnalyticsAsync(tenant, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
        Assert.Equal(40m, spend.Sum(s => s.TotalSpend));
        Assert.Equal(80m, (await report.GetStockAnalyticsAsync(tenant)).EstimatedInventoryValue);
        Assert.Equal(40m, (await new DashboardService(db).GetDashboardDataAsync()).PurchaseMetrics.TotalPurchaseSpend);
    }

    [Fact]
    public async Task UnpricedStockNeverGetsInventedValue()
    {
        var report = await new ReportService(db).GetStockAnalyticsAsync(tenant);
        Assert.Equal(0m, report.EstimatedInventoryValue);
        Assert.Equal(1, report.UnpricedStockItemCount);
    }

    [Fact]
    public async Task ServerMembershipOverridesForgedOrStaleClaims()
    {
        var principal = Principal(tenant);
        ((ClaimsIdentity)principal.Identity!).AddClaim(new Claim(ClaimTypes.Role, "Owner"));
        Assert.True(await CurrentUserService.ValidateSessionAsync(principal, db));
        Assert.Equal("Staff", principal.FindFirst("role")!.Value);
        Assert.Empty(principal.FindAll(ClaimTypes.Role));
        Assert.DoesNotContain(principal.FindAll("permissions"), c => c.Value == "users.manage");
        Assert.Contains(principal.FindAll("permissions"), c => c.Value == "stock.view");
    }

    [Fact]
    public async Task SessionCannotSelectForeignMembership()
    {
        Assert.False(await CurrentUserService.ValidateSessionAsync(Principal(foreignTenant), db));
    }

    [Fact]
    public async Task BlockedUserAndDeletedMembershipInvalidateSession()
    {
        var user = await db.Users.SingleAsync(); user.Status = UserStatus.Blocked; await db.SaveChangesAsync();
        Assert.False(await CurrentUserService.ValidateSessionAsync(Principal(tenant), db));
        user.Status = UserStatus.Active; db.Memberships.RemoveRange(db.Memberships); await db.SaveChangesAsync();
        Assert.False(await CurrentUserService.ValidateSessionAsync(Principal(tenant), db));
    }

    [Fact]
    public void FinancialRedactionProtectsNestedAmountsAndPreservesQuantities()
    {
        var data = JsonNode.Parse("{\"grandTotal\":999,\"data\":[{\"unitPrice\":123,\"orderedQuantity\":4}],\"purchaseMetrics\":{\"totalPurchaseSpend\":567}}")!;
        OwnerFinancialResultFilter.Redact(data);
        Assert.Null(data["grandTotal"]);
        Assert.Null(data["data"]![0]!["unitPrice"]);
        Assert.Equal(4, data["data"]![0]!["orderedQuantity"]!.GetValue<int>());
        Assert.DoesNotContain("567", data.ToJsonString());
    }

    [Fact]
    public void EveryDecimalContractExplicitlyClassifiesFinancialOrOperationalData()
    {
        var fields = typeof(PurchaseOrderDto).Assembly.GetTypes().Where(t => t.Namespace?.StartsWith("PurchaseAssistant.Application.DTOs") == true)
            .SelectMany(t => t.GetProperties()).Where(p => (Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType) == typeof(decimal));
        Assert.NotEmpty(fields);
        Assert.All(fields, p => Assert.True(p.IsDefined(typeof(PurchaseAssistant.Application.DTOs.FinancialFieldAttribute), true)
            ^ p.IsDefined(typeof(PurchaseAssistant.Application.DTOs.OperationalNumericAttribute), true), $"Classify {p.DeclaringType?.Name}.{p.Name}."));
    }

    private record FutureResponse(decimal UnknownInvoiceAmount,
        [property: PurchaseAssistant.Application.DTOs.OperationalNumeric] decimal Quantity,
        [property: PurchaseAssistant.Application.DTOs.FinancialField] decimal CustomNamedMoney);

    [Theory]
    [InlineData(null)]
    [InlineData("http://warehouse.example")]
    [InlineData("https://*.warehouse.example")]
    [InlineData("https://warehouse.example/path")]
    [InlineData("https://user@warehouse.example")]
    public void ProductionRequiresExplicitSecureOrigins(string? origin)
    {
        var values = new Dictionary<string, string?> { ["ConnectionStrings:DefaultConnection"] = "configured externally",
            ["DataProtection:KeyRingPath"] = Path.GetFullPath("production-key-test"), ["DataProtection:CertificatePath"] = "configured externally" };
        if (origin != null) values["Cors:AllowedOrigins:0"] = origin;
        var config = new Microsoft.Extensions.Configuration.ConfigurationBuilder().AddInMemoryCollection(values).Build();
        Assert.Throws<InvalidOperationException>(() => PurchaseAssistant.Web.Services.ProductionConfiguration.Validate(config));
    }

    [Fact]
    public void ProductionRequiresPersistentKeyPathAndDoesNotExposeConfiguredSecrets()
    {
        var config = new Microsoft.Extensions.Configuration.ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> {
            ["ConnectionStrings:DefaultConnection"] = "secret-marker", ["Cors:AllowedOrigins:0"] = "https://warehouse.example" }).Build();
        var error = Assert.Throws<InvalidOperationException>(() => PurchaseAssistant.Web.Services.ProductionConfiguration.Validate(config));
        Assert.DoesNotContain("secret-marker", error.Message);
        config["DataProtection:KeyRingPath"] = Path.GetFullPath("production-key-test");
        config["DataProtection:CertificatePath"] = "configured externally";
        PurchaseAssistant.Web.Services.ProductionConfiguration.Validate(config);
    }

    [Fact]
    public void FutureUnclassifiedDecimalCannotLeakAndClassifiedQuantitiesRemainVisible()
    {
        var options = OwnerFinancialResultFilter.NonOwnerOptions(new JsonSerializerOptions(JsonSerializerDefaults.Web));
        var result = JsonSerializer.Serialize(new FutureResponse(987, 4, 987), options);
        Assert.Equal("{\"quantity\":4}", result);
    }

    [Fact]
    public async Task BusinessUserManagementCannotAssignSuperAdminOrOwner()
    {
        var service = new UserService(db, actor);
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateUserAsync(new() { Email = "bad@test.local", Role = Role.SuperAdmin }));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.CreateUserAsync(new() { Email = "bad@test.local", Role = Role.Owner }));
    }

    [Fact]
    public async Task SharedAccountCannotBeChangedFromOneTenant()
    {
        var shared = new User { Name = "Shared", Email = "shared@test.local", Status = UserStatus.Active };
        db.Users.Add(shared); db.Memberships.AddRange(new Membership { BusinessId = tenant, UserId = shared.Id, Role = Role.Staff },
            new Membership { BusinessId = foreignTenant, UserId = shared.Id, Role = Role.Staff }); await db.SaveChangesAsync();
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => new UserService(db, actor).UpdateUserAsync(shared.Id,
            new() { Name = "Changed", Email = shared.Email, Role = Role.Staff, Status = UserStatus.Blocked }));
        Assert.Equal(UserStatus.Active, shared.Status); Assert.Equal("Shared", shared.Name);
    }

    [Theory]
    [InlineData(2, 50, 3, 150, 300)]
    [InlineData(2, 50, 3, 149.97, 300)]
    [InlineData(2, 50, 3, 100, 200)]
    public async Task PreviewAndPersistenceMatchReferenceWeightRule(decimal quantity, decimal kg, decimal perKg, decimal perUnit, decimal expected)
    {
        var input = Input(); input.Items[0].OrderedQuantity = quantity; input.Items[0].UnitPrice = perUnit;
        input.Items[0].KgPerUnit = kg; input.Items[0].LandingCostPerKg = perKg;
        var preview = await Purchases.PreviewAsync(input);
        Assert.Equal(expected, preview.Subtotal); Assert.Empty(db.Purchases); stock.VerifyNoOtherCalls();
        var saved = await Purchases.CreatePurchaseOrderAsync(input);
        Assert.Equal(preview.GrandTotal, saved.GrandTotal);
        Assert.Equal(kg, saved.Items[0].KgPerUnit); Assert.Equal(perKg, saved.Items[0].LandingCostPerKg);
    }

    [Theory]
    [InlineData(null, 3)]
    [InlineData(0.0, 3)]
    [InlineData(-1.0, 3)]
    [InlineData(0.00001, 3)]
    public async Task InvalidWeightInputsCannotPreviewOrPersist(double? kg, double perKg)
    {
        var input = Input(); input.Items[0].KgPerUnit = (decimal?)kg; input.Items[0].LandingCostPerKg = (decimal)perKg;
        await Assert.ThrowsAsync<ArgumentException>(() => Purchases.PreviewAsync(input));
        await Assert.ThrowsAsync<ArgumentException>(() => Purchases.CreatePurchaseOrderAsync(input));
        Assert.Empty(db.Purchases);
    }

    [Fact]
    public async Task PreviewRejectsForeignTenantWithoutAnyMutation()
    {
        var input = Input(); input.SupplierId = foreignSupplier.Id;
        await Assert.ThrowsAsync<ArgumentException>(() => Purchases.PreviewAsync(input));
        Assert.Empty(db.Purchases); Assert.Empty(db.StockMovements); stock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task TrackedForeignCategoryCannotBeUpdatedOrLinked()
    {
        var categories = new CategoryService(db, new EntityNormalizationService(), actor);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => categories.UpdateAsync(foreignCategory.Id, "Hacked"));
        var types = new CategoryTypeService(db, new EntityNormalizationService(), actor);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => types.CreateAsync(foreignCategory.Id, "Hacked"));
        Assert.Equal("Other food", foreignCategory.Name);
    }

    [Fact]
    public async Task TrackedForeignContactsCannotBeReadOrChanged()
    {
        var foreignBroker = new Broker { BusinessId = foreignTenant, Name = "Other broker" };
        db.Brokers.Add(foreignBroker); await db.SaveChangesAsync();
        var suppliers = new SupplierService(db, new EntityNormalizationService(), actor);
        var brokers = new BrokerService(db, new EntityNormalizationService(), actor);
        Assert.Null(await suppliers.GetByIdAsync(foreignSupplier.Id));
        Assert.Null(await brokers.GetByIdAsync(foreignBroker.Id));
        await Assert.ThrowsAsync<KeyNotFoundException>(() => suppliers.UpdateAsync(foreignSupplier.Id, new() { Name = "Changed" }));
        await Assert.ThrowsAsync<KeyNotFoundException>(() => suppliers.DeleteAsync(foreignSupplier.Id));
        await Assert.ThrowsAsync<KeyNotFoundException>(() => brokers.UpdateAsync(foreignBroker.Id, new() { Name = "Changed" }));
        await Assert.ThrowsAsync<KeyNotFoundException>(() => brokers.DeleteAsync(foreignBroker.Id));
        Assert.Equal("Foreign supplier", foreignSupplier.Name); Assert.Equal("Other broker", foreignBroker.Name);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task StockLedgerCannotBeChangedOrDeleted(bool delete)
    {
        var movement = new StockMovement { BusinessId = tenant, CatalogItemId = item.Id, CreatedById = actor.UserId!.Value,
            QuantityBefore = 19, QuantityAfter = 20, QuantityDelta = 1, MovementType = "AdjustmentIncrease" };
        db.StockMovements.Add(movement); await db.SaveChangesAsync();
        if (delete) db.StockMovements.Remove(movement); else movement.QuantityAfter = 999;
        await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
        Assert.Throws<InvalidOperationException>(() => db.SaveChanges());
    }

    [Fact]
    public async Task MalformedStoredPermissionsFailClosed()
    {
        var membership = await db.Memberships.SingleAsync(); membership.PermissionsJson = "not json";
        await db.SaveChangesAsync();
        Assert.False(await CurrentUserService.ValidateSessionAsync(Principal(tenant), db));
    }

    [Theory]
    [InlineData(Role.Staff, "stock.view")]
    [InlineData(Role.Manager, "purchase.create")]
    public async Task NewMembershipGetsUsableRolePermissions(Role role, string permission)
    {
        actor.Role = "Owner";
        var created = await new UserService(db, actor).CreateUserAsync(new() { Name = "New colleague", Email = $"{role}@test.local",
            Password = "strong-test-password", Role = role });
        var membership = await db.Memberships.SingleAsync(m => m.UserId == created.Id);
        var permissions = System.Text.Json.JsonSerializer.Deserialize<List<string>>(membership.PermissionsJson!)!;
        Assert.Contains(permission, permissions);
        Assert.Equal(role == Role.Manager, permissions.Contains("users.manage"));
    }

    [Theory]
    [InlineData(PurchaseStatus.Dispatched)]
    [InlineData(PurchaseStatus.Arrived)]
    public async Task ReceiptCannotBypassExplicitVerification(PurchaseStatus stage)
    {
        var draft = await Draft();
        await Purchases.UpdateStatusAsync(draft.Id, PurchaseStatus.Confirmed);
        await Purchases.UpdateStatusAsync(draft.Id, PurchaseStatus.Dispatched);
        if (stage == PurchaseStatus.Arrived) await Purchases.UpdateStatusAsync(draft.Id, PurchaseStatus.Arrived);
        await Assert.ThrowsAsync<InvalidOperationException>(() => Purchases.ReceiveItemsAsync(draft.Id, new() {
            Items = [new() { PurchaseItemId = draft.Items[0].Id, ReceivedQuantityDelta = 1 }] }));
        stock.VerifyNoOtherCalls(); Assert.Empty(db.StockMovements);
    }

    [Fact]
    public async Task VerificationRecordsActorAndActivityWithoutStockMutation()
    {
        var draft = await Draft();
        await Purchases.UpdateStatusAsync(draft.Id, PurchaseStatus.Confirmed);
        await Purchases.UpdateStatusAsync(draft.Id, PurchaseStatus.Dispatched);
        await Purchases.UpdateStatusAsync(draft.Id, PurchaseStatus.Arrived);
        var verified = await Purchases.UpdateStatusAsync(draft.Id, PurchaseStatus.Verified);
        Assert.NotNull(verified.VerifiedAt); Assert.Equal(actor.UserId, verified.VerifiedById);
        Assert.Equal(5, (await Purchases.GetActivityAsync(draft.Id)).Count);
        Assert.Empty(db.StockMovements); stock.VerifyNoOtherCalls();
        Assert.Equal(20, item.CurrentStock);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => Purchases.GetActivityAsync(Guid.NewGuid()));
        var foreign = new PurchaseOrder { BusinessId = foreignTenant, SupplierId = foreignSupplier.Id, OrderNumber = "FOREIGN-AUDIT" };
        db.Purchases.Add(foreign); await db.SaveChangesAsync();
        await Assert.ThrowsAsync<KeyNotFoundException>(() => Purchases.GetActivityAsync(foreign.Id));
    }

    [Fact]
    public async Task DuplicateReviewKeepsEachDistinctPairWhenIdsNeedReordering()
    {
        var items = new[] { new CatalogItem { Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), Name = "Rice A", ItemCode = "A" },
            new CatalogItem { Id = Guid.Parse("00000001-0000-0000-0000-000000000001"), Name = "Rice B", ItemCode = "B" },
            new CatalogItem { Id = Guid.Parse("00000002-0000-0000-0000-000000000002"), Name = "Rice C", ItemCode = "C" } };
        item.IsActive = false;
        foreach (var candidate in items) { candidate.BusinessId = tenant; candidate.CategoryId = category.Id; candidate.IsActive = true; }
        db.CatalogItems.AddRange(items); await db.SaveChangesAsync();
        var duplicates = await new CatalogService(db, new EntityNormalizationService(), actor).GetDuplicateCandidatesAsync();
        Assert.Equal(3, duplicates.Count);
        Assert.Equal(3, duplicates.Select(d => (d.ItemAId, d.ItemBId)).Distinct().Count());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task InvalidReportPeriodsReturnValidationErrors(int days)
    {
        var report = new ReportService(db); var start = DateTime.UtcNow;
        await Assert.ThrowsAsync<ArgumentException>(() => report.GetPeriodComparisonAsync(tenant, start, start.AddDays(days)));
        await Assert.ThrowsAsync<ArgumentException>(() => report.GetSpendAnalyticsAsync(tenant, start, start.AddDays(days)));
    }

    [Fact]
    public async Task RedactedFinancialValuesCannotOverwriteAnExistingPurchase()
    {
        var draft = await Draft(); var input = Input(); input.Items[0].UnitPrice = 0;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Purchases.UpdatePurchaseOrderAsync(draft.Id, input));
        Assert.Equal(4, (await db.PurchaseItems.SingleAsync()).UnitPrice);
        Assert.Equal(40, (await db.Purchases.SingleAsync()).GrandTotal);
        input = Input(); input.Items[0].TaxPercent = 1;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Purchases.UpdatePurchaseOrderAsync(draft.Id, input));
        Assert.Equal(0, (await db.Purchases.SingleAsync()).TaxTotal);
    }

    [Fact]
    public void ExternalStockRequestCannotForgePurchaseProvenance()
    {
        var request = System.Text.Json.JsonSerializer.Deserialize<AdjustStockRequestDto>(
            "{\"QuantityDelta\":1,\"ReferenceType\":\"PurchaseOrder\",\"ReferenceId\":\"forged\"}")!;
        Assert.Null(request.ReferenceType); Assert.Null(request.ReferenceId); Assert.Equal(1, request.QuantityDelta);
    }

    [Fact]
    public async Task PurchaseAndPreviewRejectTrackedForeignCatalogItemBeforeAnyWrite()
    {
        var foreignItem = new CatalogItem { BusinessId = foreignTenant, CategoryId = foreignCategory.Id, Name = "Foreign item", ItemCode = "FOREIGN" };
        db.CatalogItems.Add(foreignItem); await db.SaveChangesAsync();
        var input = Input(); input.Items[0].CatalogItemId = foreignItem.Id;
        await Assert.ThrowsAsync<ArgumentException>(() => Purchases.PreviewAsync(input));
        await Assert.ThrowsAsync<ArgumentException>(() => Purchases.CreatePurchaseOrderAsync(input));
        Assert.Empty(db.Purchases); Assert.Empty(db.SecurityAuditLogs); stock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(false, 100, 10, 5, 180, 9, 189)]
    [InlineData(true, 150, 10, 5, 270, 13.5, 283.5)]
    [InlineData(true, 100, 10, 5, 180, 9, 189)]
    [InlineData(false, 100, 100, 5, 0, 0, 0)]
    public async Task BackendAppliesDiscountThenTaxWithIdenticalPreviewAndSave(bool weight, decimal unitCost, decimal discount, decimal tax,
        decimal net, decimal taxAmount, decimal total)
    {
        var input = Input(); input.Items[0].OrderedQuantity = 2; input.Items[0].UnitPrice = unitCost;
        input.Items[0].DiscountPercent = discount; input.Items[0].TaxPercent = tax;
        if (weight) { input.Items[0].KgPerUnit = 50; input.Items[0].LandingCostPerKg = 3; }
        var preview = await Purchases.PreviewAsync(input);
        Assert.Equal(net, preview.Subtotal); Assert.Equal(taxAmount, preview.TaxTotal); Assert.Equal(total, preview.GrandTotal);
        Assert.Empty(db.Purchases); Assert.Empty(db.SecurityAuditLogs); stock.VerifyNoOtherCalls();
        var saved = await Purchases.CreatePurchaseOrderAsync(input);
        Assert.Equal(preview.Subtotal, saved.Subtotal); Assert.Equal(preview.TaxTotal, saved.TaxTotal); Assert.Equal(preview.GrandTotal, saved.GrandTotal);
        Assert.Equal(total, saved.Items[0].LineTotal); Assert.Equal(discount, saved.Items[0].DiscountPercent); Assert.Equal(tax, saved.Items[0].TaxPercent);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(101, 0)]
    [InlineData(0.001, 0)]
    [InlineData(0, -1)]
    [InlineData(0, 1001)]
    [InlineData(0, 0.001)]
    public async Task InvalidDiscountAndTaxRatesCannotPersist(decimal discount, decimal tax)
    {
        var input = Input(); input.Items[0].DiscountPercent = discount; input.Items[0].TaxPercent = tax;
        await Assert.ThrowsAsync<ArgumentException>(() => Purchases.PreviewAsync(input));
        await Assert.ThrowsAsync<ArgumentException>(() => Purchases.CreatePurchaseOrderAsync(input));
        Assert.Empty(db.Purchases); stock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ClientTaxAndGrandTotalsCannotBecomeFinancialTruth()
    {
        var body = System.Text.Json.JsonSerializer.SerializeToNode(Input())!.AsObject();
        body["TaxTotal"] = 9999; body["GrandTotal"] = 99999; body["Subtotal"] = 99999;
        var input = body.Deserialize<UpsertPurchaseOrderDto>()!;
        var preview = await Purchases.PreviewAsync(input); var saved = await Purchases.CreatePurchaseOrderAsync(input);
        Assert.Equal(0, preview.TaxTotal); Assert.Equal(40, preview.GrandTotal); Assert.Equal(40, saved.GrandTotal);
    }

    private ClaimsPrincipal Principal(Guid businessId) => new(new ClaimsIdentity(new[] {
        new Claim(ClaimTypes.NameIdentifier, actor.UserId!.Value.ToString()), new Claim("businessId", businessId.ToString()),
        new Claim("role", "Owner"), new Claim("permissions", "users.manage"), new Claim("sessionId", sessionId.ToString()) }, "test"));

    private sealed class Actor : ICurrentUserService
    {
        public Guid? UserId { get; init; }
        public Guid? BusinessId { get; init; }
        public string Role { get; set; } = "Manager";
        public string Email => "audit@test.local";
        public IEnumerable<string> Permissions => [];
        public bool HasPermission(string permission) => false;
    }
}
