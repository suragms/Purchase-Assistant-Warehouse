using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs.Catalog;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Infrastructure.Data;

namespace PurchaseAssistant.Infrastructure.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly AppDbContext _context;
        private readonly IEntityNormalizationService _normalization;
        private readonly ICurrentUserService _currentUser;

        public CatalogService(AppDbContext context, IEntityNormalizationService normalization, ICurrentUserService currentUser)
        {
            _context = context;
            _normalization = normalization;
            _currentUser = currentUser;
        }

        public async Task<PaginatedResult<CatalogItemDto>> GetAllAsync(int page = 1, int pageSize = 50, string? search = null, Guid? categoryId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.CatalogItems.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var normalizedSearch = search.Trim().ToLowerInvariant();
                query = query.Where(i => i.Name.ToLower().Contains(normalizedSearch) || i.ItemCode.ToLower().Contains(normalizedSearch));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(i => i.CategoryId == categoryId.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(i => i.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new CatalogItemDto
                {
                    Id = i.Id,
                    ItemCode = i.ItemCode,
                    Barcode = i.Barcode,
                    Name = i.Name,
                    CategoryId = i.CategoryId,
                    CategoryName = i.Category.Name,
                    TypeId = i.TypeId,
                    TypeName = i.Type != null ? i.Type.Name : null,
                    DefaultUnit = i.DefaultUnit,
                    KgPerUnit = i.KgPerUnit,
                    ReorderLevel = i.ReorderLevel,
                    CurrentStock = i.CurrentStock,
                    IsActive = i.IsActive,
                    RowVersion = i.RowVersion
                })
                .ToListAsync(cancellationToken);

            return new PaginatedResult<CatalogItemDto>
            {
                Data = items,
                Meta = new PaginationMeta
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                }
            };
        }

        public async Task<CatalogItemDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var item = await _context.CatalogItems
                .Include(i => i.Category)
                .Include(i => i.Type)
                .Include(i => i.LastSupplier)
                .Include(i => i.LastBroker)
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

            if (item == null) return null;

            return new CatalogItemDetailDto
            {
                Id = item.Id,
                ItemCode = item.ItemCode,
                Barcode = item.Barcode,
                Name = item.Name,
                CategoryId = item.CategoryId,
                CategoryName = item.Category.Name,
                TypeId = item.TypeId,
                TypeName = item.Type?.Name,
                DefaultUnit = item.DefaultUnit,
                KgPerUnit = item.KgPerUnit,
                ReorderLevel = item.ReorderLevel,
                CurrentStock = item.CurrentStock,
                IsActive = item.IsActive,
                RowVersion = item.RowVersion,
                LastSupplierId = item.LastSupplierId,
                LastSupplierName = item.LastSupplier?.Name,
                LastBrokerId = item.LastBrokerId,
                LastBrokerName = item.LastBroker?.Name
            };
        }

        public async Task<CatalogItemDto> CreateAsync(CatalogItemDto dto, CancellationToken cancellationToken = default)
        {
            var item = new CatalogItem
            {
                BusinessId = _currentUser.BusinessId ?? throw new InvalidOperationException("BUSINESS_CONTEXT_REQUIRED"),
                ItemCode = _normalization.NormalizeItemCode(dto.ItemCode),
                Barcode = _normalization.NormalizeBarcode(dto.Barcode),
                Name = dto.Name.Trim(),
                CategoryId = dto.CategoryId,
                TypeId = dto.TypeId,
                DefaultUnit = dto.DefaultUnit,
                KgPerUnit = dto.KgPerUnit,
                ReorderLevel = dto.ReorderLevel,
                IsActive = dto.IsActive
            };

            _context.CatalogItems.Add(item);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                // This might be due to unique constraint on ItemCode or Barcode
                throw new InvalidOperationException("DUPLICATE_ITEM_CODE_OR_BARCODE");
            }

            return new CatalogItemDto { Id = item.Id, ItemCode = item.ItemCode, Name = item.Name };
        }

        public async Task<CatalogItemDto> UpdateAsync(Guid id, CatalogItemDto dto, CancellationToken cancellationToken = default)
        {
            var item = await _context.CatalogItems.FindAsync(new object[] { id }, cancellationToken);
            if (item == null) throw new KeyNotFoundException("CATALOG_ITEM_NOT_FOUND");

            if (item.RowVersion != dto.RowVersion) throw new InvalidOperationException("CATALOG_ITEM_VERSION_CONFLICT");

            item.ItemCode = _normalization.NormalizeItemCode(dto.ItemCode);
            item.Barcode = _normalization.NormalizeBarcode(dto.Barcode);
            item.Name = dto.Name.Trim();
            item.CategoryId = dto.CategoryId;
            item.TypeId = dto.TypeId;
            item.DefaultUnit = dto.DefaultUnit;
            item.KgPerUnit = dto.KgPerUnit;
            item.ReorderLevel = dto.ReorderLevel;
            item.IsActive = dto.IsActive;
            item.RowVersion = Guid.NewGuid();

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                throw new InvalidOperationException("DUPLICATE_ITEM_CODE_OR_BARCODE");
            }

            return new CatalogItemDto { Id = item.Id, ItemCode = item.ItemCode, Name = item.Name };
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var item = await _context.CatalogItems.FindAsync(new object[] { id }, cancellationToken);
            if (item == null) throw new KeyNotFoundException("CATALOG_ITEM_NOT_FOUND");

            // Should check if it's in use

            _context.CatalogItems.Remove(item);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
