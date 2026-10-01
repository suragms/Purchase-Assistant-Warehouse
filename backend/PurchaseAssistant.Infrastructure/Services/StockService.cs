using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs.Catalog;
using PurchaseAssistant.Application.DTOs.Stock;
using PurchaseAssistant.Application.DTOs.Purchase;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Infrastructure.Data;

namespace PurchaseAssistant.Infrastructure.Services
{
    public class StockService : IStockService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public StockService(AppDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        private IQueryable<CatalogItem> GetBaseQuery() =>
            _context.CatalogItems
                .Include(i => i.Category)
                .Where(i => i.BusinessId == _currentUser.BusinessId!.Value);

        private static StockItemDto MapToDto(CatalogItem i)
        {
            var system = i.CurrentStock;
            var reserved = i.ReservedStock;
            var physical = i.PhysicalStock;
            return new StockItemDto
            {
                Id = i.Id,
                ItemCode = i.ItemCode,
                Barcode = i.Barcode,
                Name = i.Name,
                CategoryName = i.Category != null ? i.Category.Name : string.Empty,
                DefaultUnit = i.DefaultUnit,
                SystemStock = system,
                PhysicalStock = physical,
                ReservedStock = reserved,
                AvailableStock = system - reserved,
                ReorderLevel = i.ReorderLevel,
                IsActive = i.IsActive,
                RowVersion = i.RowVersion
            };
        }

        public async Task<PaginatedResult<StockItemDto>> GetStockItemsAsync(
            int page, int pageSize, string? search, bool? lowStockOnly, bool? outOfStockOnly)
        {
            page = Math.Clamp(page, 1, 10000);
            pageSize = Math.Clamp(pageSize, 1, 100);
            var query = GetBaseQuery();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLowerInvariant();
                query = query.Where(i =>
                    i.Name.ToLower().Contains(s) ||
                    i.ItemCode.ToLower().Contains(s) ||
                    (i.Barcode != null && i.Barcode.ToLower().Contains(s)));
            }

            if (outOfStockOnly == true)
            {
                query = query.Where(i => (i.CurrentStock - i.ReservedStock) <= 0);
            }
            else if (lowStockOnly == true)
            {
                query = query.Where(i =>
                    (i.CurrentStock - i.ReservedStock) > 0 &&
                    (i.CurrentStock - i.ReservedStock) <= i.ReorderLevel);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(i => i.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<StockItemDto>
            {
                Data = items.Select(MapToDto).ToList(),
                Meta = new PaginationMeta
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = total,
                    TotalPages = (int)Math.Ceiling((double)total / pageSize)
                }
            };
        }

        public async Task<StockItemDto> GetStockDetailAsync(Guid itemId)
        {
            var item = await GetBaseQuery().FirstOrDefaultAsync(i => i.Id == itemId);
            if (item == null)
                throw new KeyNotFoundException($"Stock item {itemId} not found.");
            return MapToDto(item);
        }

        public async Task<PaginatedResult<StockMovementDto>> GetItemActivityAsync(Guid itemId, int page, int pageSize)
        {
            page = Math.Clamp(page, 1, 10000);
            pageSize = Math.Clamp(pageSize, 1, 100);
            var businessId = _currentUser.BusinessId!.Value;

            var itemExists = await _context.CatalogItems
                .AnyAsync(i => i.Id == itemId && i.BusinessId == businessId);
            if (!itemExists)
                throw new KeyNotFoundException($"Stock item {itemId} not found.");

            var query = _context.StockMovements
                .Include(m => m.CreatedBy)
                .Where(m => m.CatalogItemId == itemId && m.BusinessId == businessId);

            var total = await query.CountAsync();
            var movements = await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new StockMovementDto
                {
                    Id = m.Id,
                    CatalogItemId = m.CatalogItemId,
                    MovementType = m.MovementType,
                    QuantityDelta = m.QuantityDelta,
                    QuantityBefore = m.QuantityBefore,
                    QuantityAfter = m.QuantityAfter,
                    ReferenceType = m.ReferenceType,
                    ReferenceId = m.ReferenceId,
                    Reason = m.Reason,
                    Notes = m.Notes,
                    CreatedById = m.CreatedById,
                    CreatedByName = m.CreatedBy != null ? m.CreatedBy.Name : string.Empty,
                    CreatedAt = m.CreatedAt
                })
                .ToListAsync();

            return new PaginatedResult<StockMovementDto>
            {
                Data = movements,
                Meta = new PaginationMeta
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = total,
                    TotalPages = (int)Math.Ceiling((double)total / pageSize)
                }
            };
        }

        private async Task<CatalogItem> GetAndValidateItemForUpdate(Guid itemId, Guid expectedVersion)
        {
            var businessId = _currentUser.BusinessId!.Value;
            var item = await _context.CatalogItems
                .FirstOrDefaultAsync(i => i.Id == itemId && i.BusinessId == businessId);

            if (item == null)
                throw new KeyNotFoundException($"Stock item {itemId} not found.");

            if (item.RowVersion != expectedVersion)
                throw new InvalidOperationException("STOCK_VERSION_CONFLICT");

            return item;
        }

        public async Task<StockItemDto> AdjustStockAsync(Guid itemId, AdjustStockRequestDto request)
        {
            if (request.QuantityDelta == 0 || request.QuantityDelta > PurchaseInputLimits.MaxValue || request.QuantityDelta < -PurchaseInputLimits.MaxValue || decimal.Round(request.QuantityDelta, 4) != request.QuantityDelta)
                throw new ArgumentException("Enter a nonzero stock adjustment within range, with at most four decimal places.");
            await using var tx = _context.Database.CurrentTransaction == null
                ? await _context.Database.BeginTransactionAsync() : null;

            var item = await GetAndValidateItemForUpdate(itemId, request.ExpectedVersion);

            var qtyBefore = item.CurrentStock;
            var availableBefore = item.CurrentStock - item.ReservedStock;
            var qtyAfter = qtyBefore + request.QuantityDelta;
            var availableAfter = availableBefore + request.QuantityDelta;

            if (availableAfter < 0)
                throw new InvalidOperationException("INSUFFICIENT_STOCK");
            if (qtyAfter > PurchaseInputLimits.MaxValue)
                throw new ArgumentException("Stock quantity exceeds the supported range.");

            item.CurrentStock = qtyAfter;
            item.RowVersion = Guid.NewGuid();

            _context.StockMovements.Add(new StockMovement
            {
                BusinessId = _currentUser.BusinessId!.Value,
                CatalogItemId = item.Id,
                MovementType = request.QuantityDelta >= 0 ? "AdjustmentIncrease" : "AdjustmentDecrease",
                QuantityDelta = request.QuantityDelta,
                QuantityBefore = qtyBefore,
                QuantityAfter = qtyAfter,
                ReferenceType = request.ReferenceType,
                ReferenceId = request.ReferenceId,
                Reason = request.Reason,
                Notes = request.Notes,
                CreatedById = _currentUser.UserId!.Value,
                CreatedAt = DateTime.UtcNow
            });

            try
            {
                await _context.SaveChangesAsync();
                if (tx != null)
                {
                    await tx.CommitAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (tx != null)
                {
                    await tx.RollbackAsync();
                }
                throw new InvalidOperationException("STOCK_VERSION_CONFLICT");
            }

            return await GetStockDetailAsync(item.Id);
        }

        public async Task<StockItemDto> UpdatePhysicalStockAsync(Guid itemId, UpdatePhysicalStockRequestDto request)
        {
            if (request.PhysicalStock < 0 || request.PhysicalStock > PurchaseInputLimits.MaxValue || decimal.Round(request.PhysicalStock, 4) != request.PhysicalStock)
                throw new ArgumentException("Enter a nonnegative physical count within range, with at most four decimal places.");
            await using var tx = _context.Database.CurrentTransaction == null
                ? await _context.Database.BeginTransactionAsync() : null;

            var item = await GetAndValidateItemForUpdate(itemId, request.ExpectedVersion);

            var oldPhysical = item.PhysicalStock;
            item.PhysicalStock = request.PhysicalStock;
            item.RowVersion = Guid.NewGuid();

            _context.StockMovements.Add(new StockMovement
            {
                BusinessId = _currentUser.BusinessId!.Value,
                CatalogItemId = item.Id,
                MovementType = "PhysicalCount",
                QuantityDelta = 0,
                QuantityBefore = item.CurrentStock,
                QuantityAfter = item.CurrentStock,
                Reason = request.Reason ?? $"Physical count recorded: {request.PhysicalStock}",
                Notes = $"Counted: {request.PhysicalStock}. Previous: {oldPhysical}. Variance vs system: {request.PhysicalStock - item.CurrentStock}. {request.Notes}".Trim('.', ' '),
                CreatedById = _currentUser.UserId!.Value,
                CreatedAt = DateTime.UtcNow
            });

            try
            {
                await _context.SaveChangesAsync();
                if (tx != null)
                {
                    await tx.CommitAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (tx != null)
                {
                    await tx.RollbackAsync();
                }
                throw new InvalidOperationException("STOCK_VERSION_CONFLICT");
            }

            return await GetStockDetailAsync(item.Id);
        }

        public async Task<StockItemDto> ReconcileStockAsync(Guid itemId, ReconcileStockRequestDto request)
        {
            await using var tx = _context.Database.CurrentTransaction == null
                ? await _context.Database.BeginTransactionAsync() : null;

            var item = await GetAndValidateItemForUpdate(itemId, request.ExpectedVersion);

            var qtyBefore = item.CurrentStock;
            var physical = item.PhysicalStock;
            var delta = physical - qtyBefore;

            if (delta == 0)
                throw new InvalidOperationException("RECONCILE_NO_VARIANCE");

            var availableAfter = physical - item.ReservedStock;
            if (availableAfter < 0)
                throw new InvalidOperationException("INSUFFICIENT_STOCK");

            item.CurrentStock = physical;
            item.RowVersion = Guid.NewGuid();

            _context.StockMovements.Add(new StockMovement
            {
                BusinessId = _currentUser.BusinessId!.Value,
                CatalogItemId = item.Id,
                MovementType = "Reconciliation",
                QuantityDelta = delta,
                QuantityBefore = qtyBefore,
                QuantityAfter = item.CurrentStock,
                Reason = request.Reason ?? "Reconciled variance to match physical stock",
                Notes = request.Notes,
                CreatedById = _currentUser.UserId!.Value,
                CreatedAt = DateTime.UtcNow
            });

            try
            {
                await _context.SaveChangesAsync();
                if (tx != null)
                {
                    await tx.CommitAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (tx != null)
                {
                    await tx.RollbackAsync();
                }
                throw new InvalidOperationException("STOCK_VERSION_CONFLICT");
            }

            return await GetStockDetailAsync(item.Id);
        }
    }
}
