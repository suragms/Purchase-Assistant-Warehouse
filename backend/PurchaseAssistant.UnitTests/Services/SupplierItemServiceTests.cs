using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PurchaseAssistant.Application.DTOs.Catalog;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Infrastructure.Data;
using PurchaseAssistant.Infrastructure.Services;

namespace PurchaseAssistant.UnitTests.Services;

public sealed class SupplierItemServiceTests : IDisposable
{
    private readonly Guid _businessId = Guid.NewGuid();
    private readonly Guid _otherBusinessId = Guid.NewGuid();
    private readonly AppDbContext _db;
    private readonly SupplierService _service;
    private readonly Supplier _supplier;
    private readonly Supplier _otherSupplier;
    private readonly CatalogItem _item;
    private readonly CatalogItem _otherItem;

    public SupplierItemServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options;
        _db = new AppDbContext(options, new SupplierTenant(_businessId));
        _supplier = new Supplier { BusinessId = _businessId, Name = "Local supplier" };
        _otherSupplier = new Supplier { BusinessId = _businessId, Name = "Other supplier" };
        var category = new Category { BusinessId = _businessId, Name = "Food" };
        _item = new CatalogItem { BusinessId = _businessId, CategoryId = category.Id, Category = category, ItemCode = "RICE-1", Name = "Rice" };
        var foreignCategory = new Category { BusinessId = _otherBusinessId, Name = "Other" };
        _otherItem = new CatalogItem { BusinessId = _otherBusinessId, CategoryId = foreignCategory.Id, Category = foreignCategory, ItemCode = "X-1", Name = "Foreign item" };
        _db.AddRange(new Business { Id = _businessId, Name = "Here", IsActive = true },
            new Business { Id = _otherBusinessId, Name = "There", IsActive = true },
            category, foreignCategory, _supplier, _otherSupplier, _item, _otherItem);
        _db.SaveChanges();
        _service = new SupplierService(_db, new EntityNormalizationService(), new SupplierActor(_businessId));
    }

    [Fact]
    public async Task SupplierItemCanBeAddedListedEditedAndRemoved()
    {
        var created = await _service.AddItemAsync(_supplier.Id, new SupplierItemInputDto
        {
            CatalogItemId = _item.Id,
            SupplierItemCode = "  S-42  ",
            IsDefault = true,
            Notes = "  Preferred pack  "
        });
        Assert.Equal("S-42", created.SupplierItemCode);
        Assert.Equal("Preferred pack", created.Notes);
        Assert.Equal(_item.Name, created.ItemName);
        Assert.True(created.IsDefault);
        Assert.Single(await _service.GetItemsAsync(_supplier.Id));

        var updated = await _service.UpdateItemAsync(_supplier.Id, created.Id, new SupplierItemInputDto
        {
            CatalogItemId = _item.Id,
            SupplierItemCode = "S-43",
            IsDefault = false,
            Notes = "Updated"
        });
        Assert.Equal("S-43", updated.SupplierItemCode);
        Assert.False(updated.IsDefault);
        Assert.Equal("Updated", updated.Notes);

        await _service.RemoveItemAsync(_supplier.Id, created.Id);
        Assert.Empty(await _service.GetItemsAsync(_supplier.Id));
    }

    [Fact]
    public async Task DuplicateAndCrossTenantSupplierItemLinksAreRejected()
    {
        await _service.AddItemAsync(_supplier.Id, new SupplierItemInputDto { CatalogItemId = _item.Id });
        var duplicate = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.AddItemAsync(_supplier.Id, new SupplierItemInputDto { CatalogItemId = _item.Id }));
        Assert.Equal("SUPPLIER_ITEM_EXISTS", duplicate.Message);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.AddItemAsync(_supplier.Id, new SupplierItemInputDto { CatalogItemId = _otherItem.Id }));
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetItemsAsync(Guid.NewGuid()));
        Assert.Single(await _service.GetItemsAsync(_supplier.Id));
    }

    [Fact]
    public async Task PreferredSupplierIsUniquePerItemWhenSet()
    {
        var first = await _service.AddItemAsync(_supplier.Id, new SupplierItemInputDto { CatalogItemId = _item.Id, IsDefault = true });
        var second = await _service.AddItemAsync(_otherSupplier.Id, new SupplierItemInputDto { CatalogItemId = _item.Id, IsDefault = true });
        Assert.True(second.IsDefault);
        Assert.False((await _service.GetItemsAsync(_supplier.Id)).Single(x => x.Id == first.Id).IsDefault);
    }

    [Fact]
    public async Task LinkCannotBeMovedToAnotherCatalogItemOnUpdate()
    {
        var link = await _service.AddItemAsync(_supplier.Id, new SupplierItemInputDto { CatalogItemId = _item.Id });
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateItemAsync(_supplier.Id, link.Id,
            new SupplierItemInputDto { CatalogItemId = Guid.NewGuid() }));
        Assert.Equal("SUPPLIER_ITEM_CATALOG_ITEM_IMMUTABLE", error.Message);
    }

    public void Dispose() => _db.Dispose();

    private sealed class SupplierTenant(Guid businessId) : ITenantProvider
    {
        public Guid GetBusinessId() => businessId;
    }

    private sealed class SupplierActor(Guid businessId) : ICurrentUserService
    {
        public Guid? UserId => null;
        public Guid? BusinessId => businessId;
        public string Email => "supplier.test@local";
        public string Role => "Owner";
        public IEnumerable<string> Permissions => [];
        public bool HasPermission(string permission) => true;
    }
}
