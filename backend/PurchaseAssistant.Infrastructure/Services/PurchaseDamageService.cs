using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs.Purchase;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PurchaseAssistant.Infrastructure.Services
{
    public class PurchaseDamageService : IPurchaseDamageService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUser;

        private const string StatusPending = "pending";
        private const string StatusApproved = "approved";
        private const string StatusRejected = "rejected";
        private const string StatusReturned = "returned";

        public PurchaseDamageService(AppDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<DamageReportDto>> GetDamageReportsAsync(Guid purchaseOrderId)
        {
            // Use inline LINQ projection so EF can translate to SQL — avoid static method in Select()
            return await _context.PurchaseDamageReports
                .AsNoTracking()
                .Where(r => r.PurchaseOrderId == purchaseOrderId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new DamageReportDto
                {
                    Id = r.Id,
                    CreatedAt = r.CreatedAt,
                    ReportedBy = r.ReportedByUser != null ? r.ReportedByUser.Name : null,
                    PurchaseOrderId = r.PurchaseOrderId,
                    CatalogItemId = r.CatalogItemId,
                    ItemName = r.ItemName,
                    QtyDamaged = r.QtyDamaged,
                    Unit = r.Unit,
                    DamageType = r.DamageType,
                    Reason = r.Reason,
                    Status = r.Status,
                    PhotoUrl = r.PhotoUrl,
                    Notes = r.Notes
                })
                .ToListAsync();
        }

        public async Task<DamageReportDto> CreateDamageReportAsync(Guid purchaseOrderId, CreateDamageReportDto dto)
        {
            var purchase = await _context.Purchases
                .FirstOrDefaultAsync(p => p.Id == purchaseOrderId)
                ?? throw new KeyNotFoundException($"Purchase order {purchaseOrderId} not found.");

            var businessId = purchase.BusinessId;

            // Resolve item name — explicit tenant guard on catalog item
            string itemName;
            Guid? catalogItemId = dto.CatalogItemId;
            string? unit = dto.Unit;

            if (!PurchaseInputLimits.IsQuantityValid(dto.QtyDamaged) || dto.ItemName?.Length > 500
                || dto.Notes?.Length > 4000 || dto.DamageType.HasValue && !Enum.IsDefined(dto.DamageType.Value)
                || dto.Reason.HasValue && !Enum.IsDefined(dto.Reason.Value)) throw new ArgumentException("Invalid damage details.");
            if (catalogItemId.HasValue)
            {
                if (!await _context.PurchaseItems.AnyAsync(i => i.PurchaseOrderId == purchaseOrderId && i.CatalogItemId == catalogItemId.Value && i.BusinessId == businessId)) throw new ArgumentException("Item is not on this purchase.");
                var catalogItem = await _context.CatalogItems
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == catalogItemId.Value && c.BusinessId == businessId)
                    ?? throw new KeyNotFoundException($"Catalog item {catalogItemId.Value} not found.");
                itemName = catalogItem.Name;
                unit ??= catalogItem.DefaultUnit;
            }
            else if (!string.IsNullOrWhiteSpace(dto.ItemName))
            {
                itemName = dto.ItemName.Trim();
            }
            else
            {
                throw new ArgumentException("Either ItemName or CatalogItemId must be provided.");
            }

            // At least one of DamageType or Reason must be set (matches reference business rule)
            if (dto.DamageType is null && dto.Reason is null)
                throw new ArgumentException("At least one of DamageType or Reason must be provided.");

            var report = new PurchaseDamageReport
            {
                Id = Guid.NewGuid(),
                BusinessId = businessId,
                PurchaseOrderId = purchaseOrderId,
                CatalogItemId = catalogItemId,
                ItemName = itemName,
                QtyDamaged = dto.QtyDamaged,
                Unit = unit,
                DamageType = dto.DamageType.HasValue ? ToSnakeCase(dto.DamageType.Value.ToString()) : "damaged",
                Reason = dto.Reason.HasValue ? ToSnakeCase(dto.Reason.Value.ToString()) : null,
                Status = StatusPending,
                PhotoUrl = PurchaseAssistant.Application.DTOs.SafeImageUrl.Validate(dto.PhotoUrl),
                Notes = dto.Notes,
                ReportedByUserId = _currentUser.UserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await using var transaction = _context.Database.IsRelational()
                ? await _context.Database.BeginTransactionAsync()
                : null;
            _context.PurchaseDamageReports.Add(report);
            _context.SecurityAuditLogs.Add(new SecurityAuditLog
            {
                BusinessId = businessId,
                UserId = _currentUser.UserId,
                EventType = "DamageReportCreated",
                Description = $"DamageReport:{report.Id} created",
                MetadataJson = JsonSerializer.Serialize(new
                {
                    reportId = report.Id,
                    purchaseOrderId,
                    catalogItemId,
                    report.DamageType,
                    report.Reason,
                    report.QtyDamaged
                })
            });

            if (dto.EmitNotification)
            {
                var recipients = await _context.Memberships.Where(m => m.BusinessId == businessId && (m.Role == PurchaseAssistant.Domain.Enums.Role.Owner || m.Role == PurchaseAssistant.Domain.Enums.Role.Admin || m.Role == PurchaseAssistant.Domain.Enums.Role.Manager)).Select(m => m.UserId).ToListAsync();
                var notifications = new NotificationService(_context);
                foreach (var recipient in recipients) await notifications.CreateNotificationAsync(businessId, recipient, PurchaseAssistant.Domain.Enums.NotificationType.VerificationRequired, "Damage reported", "A purchase damage report requires review.", "DamageReport", report.Id);
            }
            await _context.SaveChangesAsync();
            if (transaction != null) await transaction.CommitAsync();
            return await GetReportDtoAsync(report.Id);
        }

        public async Task<DamageReportDto> UpdateDamageReportStatusAsync(Guid purchaseOrderId, Guid reportId, UpdateDamageReportStatusDto dto)
        {
            // Cannot set status back to Pending
            if (!Enum.IsDefined(dto.Status) || dto.Status == DamageStatus.Pending || dto.Notes?.Length > 4000)
                throw new ArgumentException("Invalid damage report status or notes.");

            // Explicit ownership check: reportId must belong to this purchaseOrderId (tenant guard via query filter)
            var report = await _context.PurchaseDamageReports
                .FirstOrDefaultAsync(r => r.Id == reportId && r.PurchaseOrderId == purchaseOrderId)
                ?? throw new KeyNotFoundException($"Damage report {reportId} not found for purchase order {purchaseOrderId}.");

            // State transition guard: only pending reports can be updated (reference: DamageStatusPatch only valid for pending)
            if (report.Status != StatusPending)
                throw new DbUpdateConcurrencyException("DAMAGE_REPORT_VERSION_CONFLICT");

            report.Status = ToSnakeCase(dto.Status.ToString());
            if (dto.Notes is not null)
                report.Notes = dto.Notes;
            report.UpdatedAt = DateTime.UtcNow;
            _context.SecurityAuditLogs.Add(new SecurityAuditLog
            {
                BusinessId = report.BusinessId,
                UserId = _currentUser.UserId,
                EventType = "DamageReportStatusChanged",
                Description = $"DamageReport:{report.Id} status changed",
                MetadataJson = JsonSerializer.Serialize(new
                {
                    reportId = report.Id,
                    purchaseOrderId,
                    previousStatus = StatusPending,
                    newStatus = report.Status,
                    notesUpdated = dto.Notes is not null
                })
            });

            await using var transaction = _context.Database.IsRelational()
                ? await _context.Database.BeginTransactionAsync()
                : null;
            try
            {
                await _context.SaveChangesAsync();
                if (transaction != null) await transaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DbUpdateConcurrencyException("DAMAGE_REPORT_VERSION_CONFLICT");
            }

            return await GetReportDtoAsync(reportId);
        }

        public async Task<PendingDamageReportsCountDto> GetPendingCountAsync()
        {
            var count = await _context.PurchaseDamageReports
                .AsNoTracking()
                .CountAsync(r => r.Status == StatusPending);

            return new PendingDamageReportsCountDto { Count = count };
        }

        // ── helpers ────────────────────────────────────────────────────────────

        private async Task<DamageReportDto> GetReportDtoAsync(Guid id)
        {
            return await _context.PurchaseDamageReports
                .AsNoTracking()
                .Include(x => x.ReportedByUser)
                .Where(x => x.Id == id)
                .Select(r => new DamageReportDto
                {
                    Id = r.Id,
                    CreatedAt = r.CreatedAt,
                    ReportedBy = r.ReportedByUser != null ? r.ReportedByUser.Name : null,
                    PurchaseOrderId = r.PurchaseOrderId,
                    CatalogItemId = r.CatalogItemId,
                    ItemName = r.ItemName,
                    QtyDamaged = r.QtyDamaged,
                    Unit = r.Unit,
                    DamageType = r.DamageType,
                    Reason = r.Reason,
                    Status = r.Status,
                    PhotoUrl = r.PhotoUrl,
                    Notes = r.Notes
                })
                .FirstAsync();
        }

        /// <summary>
        /// Converts PascalCase enum names to snake_case strings matching reference DB values.
        /// Uses regex to handle consecutive uppercase (e.g. "WETDamage" → "wet_damage").
        /// </summary>
        private static string ToSnakeCase(string value)
            => Regex.Replace(value, @"([A-Z])([A-Z][a-z])|([a-z0-9])([A-Z])", "$1$3_$2$4").ToLowerInvariant();
    }
}
