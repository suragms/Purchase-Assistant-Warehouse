using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs.Catalog;
using PurchaseAssistant.Application.DTOs.Purchase;
using PurchaseAssistant.Application.DTOs.Stock;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Domain.Enums;
using PurchaseAssistant.Infrastructure.Data;

namespace PurchaseAssistant.Infrastructure.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IStockService _stockService;

        public PurchaseService(AppDbContext context, ICurrentUserService currentUser, IStockService stockService)
        {
            _context = context;
            _currentUser = currentUser;
            _stockService = stockService;
        }

        private IQueryable<PurchaseOrder> GetBaseQuery() =>
            _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.Broker)
                .Include(p => p.Items)
                    .ThenInclude(i => i.CatalogItem)
                .Where(p => p.BusinessId == _currentUser.BusinessId!.Value);

        private static PurchaseItemDto MapItemToDto(PurchaseItem i) => new()
        {
            Id = i.Id,
            PurchaseOrderId = i.PurchaseOrderId,
            CatalogItemId = i.CatalogItemId,
            ItemCode = i.CatalogItem?.ItemCode ?? string.Empty,
            CatalogItemName = i.CatalogItem?.Name ?? string.Empty,
            OrderedQuantity = i.OrderedQuantity,
            ReceivedQuantity = i.ReceivedQuantity,
            UnitPrice = i.UnitPrice,
            LineTotal = i.LineTotal,
            Notes = i.Notes
        };

        private static PurchaseOrderDto MapToDto(PurchaseOrder p) => new()
        {
            Id = p.Id,
            OrderNumber = p.OrderNumber,
            SupplierId = p.SupplierId,
            SupplierName = p.Supplier?.Name ?? string.Empty,
            BrokerId = p.BrokerId,
            BrokerName = p.Broker?.Name,
            Status = p.Status,
            PaymentState = p.PaymentState,
            DeliveryState = p.DeliveryState,
            Notes = p.Notes,
            Subtotal = p.Subtotal,
            TaxTotal = p.TaxTotal,
            GrandTotal = p.GrandTotal,
            ConfirmedAt = p.ConfirmedAt,
            DispatchedAt = p.DispatchedAt,
            ArrivedAt = p.ArrivedAt,
            CompletedAt = p.CompletedAt,
            CreatedAt = p.CreatedAt,
            Items = p.Items.Select(MapItemToDto).ToList()
        };

        public async Task<PaginatedResult<PurchaseOrderDto>> GetPurchaseOrdersAsync(
            int page, int pageSize, string? search, PurchaseStatus? status, Guid? supplierId)
        {
            var query = GetBaseQuery();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLowerInvariant();
                query = query.Where(p =>
                    p.OrderNumber.ToLower().Contains(s) ||
                    p.Supplier.Name.ToLower().Contains(s) ||
                    (p.Notes != null && p.Notes.ToLower().Contains(s)));
            }

            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            if (supplierId.HasValue)
            {
                query = query.Where(p => p.SupplierId == supplierId.Value);
            }

            var total = await query.CountAsync();
            var orders = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<PurchaseOrderDto>
            {
                Data = orders.Select(MapToDto).ToList(),
                Meta = new PaginationMeta
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = total,
                    TotalPages = (int)Math.Ceiling((double)total / pageSize)
                }
            };
        }

        public async Task<PurchaseOrderDto> GetPurchaseOrderByIdAsync(Guid id)
        {
            var order = await GetBaseQuery().FirstOrDefaultAsync(p => p.Id == id);
            if (order == null)
                throw new KeyNotFoundException($"Purchase order {id} not found.");
            return MapToDto(order);
        }

        public async Task<PurchaseOrderDto> CreatePurchaseOrderAsync(UpsertPurchaseOrderDto dto)
        {
            var businessId = _currentUser.BusinessId!.Value;

            // Validate Supplier
            var supplierExists = await _context.Suppliers.AnyAsync(s => s.Id == dto.SupplierId && s.BusinessId == businessId);
            if (!supplierExists)
                throw new ArgumentException("Invalid supplier ID.");

            // Validate Broker if present
            if (dto.BrokerId.HasValue)
            {
                var brokerExists = await _context.Brokers.AnyAsync(b => b.Id == dto.BrokerId.Value && b.BusinessId == businessId);
                if (!brokerExists)
                    throw new ArgumentException("Invalid broker ID.");
            }

            // Generate OrderNumber if not provided
            var orderNumber = string.IsNullOrWhiteSpace(dto.OrderNumber)
                ? $"PO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}"
                : dto.OrderNumber.Trim();

            // Check duplicate order number
            var exists = await _context.Purchases.AnyAsync(p => p.BusinessId == businessId && p.OrderNumber == orderNumber);
            if (exists)
                throw new InvalidOperationException($"Purchase order number '{orderNumber}' already exists.");

            var order = new PurchaseOrder
            {
                BusinessId = businessId,
                OrderNumber = orderNumber,
                SupplierId = dto.SupplierId,
                BrokerId = dto.BrokerId,
                Notes = dto.Notes,
                Status = PurchaseStatus.Draft,
                PaymentState = PaymentState.Pending,
                DeliveryState = DeliveryState.Pending,
                TaxTotal = dto.TaxTotal
            };

            decimal subtotal = 0;
            foreach (var itemDto in dto.Items)
            {
                var catalogItemExists = await _context.CatalogItems.AnyAsync(c => c.Id == itemDto.CatalogItemId && c.BusinessId == businessId);
                if (!catalogItemExists)
                    throw new ArgumentException($"Invalid catalog item ID {itemDto.CatalogItemId}.");

                var lineTotal = itemDto.OrderedQuantity * itemDto.UnitPrice;
                subtotal += lineTotal;

                order.Items.Add(new PurchaseItem
                {
                    BusinessId = businessId,
                    CatalogItemId = itemDto.CatalogItemId,
                    OrderedQuantity = itemDto.OrderedQuantity,
                    ReceivedQuantity = 0,
                    UnitPrice = itemDto.UnitPrice,
                    LineTotal = lineTotal,
                    Notes = itemDto.Notes
                });
            }

            order.Subtotal = subtotal;
            order.GrandTotal = subtotal + order.TaxTotal;

            _context.Purchases.Add(order);
            await _context.SaveChangesAsync();

            return await GetPurchaseOrderByIdAsync(order.Id);
        }

        public async Task<PurchaseOrderDto> UpdatePurchaseOrderAsync(Guid id, UpsertPurchaseOrderDto dto)
        {
            var businessId = _currentUser.BusinessId!.Value;
            var order = await _context.Purchases
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == id && p.BusinessId == businessId);

            if (order == null)
                throw new KeyNotFoundException($"Purchase order {id} not found.");

            if (order.Status != PurchaseStatus.Draft)
                throw new InvalidOperationException("Only purchase orders in Draft status can be updated.");

            var supplierExists = await _context.Suppliers.AnyAsync(s => s.Id == dto.SupplierId && s.BusinessId == businessId);
            if (!supplierExists)
                throw new ArgumentException("Invalid supplier ID.");

            if (dto.BrokerId.HasValue)
            {
                var brokerExists = await _context.Brokers.AnyAsync(b => b.Id == dto.BrokerId.Value && b.BusinessId == businessId);
                if (!brokerExists)
                    throw new ArgumentException("Invalid broker ID.");
            }

            if (!string.IsNullOrWhiteSpace(dto.OrderNumber) && dto.OrderNumber != order.OrderNumber)
            {
                var duplicate = await _context.Purchases.AnyAsync(p => p.BusinessId == businessId && p.OrderNumber == dto.OrderNumber && p.Id != id);
                if (duplicate)
                    throw new InvalidOperationException($"Purchase order number '{dto.OrderNumber}' already exists.");
                order.OrderNumber = dto.OrderNumber.Trim();
            }

            order.SupplierId = dto.SupplierId;
            order.BrokerId = dto.BrokerId;
            order.Notes = dto.Notes;
            order.TaxTotal = dto.TaxTotal;

            // Remove existing items and add updated ones
            _context.PurchaseItems.RemoveRange(order.Items);
            order.Items.Clear();

            decimal subtotal = 0;
            foreach (var itemDto in dto.Items)
            {
                var catalogItemExists = await _context.CatalogItems.AnyAsync(c => c.Id == itemDto.CatalogItemId && c.BusinessId == businessId);
                if (!catalogItemExists)
                    throw new ArgumentException($"Invalid catalog item ID {itemDto.CatalogItemId}.");

                var lineTotal = itemDto.OrderedQuantity * itemDto.UnitPrice;
                subtotal += lineTotal;

                order.Items.Add(new PurchaseItem
                {
                    BusinessId = businessId,
                    CatalogItemId = itemDto.CatalogItemId,
                    OrderedQuantity = itemDto.OrderedQuantity,
                    ReceivedQuantity = 0,
                    UnitPrice = itemDto.UnitPrice,
                    LineTotal = lineTotal,
                    Notes = itemDto.Notes
                });
            }

            order.Subtotal = subtotal;
            order.GrandTotal = subtotal + order.TaxTotal;

            await _context.SaveChangesAsync();
            return await GetPurchaseOrderByIdAsync(order.Id);
        }

        public async Task DeletePurchaseOrderAsync(Guid id)
        {
            var businessId = _currentUser.BusinessId!.Value;
            var order = await _context.Purchases
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == id && p.BusinessId == businessId);

            if (order == null)
                throw new KeyNotFoundException($"Purchase order {id} not found.");

            if (order.Status != PurchaseStatus.Draft && order.Status != PurchaseStatus.Cancelled)
                throw new InvalidOperationException("Only Draft or Cancelled purchase orders can be deleted.");

            _context.PurchaseItems.RemoveRange(order.Items);
            _context.Purchases.Remove(order);
            await _context.SaveChangesAsync();
        }

        public async Task<PurchaseOrderDto> UpdateStatusAsync(Guid id, PurchaseStatus newStatus)
        {
            var businessId = _currentUser.BusinessId!.Value;
            var order = await _context.Purchases
                .FirstOrDefaultAsync(p => p.Id == id && p.BusinessId == businessId);

            if (order == null)
                throw new KeyNotFoundException($"Purchase order {id} not found.");

            var now = DateTime.UtcNow;

            // Validate status transitions
            if (newStatus == PurchaseStatus.Confirmed && order.Status != PurchaseStatus.Draft)
                throw new InvalidOperationException("Only Draft orders can be confirmed.");
            if (newStatus == PurchaseStatus.Dispatched && order.Status != PurchaseStatus.Confirmed)
                throw new InvalidOperationException("Only Confirmed orders can be dispatched.");
            if (newStatus == PurchaseStatus.Arrived && order.Status != PurchaseStatus.Dispatched && order.Status != PurchaseStatus.Confirmed)
                throw new InvalidOperationException("Order must be dispatched or confirmed before arriving.");

            order.Status = newStatus;
            if (newStatus == PurchaseStatus.Confirmed) order.ConfirmedAt = now;
            else if (newStatus == PurchaseStatus.Dispatched) order.DispatchedAt = now;
            else if (newStatus == PurchaseStatus.Arrived) order.ArrivedAt = now;
            else if (newStatus == PurchaseStatus.Completed) order.CompletedAt = now;

            await _context.SaveChangesAsync();
            return await GetPurchaseOrderByIdAsync(order.Id);
        }

        public async Task<PurchaseOrderDto> ReceiveItemsAsync(Guid id, ReceivePurchaseDto dto)
        {
            var businessId = _currentUser.BusinessId!.Value;
            using var tx = await _context.Database.BeginTransactionAsync();

            var order = await _context.Purchases
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == id && p.BusinessId == businessId);

            if (order == null)
                throw new KeyNotFoundException($"Purchase order {id} not found.");

            if (order.Status == PurchaseStatus.Draft || order.Status == PurchaseStatus.Cancelled)
                throw new InvalidOperationException("Cannot receive items for a Draft or Cancelled purchase order.");

            foreach (var receiveItem in dto.Items)
            {
                var poItem = order.Items.FirstOrDefault(i => i.Id == receiveItem.PurchaseItemId);
                if (poItem == null)
                    throw new ArgumentException($"Purchase item {receiveItem.PurchaseItemId} not found in order {id}.");

                if (receiveItem.ReceivedQuantityDelta <= 0)
                    continue;

                // Update received quantity on PO item
                poItem.ReceivedQuantity += receiveItem.ReceivedQuantityDelta;

                // CRITICAL INVARIANT: Commit stock via StockService
                var catalogItem = await _context.CatalogItems
                    .FirstOrDefaultAsync(c => c.Id == poItem.CatalogItemId && c.BusinessId == businessId);
                if (catalogItem == null)
                    throw new KeyNotFoundException($"Catalog item {poItem.CatalogItemId} not found.");

                await _stockService.AdjustStockAsync(poItem.CatalogItemId, new AdjustStockRequestDto
                {
                    QuantityDelta = receiveItem.ReceivedQuantityDelta,
                    Reason = "PurchaseReceipt",
                    Notes = $"Received PO {order.OrderNumber}. {receiveItem.Notes}".Trim(),
                    ExpectedVersion = catalogItem.RowVersion
                });
            }

            // Evaluate DeliveryState and Status
            bool allFullyReceived = order.Items.All(i => i.ReceivedQuantity >= i.OrderedQuantity);
            bool anyReceived = order.Items.Any(i => i.ReceivedQuantity > 0);

            if (allFullyReceived)
            {
                order.DeliveryState = DeliveryState.Delivered;
                order.Status = PurchaseStatus.Completed;
                order.CompletedAt = DateTime.UtcNow;
            }
            else if (anyReceived)
            {
                order.DeliveryState = DeliveryState.Partial;
                order.Status = PurchaseStatus.Arrived;
                if (!order.ArrivedAt.HasValue) order.ArrivedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return await GetPurchaseOrderByIdAsync(order.Id);
        }
    }
}
