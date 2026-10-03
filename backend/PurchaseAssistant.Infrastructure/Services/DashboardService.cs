using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs.Dashboard;
using PurchaseAssistant.Application.DTOs.Purchase;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Enums;
using PurchaseAssistant.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PurchaseAssistant.Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService? _user;

        public DashboardService(AppDbContext context, ICurrentUserService? user = null)
        {
            _context = context;
            _user = user;
        }

        public async Task<DashboardDto> GetDashboardDataAsync()
        {
            var dto = new DashboardDto();
            var today = DateTime.UtcNow.Date;

            // Purchase Metrics
            var canPurchase = _user == null || _user.Role is "Owner" or "SuperAdmin" || _user.HasPermission("purchase.view");
            var canStock = _user == null || _user.Role is "Owner" or "SuperAdmin" || _user.HasPermission("stock.view");
            var purchasesQuery = _context.Purchases.AsNoTracking().Where(_ => canPurchase);

            dto.PurchaseMetrics.TodayPurchasesCount = await purchasesQuery
                .Where(p => p.CreatedAt >= today)
                .CountAsync();

            dto.PurchaseMetrics.PendingPurchasesCount = await purchasesQuery
                .Where(p => p.Status == PurchaseStatus.Draft || p.Status == PurchaseStatus.Confirmed)
                .CountAsync();

            dto.PurchaseMetrics.ActivePurchasesCount = await purchasesQuery
                .Where(p => p.Status != PurchaseStatus.Completed && p.Status != PurchaseStatus.Cancelled)
                .CountAsync();

            dto.PurchaseMetrics.CompletedPurchasesCount = await purchasesQuery
                .Where(p => p.Status == PurchaseStatus.Completed)
                .CountAsync();

            dto.PurchaseMetrics.TotalPurchaseSpend = await purchasesQuery
                .Where(p => p.Status != PurchaseStatus.Cancelled && p.Status != PurchaseStatus.Draft)
                .SumAsync(p => p.GrandTotal);

            // Stock Metrics
            var itemsQuery = _context.CatalogItems.AsNoTracking().Where(i => canStock && i.IsActive);

            dto.StockMetrics.TotalCatalogItems = await itemsQuery.CountAsync();
            dto.StockMetrics.LowStockCount = await itemsQuery
                .Where(i => i.CurrentStock - i.ReservedStock <= i.ReorderLevel && i.CurrentStock - i.ReservedStock > 0)
                .CountAsync();
            dto.StockMetrics.OutOfStockCount = await itemsQuery
                .Where(i => i.CurrentStock - i.ReservedStock <= 0)
                .CountAsync();

            dto.StockMetrics.ItermsWithPhysicalVariance = await itemsQuery
                .Where(i => i.CurrentStock != i.PhysicalStock)
                .CountAsync();

            // Delivery Metrics
            // The prompt states: Show Draft, Confirmed, Dispatched, Arrived, VerificationPending
            var statuses = await purchasesQuery
                .GroupBy(p => p.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            dto.DeliveryMetrics.DraftPurchases = statuses.FirstOrDefault(s => s.Status == PurchaseStatus.Draft)?.Count ?? 0;
            dto.DeliveryMetrics.ConfirmedPurchases = statuses.FirstOrDefault(s => s.Status == PurchaseStatus.Confirmed)?.Count ?? 0;
            dto.DeliveryMetrics.DispatchedPurchases = statuses.FirstOrDefault(s => s.Status == PurchaseStatus.Dispatched)?.Count ?? 0;
            dto.DeliveryMetrics.ArrivedPurchases = statuses.FirstOrDefault(s => s.Status == PurchaseStatus.Arrived)?.Count ?? 0;
            dto.DeliveryMetrics.VerificationPending = statuses.FirstOrDefault(s => s.Status == PurchaseStatus.Verified)?.Count ?? 0;

            // Operational Alerts
            // Create alerts dynamically based on actual status
            if (dto.StockMetrics.OutOfStockCount > 0)
            {
                dto.OperationalAlerts.Add(new DashboardAlertDto
                {
                    Type = NotificationType.OutOfStock,
                    Title = "Critical: Out of Stock",
                    Message = $"{dto.StockMetrics.OutOfStockCount} items have reached strictly zero stock.",
                    Timestamp = DateTime.UtcNow
                });
            }

            if (dto.StockMetrics.LowStockCount > 0)
            {
                dto.OperationalAlerts.Add(new DashboardAlertDto
                {
                    Type = NotificationType.LowStock,
                    Title = "Low Stock Warning",
                    Message = $"{dto.StockMetrics.LowStockCount} items are at or below their reorder point.",
                    Timestamp = DateTime.UtcNow
                });
            }

            if (dto.DeliveryMetrics.ArrivedPurchases > 0)
            {
                dto.OperationalAlerts.Add(new DashboardAlertDto
                {
                    Type = NotificationType.VerificationRequired,
                    Title = "Pending Receiving Verification",
                    Message = $"{dto.DeliveryMetrics.ArrivedPurchases} purchases have arrived and need verification.",
                    Timestamp = DateTime.UtcNow
                });
            }

            // Recent Purchases
            dto.RecentPurchases = await purchasesQuery
                .Include(p => p.Supplier)
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .Select(p => new PurchaseOrderDto
                {
                    Id = p.Id,
                    OrderNumber = p.OrderNumber,
                    SupplierName = p.Supplier != null ? p.Supplier.Name : "N/A",
                    CreatedAt = p.CreatedAt,
                    Status = p.Status,
                    DeliveryState = p.DeliveryState,
                    PaymentState = p.PaymentState,
                    GrandTotal = p.GrandTotal
                })
                .ToListAsync();

            // Recent Stock Activity
            dto.RecentStockActivity = await _context.StockMovements.AsNoTracking().Where(_ => canStock)
                .Include(m => m.CatalogItem)
                .OrderByDescending(m => m.CreatedAt)
                .Take(10)
                .Select(m => new RecentStockActivityDto
                {
                    Id = m.Id,
                    ItemCode = m.CatalogItem != null ? m.CatalogItem.ItemCode : "",
                    CatalogItemName = m.CatalogItem != null ? m.CatalogItem.Name : "",
                    MovementType = m.MovementType,
                    QuantityDelta = m.QuantityDelta,
                    BeforeQuantity = m.QuantityBefore,
                    AfterQuantity = m.QuantityAfter,
                    Reason = m.Reason ?? "",
                    Date = m.CreatedAt,
                    UserId = m.CreatedById
                })
                .ToListAsync();

            return dto;
        }
    }
}
