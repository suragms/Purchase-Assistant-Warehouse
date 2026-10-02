using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs.Reports;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PurchaseAssistant.Infrastructure.Services
{
    public partial class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        // Shared authoritative eligibility for reports, exports and business backups.
        public static IQueryable<Domain.Entities.PurchaseOrder> ReportingPurchases(AppDbContext context, Guid businessId)
            => context.Purchases.AsNoTracking().Where(p => p.BusinessId == businessId
                && p.Status != Domain.Enums.PurchaseStatus.Cancelled && p.Status != Domain.Enums.PurchaseStatus.Draft);

        private static void ValidatePeriod(ref DateTime startDate, ref DateTime endDate)
        {
            startDate = startDate.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(startDate, DateTimeKind.Utc) : startDate.ToUniversalTime();
            endDate = endDate.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(endDate, DateTimeKind.Utc) : endDate.ToUniversalTime();
            if (endDate <= startDate || (endDate - startDate).TotalDays > 3660 || startDate < DateTime.MinValue.AddDays(3660))
                throw new ArgumentException("Choose an end date after the start date, with a reporting period of at most ten years.");
        }

        public async Task<List<SpendAnalyticsDto>> GetSpendAnalyticsAsync(Guid businessId, DateTime startDate, DateTime endDate, string groupBy = "day")
        {
            ValidatePeriod(ref startDate, ref endDate);
            if (groupBy is not ("day" or "month")) throw new ArgumentException("Choose day or month for spend grouping.");
            var query = ReportingPurchases(_context, businessId).Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate);

            var rawData = await query
                .Select(p => new { p.CreatedAt, p.GrandTotal })
                .ToListAsync();

            var grouped = groupBy.ToLower() switch
            {
                "month" => rawData
                    .GroupBy(p => new { p.CreatedAt.Year, p.CreatedAt.Month })
                    .Select(g => new SpendAnalyticsDto
                    {
                        PeriodLabel = $"{g.Key.Year}-{g.Key.Month:D2}",
                        TotalSpend = g.Sum(x => x.GrandTotal),
                        PurchaseCount = g.Count()
                    })
                    .OrderBy(x => x.PeriodLabel)
                    .ToList(),
                _ => rawData
                    .GroupBy(p => p.CreatedAt.Date)
                    .Select(g => new SpendAnalyticsDto
                    {
                        PeriodLabel = g.Key.ToString("yyyy-MM-dd"),
                        TotalSpend = g.Sum(x => x.GrandTotal),
                        PurchaseCount = g.Count()
                    })
                    .OrderBy(x => x.PeriodLabel)
                    .ToList()
            };

            return grouped;
        }

        public async Task<PurchaseSummaryReportDto> GetPurchaseSummaryAsync(Guid businessId, DateTime startDate, DateTime endDate)
        {
            ValidatePeriod(ref startDate, ref endDate);
            var purchasesQuery = ReportingPurchases(_context, businessId).Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate);

            // By Supplier
            var bySupplier = await purchasesQuery
                .Include(p => p.Supplier)
                .GroupBy(p => p.Supplier != null ? p.Supplier.Name : "Unknown Supplier")
                .Select(g => new SummaryItemDto
                {
                    Key = g.Key,
                    TotalSpend = g.Sum(p => p.GrandTotal),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.TotalSpend)
                .ToListAsync();

            // By Status
            var byStatus = await purchasesQuery
                .GroupBy(p => p.Status.ToString())
                .Select(g => new SummaryItemDto
                {
                    Key = g.Key,
                    TotalSpend = g.Sum(p => p.GrandTotal),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.TotalSpend)
                .ToListAsync();

            // By Category (via PurchaseItems -> CatalogItem -> Category)
            var itemsQuery = _context.PurchaseItems
                .AsNoTracking()
                .Where(pi => pi.BusinessId == businessId && pi.PurchaseOrder.CreatedAt >= startDate && pi.PurchaseOrder.CreatedAt <= endDate && pi.PurchaseOrder.Status != Domain.Enums.PurchaseStatus.Cancelled && pi.PurchaseOrder.Status != Domain.Enums.PurchaseStatus.Draft);

            var byCategoryRaw = await itemsQuery
                .Include(pi => pi.CatalogItem)
                .ThenInclude(ci => ci!.Category)
                .ToListAsync();

            var byCategory = byCategoryRaw
                .GroupBy(pi => pi.CatalogItem?.Category?.Name ?? "Uncategorized")
                .Select(g => new SummaryItemDto
                {
                    Key = g.Key,
                    TotalSpend = g.Sum(pi => pi.LineTotal),
                    Count = g.Select(pi => pi.PurchaseOrderId).Distinct().Count()
                })
                .OrderByDescending(x => x.TotalSpend)
                .ToList();

            return new PurchaseSummaryReportDto
            {
                BySupplier = bySupplier,
                ByCategory = byCategory,
                ByStatus = byStatus
            };
        }

        public async Task<StockAnalyticsDto> GetStockAnalyticsAsync(Guid businessId)
        {
            var itemsQuery = _context.CatalogItems
                .AsNoTracking()
                .Where(i => i.BusinessId == businessId);

            var totalItems = await itemsQuery.CountAsync();
            var lowStockCount = await itemsQuery.Where(i => i.CurrentStock - i.ReservedStock <= i.ReorderLevel && i.CurrentStock - i.ReservedStock > 0).CountAsync();
            var outOfStockCount = await itemsQuery.Where(i => i.CurrentStock - i.ReservedStock <= 0).CountAsync();

            // Value only priced stock using the latest confirmed purchase cost. Never invent a fallback rate.
            var inventoryValue = await itemsQuery.Select(i => i.CurrentStock *
                (_context.PurchaseItems.Where(pi => pi.CatalogItemId == i.Id && pi.BusinessId == businessId
                    && pi.PurchaseOrder.Status != Domain.Enums.PurchaseStatus.Draft
                    && pi.PurchaseOrder.Status != Domain.Enums.PurchaseStatus.Cancelled)
                    .OrderByDescending(pi => pi.PurchaseOrder.ConfirmedAt ?? pi.PurchaseOrder.CreatedAt)
                    .ThenByDescending(pi => pi.Id)
                    .Select(pi => (decimal?)(pi.LineTotal / pi.OrderedQuantity)).FirstOrDefault() ?? 0m)).SumAsync();

            var unpricedCount = await itemsQuery.CountAsync(i => i.CurrentStock > 0 && !_context.PurchaseItems.Any(pi =>
                pi.CatalogItemId == i.Id && pi.BusinessId == businessId
                && pi.PurchaseOrder.Status != Domain.Enums.PurchaseStatus.Draft
                && pi.PurchaseOrder.Status != Domain.Enums.PurchaseStatus.Cancelled));

            var movementsCount = await _context.StockMovements
                .AsNoTracking()
                .Where(m => m.BusinessId == businessId)
                .CountAsync();

            return new StockAnalyticsDto
            {
                TotalCatalogItems = totalItems,
                LowStockCount = lowStockCount,
                OutOfStockCount = outOfStockCount,
                EstimatedInventoryValue = inventoryValue,
                UnpricedStockItemCount = unpricedCount,
                TotalMovementsCount = movementsCount
            };
        }

        public async Task<PeriodComparisonDto> GetPeriodComparisonAsync(Guid businessId, DateTime startDate, DateTime endDate)
        {
            ValidatePeriod(ref startDate, ref endDate);
            var span = endDate - startDate;
            var prevEndDate = startDate;
            var prevStartDate = startDate - span;

            // Current Period
            var currentPurchases = await _context.Purchases
                .AsNoTracking()
                .Where(p => p.BusinessId == businessId && p.CreatedAt >= startDate && p.CreatedAt <= endDate && p.Status != Domain.Enums.PurchaseStatus.Cancelled && p.Status != Domain.Enums.PurchaseStatus.Draft)
                .ToListAsync();

            var currentSpend = currentPurchases.Sum(p => p.GrandTotal);
            var currentOrders = currentPurchases.Count;
            var currentAvg = currentOrders > 0 ? currentSpend / currentOrders : 0;

            // Previous Period
            var prevPurchases = await _context.Purchases
                .AsNoTracking()
                .Where(p => p.BusinessId == businessId && p.CreatedAt >= prevStartDate && p.CreatedAt < prevEndDate && p.Status != Domain.Enums.PurchaseStatus.Cancelled && p.Status != Domain.Enums.PurchaseStatus.Draft)
                .ToListAsync();

            var prevSpend = prevPurchases.Sum(p => p.GrandTotal);
            var prevOrders = prevPurchases.Count;
            var prevAvg = prevOrders > 0 ? prevSpend / prevOrders : 0;

            decimal spendChange = prevSpend > 0 ? ((currentSpend - prevSpend) / prevSpend) * 100 : (currentSpend > 0 ? 100 : 0);
            decimal ordersChange = prevOrders > 0 ? ((decimal)(currentOrders - prevOrders) / prevOrders) * 100 : (currentOrders > 0 ? 100 : 0);
            decimal avgChange = prevAvg > 0 ? ((currentAvg - prevAvg) / prevAvg) * 100 : (currentAvg > 0 ? 100 : 0);

            return new PeriodComparisonDto
            {
                CurrentStartDate = startDate,
                CurrentEndDate = endDate,
                PreviousStartDate = prevStartDate,
                PreviousEndDate = prevEndDate,
                CurrentPeriodSpend = currentSpend,
                PreviousPeriodSpend = prevSpend,
                SpendChangePercentage = Math.Round(spendChange, 2),
                CurrentPeriodOrders = currentOrders,
                PreviousPeriodOrders = prevOrders,
                OrdersChangePercentage = Math.Round(ordersChange, 2),
                CurrentPeriodAvgOrderValue = Math.Round(currentAvg, 2),
                PreviousPeriodAvgOrderValue = Math.Round(prevAvg, 2),
                AvgOrderValueChangePercentage = Math.Round(avgChange, 2)
            };
        }
    }
}
