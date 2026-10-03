using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PurchaseAssistant.Application.DTOs.Purchase;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Domain.Enums;
using PurchaseAssistant.Infrastructure.Data;
using PurchaseAssistant.Infrastructure.Services;
using PurchaseAssistant.Web.Controllers;

namespace PurchaseAssistant.UnitTests.Services;

public sealed class PurchaseDamageServiceTests : IDisposable
{
    private readonly Guid _businessId = Guid.NewGuid();
    private readonly Guid _managerId = Guid.NewGuid();
    private readonly AppDbContext _db;
    private readonly PurchaseDamageService _service;
    private readonly PurchaseOrder _purchase;
    private readonly CatalogItem _item;

    public PurchaseDamageServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        var actor = new DamageActor { BusinessId = _businessId, UserId = _managerId };
        _db = new AppDbContext(options, new DamageTenant(_businessId));

        var business = new Business { Id = _businessId, Name = "Damage test", IsActive = true };
        var manager = new User { Id = _managerId, Name = "Manager", Email = "manager@damage.test", Status = UserStatus.Active };
        var supplier = new Supplier { BusinessId = _businessId, Name = "Supplier" };
        var category = new Category { BusinessId = _businessId, Name = "Produce" };
        _item = new CatalogItem
        {
            BusinessId = _businessId,
            CategoryId = category.Id,
            Category = category,
            Name = "Apples",
            ItemCode = "APL-01",
            RowVersion = Guid.NewGuid()
        };
        _purchase = new PurchaseOrder
        {
            BusinessId = _businessId,
            SupplierId = supplier.Id,
            Supplier = supplier,
            OrderNumber = "PO-DAMAGE-01",
            Status = PurchaseStatus.Confirmed,
            Items =
            [
                new PurchaseItem
                {
                    BusinessId = _businessId,
                    CatalogItemId = _item.Id,
                    CatalogItem = _item,
                    OrderedQuantity = 10,
                    UnitPrice = 2,
                    Unit = "kg"
                }
            ]
        };
        _db.AddRange(business, manager, supplier, category, _item, _purchase,
            new Membership { BusinessId = _businessId, UserId = _managerId, Role = Role.Manager });
        _db.SaveChanges();
        _service = new PurchaseDamageService(_db, actor);
    }

    [Fact]
    public async Task CreateDamageReport_SavesReportAuditAndReviewerNotificationTogether()
    {
        var report = await _service.CreateDamageReportAsync(_purchase.Id, new CreateDamageReportDto
        {
            CatalogItemId = _item.Id,
            QtyDamaged = 2,
            DamageType = DamageType.Damaged,
            Reason = DamageReason.WetDamage,
            Notes = "Two boxes arrived wet."
        });

        Assert.Equal("damaged", report.DamageType);
        Assert.Equal("wet_damage", report.Reason);
        Assert.Equal(1, await _db.PurchaseDamageReports.CountAsync());
        var audit = await _db.SecurityAuditLogs.SingleAsync();
        Assert.Equal("DamageReportCreated", audit.EventType);
        Assert.Equal(_managerId, audit.UserId);
        Assert.Contains(report.Id.ToString(), audit.MetadataJson);
        var notification = await _db.Notifications.SingleAsync();
        Assert.Equal(_managerId, notification.UserId);
        Assert.Equal(report.Id, notification.ReferenceId);
    }

    [Fact]
    public async Task ResolveDamageReport_AuditsOnceAndReturnsConflictOnRepeatedResolution()
    {
        Assert.True(_db.Model.FindEntityType(typeof(PurchaseDamageReport))!
            .FindProperty(nameof(PurchaseDamageReport.Status))!.IsConcurrencyToken);
        var report = await CreateReportAsync();

        var resolved = await _service.UpdateDamageReportStatusAsync(_purchase.Id, report.Id,
            new UpdateDamageReportStatusDto { Status = DamageStatus.Approved, Notes = "Reviewed" });

        Assert.Equal("approved", resolved.Status);
        Assert.Equal(2, await _db.SecurityAuditLogs.CountAsync());
        var conflict = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() =>
            _service.UpdateDamageReportStatusAsync(_purchase.Id, report.Id,
                new UpdateDamageReportStatusDto { Status = DamageStatus.Rejected }));

        Assert.Equal("DAMAGE_REPORT_VERSION_CONFLICT", conflict.Message);
        Assert.Equal("approved", (await _db.PurchaseDamageReports.SingleAsync()).Status);
        Assert.Equal(2, await _db.SecurityAuditLogs.CountAsync());
    }

    [Fact]
    public async Task CreateDamageReport_RejectsUnclassifiedReportWithoutWrites()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateDamageReportAsync(_purchase.Id,
            new CreateDamageReportDto { CatalogItemId = _item.Id, QtyDamaged = 1 }));

        Assert.Empty(await _db.PurchaseDamageReports.ToListAsync());
        Assert.Empty(await _db.SecurityAuditLogs.ToListAsync());
        Assert.Empty(await _db.Notifications.ToListAsync());
    }

    [Fact]
    public async Task DamageControllerReturnsConflictWhenAReviewerHasAlreadyChangedTheReport()
    {
        var service = new Mock<IPurchaseDamageService>();
        service.Setup(s => s.UpdateDamageReportStatusAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<UpdateDamageReportStatusDto>()))
            .ThrowsAsync(new DbUpdateConcurrencyException("DAMAGE_REPORT_VERSION_CONFLICT"));
        var controller = new DamageReportController(service.Object);

        var result = await controller.UpdateDamageReportStatus(_purchase.Id, Guid.NewGuid(),
            new UpdateDamageReportStatusDto { Status = DamageStatus.Approved });

        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    private Task<DamageReportDto> CreateReportAsync() => _service.CreateDamageReportAsync(_purchase.Id,
        new CreateDamageReportDto
        {
            CatalogItemId = _item.Id,
            QtyDamaged = 1,
            DamageType = DamageType.Short,
            Reason = DamageReason.ShortWeight,
            EmitNotification = false
        });

    public void Dispose() => _db.Dispose();

    private sealed class DamageTenant(Guid businessId) : ITenantProvider
    {
        public Guid GetBusinessId() => businessId;
    }

    private sealed class DamageActor : ICurrentUserService
    {
        public Guid? UserId { get; init; }
        public Guid? BusinessId { get; init; }
        public string Email => "manager@damage.test";
        public string Role => "Manager";
        public IEnumerable<string> Permissions => ["purchase.damage_report", "purchase.damage_approve"];
        public bool HasPermission(string permission) => Permissions.Contains(permission);
    }
}
