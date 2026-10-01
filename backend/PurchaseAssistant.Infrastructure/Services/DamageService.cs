using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs.Purchases;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Infrastructure.Data;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Domain.Constants;

namespace PurchaseAssistant.Infrastructure.Services;

public class DamageService : IDamageService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DamageService(AppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<PurchaseDamageReportDto>> GetReportsAsync(Guid purchaseOrderId)
    {
        return await _context.PurchaseDamageReports
            .AsNoTracking()
            .Where(r => r.PurchaseOrderId == purchaseOrderId)
            .Select(r => new PurchaseDamageReportDto
            {
                Id = r.Id,
                PurchaseOrderId = r.PurchaseOrderId,
                ItemName = r.ItemName,
                QtyDamaged = r.QtyDamaged,
                Unit = r.Unit,
                DamageType = r.DamageType,
                Status = r.Status,
                PhotoUrl = r.PhotoUrl,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<PurchaseDamageReportDto> CreateReportAsync(CreateDamageReportDto reportDto)
    {
        var purchase = await _context.Purchases.FindAsync(reportDto.PurchaseOrderId);
        if (purchase == null) throw new ArgumentException("Purchase order not found.");

        var report = new PurchaseDamageReport
        {
            Id = Guid.NewGuid(),
            BusinessId = _context.CurrentBusinessId,
            PurchaseOrderId = reportDto.PurchaseOrderId,
            CatalogItemId = reportDto.CatalogItemId,
            ItemName = reportDto.ItemName,
            QtyDamaged = reportDto.QtyDamaged,
            Unit = reportDto.Unit,
            DamageType = reportDto.DamageType,
            Reason = reportDto.Reason,
            Notes = reportDto.Notes,
            Status = "pending",
            ReportedByUserId = _currentUser.UserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.PurchaseDamageReports.Add(report);
        await _context.SaveChangesAsync();

        return new PurchaseDamageReportDto
        {
            Id = report.Id,
            PurchaseOrderId = report.PurchaseOrderId,
            ItemName = report.ItemName,
            QtyDamaged = report.QtyDamaged,
            Unit = report.Unit,
            DamageType = report.DamageType,
            Status = report.Status,
            PhotoUrl = report.PhotoUrl,
            CreatedAt = report.CreatedAt
        };
    }

    public async Task<bool> ApproveReportAsync(Guid reportId)
    {
        if (!_currentUser.HasPermission(Permissions.PurchaseDamageApprove))
            throw new UnauthorizedAccessException("Not permitted.");

        var report = await _context.PurchaseDamageReports.FindAsync(reportId);
        if (report == null) return false;

        report.Status = "approved";
        await _context.SaveChangesAsync();
        return true;
    }
}
