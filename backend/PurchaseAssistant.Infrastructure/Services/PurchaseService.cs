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
            DiscountPercent = i.DiscountPercent,
            TaxPercent = i.TaxPercent,
            KgPerUnit = i.KgPerUnit,
            LandingCostPerKg = i.LandingCostPerKg,
            LineTotal = i.LineTotal,
            Notes = i.Notes
        };

        private static PurchaseOrderDto MapToDto(PurchaseOrder p) => new()
        {
            Id = p.Id,
            Version = p.Version,
            OrderNumber = p.OrderNumber,
            SupplierId = p.SupplierId,
            SupplierName = p.Supplier?.Name ?? string.Empty,
            BrokerId = p.BrokerId,
            BrokerName = p.Broker?.Name,
            Status = p.Status,
            PaymentState = GetPaymentState(p),
            DeliveryState = p.DeliveryState,
            Notes = p.Notes,
            Subtotal = p.Subtotal,
            TaxTotal = p.TaxTotal,
            GrandTotal = p.GrandTotal,
            PaidAmount = p.PaidAmount,
            RemainingAmount = Math.Max(0, p.GrandTotal - p.PaidAmount),
            PaidAt = p.PaidAt,
            PaymentDays = p.PaymentDays,
            DueDate = GetDueDate(p),
            ConfirmedAt = p.ConfirmedAt,
            DispatchedAt = p.DispatchedAt,
            ArrivedAt = p.ArrivedAt,
            VerifiedAt = p.VerifiedAt,
            VerifiedById = p.VerifiedById,
            CompletedAt = p.CompletedAt,
            CreatedAt = p.CreatedAt,
            Items = p.Items.Select(MapItemToDto).ToList()
        };

        private static DateOnly? GetDueDate(PurchaseOrder p) => p.PaymentDays.HasValue
            ? DateOnly.FromDateTime(p.CreatedAt).AddDays(p.PaymentDays.Value) : null;

        private static PaymentState GetPaymentState(PurchaseOrder p)
        {
            if (p.Status is PurchaseStatus.Draft or PurchaseStatus.Cancelled || p.GrandTotal <= 0) return p.PaymentState;
            if (p.PaidAmount >= p.GrandTotal) return PaymentState.Paid;
            if (GetDueDate(p) is DateOnly due)
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                if (today > due) return PaymentState.Overdue;
                if (due.DayNumber - today.DayNumber <= 3) return PaymentState.DueSoon;
            }
            return p.PaidAmount > 0 ? PaymentState.Partial : PaymentState.Pending;
        }

        public async Task<PurchaseOrderDto> UpdatePaymentAsync(Guid id, UpdatePurchasePaymentDto dto)
        {
            if (_currentUser.Role != "Owner") throw new UnauthorizedAccessException("Only the business owner can record payments.");
            if (!dto.ExpectedVersion.HasValue) throw new ArgumentException("Refresh this purchase before recording payment; expectedVersion is required.");
            if (dto.PaidAmount < 0 || dto.PaidAmount > PurchaseInputLimits.MaxValue || decimal.Round(dto.PaidAmount, 4) != dto.PaidAmount)
                throw new ArgumentException("Enter a nonnegative paid amount within range, with at most four decimal places.");
            var order = await GetBaseQuery().FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new KeyNotFoundException("Purchase order not found.");
            ValidateVersion(order, dto.ExpectedVersion);
            if (order.Status is PurchaseStatus.Draft or PurchaseStatus.Cancelled)
                throw new InvalidOperationException("Payments can only be recorded for a confirmed, active purchase.");
            var previousState = GetPaymentState(order);
            // Reference payment patch records a cumulative total and clamps it to the invoice total.
            order.PaidAmount = Math.Min(dto.PaidAmount, order.GrandTotal);
            order.PaidAt = DateTime.UtcNow; order.UpdatedAt = order.PaidAt;
            order.PaymentState = GetPaymentState(order);
            AddActivity(order, "PurchasePaymentUpdated", new { fromPaymentState = previousState.ToString(), toPaymentState = order.PaymentState.ToString() });
            await _context.SaveChangesAsync();
            return await GetPurchaseOrderByIdAsync(order.Id);
        }

        public async Task<PaginatedResult<PurchaseOrderDto>> GetPurchaseOrdersAsync(
            int page, int pageSize, string? search, PurchaseStatus? status, Guid? supplierId)
        {
            page = Math.Clamp(page, 1, 10000);
            pageSize = Math.Clamp(pageSize, 1, 100);
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
            PurchaseInputLimits.Validate(dto);
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
            await LoadCatalogAsync(dto, businessId);
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
                PaymentDays = dto.PaymentDays,
                Notes = dto.Notes,
                Status = PurchaseStatus.Draft,
                PaymentState = PaymentState.Pending,
                DeliveryState = DeliveryState.Pending
            };

            decimal subtotal = 0;
            foreach (var itemDto in dto.Items)
            {
                var lineTotal = PurchaseInputLimits.LineTotal(itemDto);
                subtotal += PurchaseInputLimits.LineSubtotal(itemDto);
                order.TaxTotal += PurchaseInputLimits.LineTax(itemDto);

                order.Items.Add(new PurchaseItem
                {
                    BusinessId = businessId,
                    CatalogItemId = itemDto.CatalogItemId,
                    OrderedQuantity = itemDto.OrderedQuantity,
                    ReceivedQuantity = 0,
                    UnitPrice = itemDto.UnitPrice,
                    DiscountPercent = itemDto.DiscountPercent,
                    TaxPercent = itemDto.TaxPercent,
                    KgPerUnit = itemDto.KgPerUnit,
                    LandingCostPerKg = itemDto.LandingCostPerKg,
                    LineTotal = lineTotal,
                    Notes = itemDto.Notes
                });
            }

            order.Subtotal = subtotal;
            order.GrandTotal = subtotal + order.TaxTotal;

            _context.Purchases.Add(order);
            AddActivity(order, "PurchaseDraftCreated", new { toStatus = "Draft" });
            await _context.SaveChangesAsync();

            return await GetPurchaseOrderByIdAsync(order.Id);
        }

        public async Task<PurchasePreviewDto> PreviewAsync(UpsertPurchaseOrderDto dto)
        {
            PurchaseInputLimits.Validate(dto);
            var businessId = _currentUser.BusinessId!.Value;
            if (!await _context.Suppliers.AnyAsync(s => s.Id == dto.SupplierId && s.BusinessId == businessId))
                throw new ArgumentException("Choose a supplier in the current business.");
            if (dto.BrokerId.HasValue && !await _context.Brokers.AnyAsync(b => b.Id == dto.BrokerId && b.BusinessId == businessId))
                throw new ArgumentException("Choose a broker in the current business.");
            var catalog = await LoadCatalogAsync(dto, businessId);
            var result = new PurchasePreviewDto { TaxTotal = dto.Items.Sum(PurchaseInputLimits.LineTax) };
            result.Items = dto.Items.Select(i => new PurchaseItemDto
            {
                CatalogItemId = i.CatalogItemId, ItemCode = catalog[i.CatalogItemId].ItemCode,
                CatalogItemName = catalog[i.CatalogItemId].Name, OrderedQuantity = i.OrderedQuantity,
                UnitPrice = i.UnitPrice, DiscountPercent = i.DiscountPercent, TaxPercent = i.TaxPercent,
                KgPerUnit = i.KgPerUnit, LandingCostPerKg = i.LandingCostPerKg, LineTotal = PurchaseInputLimits.LineTotal(i), Notes = i.Notes
            }).ToList();
            result.Subtotal = dto.Items.Sum(PurchaseInputLimits.LineSubtotal);
            result.GrandTotal = result.Subtotal + result.TaxTotal;
            return result;
        }

        private void AddActivity(PurchaseOrder order, string eventType, object details) =>
            _context.SecurityAuditLogs.Add(new SecurityAuditLog { BusinessId = order.BusinessId, UserId = _currentUser.UserId,
                EventType = eventType, Description = $"PurchaseOrder:{order.Id}",
                MetadataJson = System.Text.Json.JsonSerializer.Serialize(details) });

        private async Task<Dictionary<Guid, CatalogItem>> LoadCatalogAsync(UpsertPurchaseOrderDto dto, Guid businessId)
        {
            var ids = dto.Items.Select(i => i.CatalogItemId).Distinct().ToList();
            var catalog = await _context.CatalogItems.AsNoTracking().Where(i => ids.Contains(i.Id) && i.BusinessId == businessId)
                .ToDictionaryAsync(i => i.Id);
            if (catalog.Count != ids.Count) throw new ArgumentException("Choose catalog items in the current business.");
            return catalog;
        }

        public async Task<List<PurchaseActivityDto>> GetActivityAsync(Guid id)
        {
            if (!await _context.Purchases.AnyAsync(p => p.Id == id && p.BusinessId == _currentUser.BusinessId))
                throw new KeyNotFoundException("Purchase order not found.");
            return await _context.SecurityAuditLogs.AsNoTracking().Where(a => a.BusinessId == _currentUser.BusinessId
                && a.Description == $"PurchaseOrder:{id}" && a.EventType.StartsWith("Purchase"))
                .OrderByDescending(a => a.CreatedAt).Take(100).Select(a => new PurchaseActivityDto {
                    Id = a.Id, EventType = a.EventType, UserId = a.UserId, CreatedAt = a.CreatedAt, DetailsJson = a.MetadataJson
                }).ToListAsync();
        }

        public async Task<PurchaseOrderDto> UpdatePurchaseOrderAsync(Guid id, UpsertPurchaseOrderDto dto)
        {
            PurchaseInputLimits.Validate(dto);
            var businessId = _currentUser.BusinessId!.Value;
            var order = await _context.Purchases
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == id && p.BusinessId == businessId);

            if (order == null)
                throw new KeyNotFoundException($"Purchase order {id} not found.");

            if (order.Status != PurchaseStatus.Draft)
                throw new InvalidOperationException("Only purchase orders in Draft status can be updated.");

            ValidateVersion(order, dto.ExpectedVersion);
            if (_currentUser.Role != "Owner" && (dto.PaymentDays != order.PaymentDays || (order.TaxTotal > 0 && order.Items.All(i => i.TaxPercent == 0)) || dto.Items.Any(input =>
                !order.Items.Any(existing => existing.CatalogItemId == input.CatalogItemId && existing.UnitPrice == input.UnitPrice
                    && existing.KgPerUnit == input.KgPerUnit && existing.LandingCostPerKg == input.LandingCostPerKg
                    && existing.TaxPercent == input.TaxPercent && existing.DiscountPercent == input.DiscountPercent))))
                throw new UnauthorizedAccessException("Only an owner can change existing purchase financial values.");

            var supplierExists = await _context.Suppliers.AnyAsync(s => s.Id == dto.SupplierId && s.BusinessId == businessId);
            if (!supplierExists)
                throw new ArgumentException("Invalid supplier ID.");

            if (dto.BrokerId.HasValue)
            {
                var brokerExists = await _context.Brokers.AnyAsync(b => b.Id == dto.BrokerId.Value && b.BusinessId == businessId);
                if (!brokerExists)
                    throw new ArgumentException("Invalid broker ID.");
            }

            await LoadCatalogAsync(dto, businessId);
            if (!string.IsNullOrWhiteSpace(dto.OrderNumber) && dto.OrderNumber != order.OrderNumber)
            {
                var duplicate = await _context.Purchases.AnyAsync(p => p.BusinessId == businessId && p.OrderNumber == dto.OrderNumber && p.Id != id);
                if (duplicate)
                    throw new InvalidOperationException($"Purchase order number '{dto.OrderNumber}' already exists.");
                order.OrderNumber = dto.OrderNumber.Trim();
            }

            order.SupplierId = dto.SupplierId;
            order.BrokerId = dto.BrokerId;
            order.PaymentDays = dto.PaymentDays;
            order.Notes = dto.Notes;
            order.UpdatedAt = DateTime.UtcNow;
            order.TaxTotal = 0;

            // Remove existing items and add updated ones
            _context.PurchaseItems.RemoveRange(order.Items);
            order.Items.Clear();

            decimal subtotal = 0;
            foreach (var itemDto in dto.Items)
            {
                var lineTotal = PurchaseInputLimits.LineTotal(itemDto);
                subtotal += PurchaseInputLimits.LineSubtotal(itemDto);
                order.TaxTotal += PurchaseInputLimits.LineTax(itemDto);

                order.Items.Add(new PurchaseItem
                {
                    BusinessId = businessId,
                    CatalogItemId = itemDto.CatalogItemId,
                    OrderedQuantity = itemDto.OrderedQuantity,
                    ReceivedQuantity = 0,
                    UnitPrice = itemDto.UnitPrice,
                    DiscountPercent = itemDto.DiscountPercent,
                    TaxPercent = itemDto.TaxPercent,
                    KgPerUnit = itemDto.KgPerUnit,
                    LandingCostPerKg = itemDto.LandingCostPerKg,
                    LineTotal = lineTotal,
                    Notes = itemDto.Notes
                });
            }

            order.Subtotal = subtotal;
            order.GrandTotal = subtotal + order.TaxTotal;

            AddActivity(order, "PurchaseDraftUpdated", new { toStatus = "Draft" });
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

            if (order.PaidAmount > 0) throw new InvalidOperationException("A purchase with a recorded payment cannot be deleted. Keep it for the payment audit trail.");

            _context.PurchaseItems.RemoveRange(order.Items);
            AddActivity(order, "PurchaseDraftDeleted", new { fromStatus = order.Status.ToString() });
            _context.Purchases.Remove(order);
            await _context.SaveChangesAsync();
        }

        private static void ValidateVersion(PurchaseOrder order, uint? expectedVersion)
        {
            if (expectedVersion.HasValue && order.Version != expectedVersion.Value)
                throw new InvalidOperationException("PURCHASE_VERSION_CONFLICT");
        }

        public async Task<PurchaseOrderDto> UpdateStatusAsync(Guid id, PurchaseStatus newStatus, uint? expectedVersion = null)
        {
            var businessId = _currentUser.BusinessId!.Value;
            var order = await _context.Purchases
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == id && p.BusinessId == businessId);

            if (order == null)
                throw new KeyNotFoundException($"Purchase order {id} not found.");

            var now = DateTime.UtcNow;
            ValidateVersion(order, expectedVersion);
            var allowed = (order.Status, newStatus) switch
            {
                (PurchaseStatus.Draft, PurchaseStatus.Confirmed) => true,
                (PurchaseStatus.Confirmed, PurchaseStatus.Dispatched) => true,
                (PurchaseStatus.Dispatched, PurchaseStatus.Arrived) => true,
                (PurchaseStatus.Arrived, PurchaseStatus.Verified) => true,
                (_, PurchaseStatus.Cancelled) => order.Status is PurchaseStatus.Draft or PurchaseStatus.Confirmed or PurchaseStatus.Dispatched
                    && order.Items.All(i => i.ReceivedQuantity == 0),
                _ => false
            };
            if (!allowed)
                throw new InvalidOperationException("This purchase status transition is not allowed. Complete receiving to finish an order.");

            var previousStatus = order.Status;
            order.Status = newStatus;
            order.UpdatedAt = now;
            if (newStatus == PurchaseStatus.Confirmed) order.ConfirmedAt = now;
            else if (newStatus == PurchaseStatus.Dispatched) order.DispatchedAt = now;
            else if (newStatus == PurchaseStatus.Arrived) order.ArrivedAt = now;
            else if (newStatus == PurchaseStatus.Verified)
            {
                order.VerifiedAt = now;
                order.VerifiedById = _currentUser.UserId;
            }
            else if (newStatus == PurchaseStatus.Completed) order.CompletedAt = now;

            AddActivity(order, "PurchaseStatusChanged", new { fromStatus = previousStatus.ToString(), toStatus = newStatus.ToString() });

            await _context.SaveChangesAsync();
            return await GetPurchaseOrderByIdAsync(order.Id);
        }

        public async Task<PurchaseOrderDto> ReceiveItemsAsync(Guid id, ReceivePurchaseDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0 || dto.Items.Count > 200 || dto.Items.Any(i => i == null || !PurchaseInputLimits.IsQuantityValid(i.ReceivedQuantityDelta))
                || dto.Items.Select(i => i.PurchaseItemId).Distinct().Count() != dto.Items.Count)
                throw new ArgumentException("Provide unique purchase lines with positive receiving quantities and at most four decimal places.");
            var businessId = _currentUser.BusinessId!.Value;
            using var tx = await _context.Database.BeginTransactionAsync();

            var order = await _context.Purchases
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == id && p.BusinessId == businessId);

            if (order == null)
                throw new KeyNotFoundException($"Purchase order {id} not found.");

            ValidateVersion(order, dto.ExpectedVersion);
            if (order.Status != PurchaseStatus.Verified || !order.VerifiedAt.HasValue)
                throw new InvalidOperationException("Mark this purchase arrived and explicitly verify it before committing received quantities to stock.");

            // Validate the complete batch before any ledger mutation or save.
            foreach (var received in dto.Items)
            {
                var line = order.Items.FirstOrDefault(i => i.Id == received.PurchaseItemId)
                    ?? throw new ArgumentException("A receiving line does not belong to this purchase.");
                if (received.ReceivedQuantityDelta > line.OrderedQuantity - line.ReceivedQuantity)
                    throw new InvalidOperationException("Received quantity exceeds the remaining ordered quantity.");
            }

            // Even partial receipts that retain the same lifecycle state must advance xmin.
            order.UpdatedAt = DateTime.UtcNow;

            foreach (var receiveItem in dto.Items)
            {
                var poItem = order.Items.FirstOrDefault(i => i.Id == receiveItem.PurchaseItemId);
                if (poItem == null)
                    throw new ArgumentException($"Purchase item {receiveItem.PurchaseItemId} not found in order {id}.");

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
                    ReferenceType = "PurchaseOrder",
                    ReferenceId = order.Id.ToString(),
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
                // Partial receipts remain verified; a later explicit receipt can commit the balance.
            }

            AddActivity(order, "PurchaseReceived", new { fromStatus = "Verified", toStatus = order.Status.ToString(),
                lines = dto.Items.Select(i => new { i.PurchaseItemId, i.ReceivedQuantityDelta }).ToArray() });
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return await GetPurchaseOrderByIdAsync(order.Id);
        }
    }
}
