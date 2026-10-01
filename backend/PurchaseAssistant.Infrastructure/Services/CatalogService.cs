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
            page = Math.Clamp(page, 1, 10000);
            pageSize = Math.Clamp(pageSize, 1, 200);
            var query = _context.CatalogItems.AsNoTracking();

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
                LastBrokerName = item.LastBroker?.Name,
                Variants = await GetVariantsAsync(id, cancellationToken)
            };
        }

        public async Task<CatalogItemDto?> GetByBarcodeAsync(string barcode)
        {
            var normalized = barcode.Trim().ToLowerInvariant();
            var item = await _context.CatalogItems
                .Include(i => i.Category)
                .Include(i => i.Type)
                .FirstOrDefaultAsync(i => i.Barcode != null && i.Barcode.ToLower() == normalized);

            if (item == null) return null;

            return new CatalogItemDto
            {
                Id = item.Id,
                ItemCode = item.ItemCode,
                Barcode = item.Barcode,
                Name = item.Name,
                CategoryId = item.CategoryId,
                CategoryName = item.Category.Name,
                TypeId = item.TypeId,
                TypeName = item.Type != null ? item.Type.Name : null,
                DefaultUnit = item.DefaultUnit,
                KgPerUnit = item.KgPerUnit,
                ReorderLevel = item.ReorderLevel,
                CurrentStock = item.CurrentStock,
                IsActive = item.IsActive,
                RowVersion = item.RowVersion
            };
        }

        public async Task<CatalogItemDto> CreateAsync(CatalogItemDto dto, CancellationToken cancellationToken = default)
        {
            await ValidateReferencesAsync(dto, cancellationToken);
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
            var item = await _context.CatalogItems.FirstOrDefaultAsync(i => i.Id == id && i.BusinessId == _currentUser.BusinessId, cancellationToken);
            if (item == null) throw new KeyNotFoundException("CATALOG_ITEM_NOT_FOUND");

            if (item.RowVersion != dto.RowVersion) throw new InvalidOperationException("CATALOG_ITEM_VERSION_CONFLICT");

            await ValidateReferencesAsync(dto, cancellationToken);

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
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("CATALOG_ITEM_VERSION_CONFLICT");
            }
            catch (DbUpdateException)
            {
                throw new InvalidOperationException("DUPLICATE_ITEM_CODE_OR_BARCODE");
            }

            return new CatalogItemDto { Id = item.Id, ItemCode = item.ItemCode, Name = item.Name };
        }

        public async Task<List<VariantDto>> GetVariantsAsync(Guid itemId, CancellationToken cancellationToken = default)
        {
            await RequireVariantParentAsync(itemId, cancellationToken);
            return await _context.CatalogVariants.AsNoTracking()
                .Where(v => v.CatalogItemId == itemId && v.BusinessId == _currentUser.BusinessId)
                .OrderBy(v => v.Name).Select(v => new VariantDto { Id = v.Id, Name = v.Name, Code = v.Code,
                    Barcode = v.Barcode, AttributesJson = v.AttributesJson, IsActive = v.IsActive,
                    KgPerUnit = v.KgPerUnit, RowVersion = v.RowVersion }).ToListAsync(cancellationToken);
        }

        public async Task<VariantDto> CreateVariantAsync(Guid itemId, VariantDto dto, CancellationToken cancellationToken = default)
        {
            await RequireVariantParentAsync(itemId, cancellationToken);
            await ValidateVariantAsync(itemId, dto, null, cancellationToken);
            var variant = new CatalogVariant { BusinessId = _currentUser.BusinessId!.Value, CatalogItemId = itemId,
                Name = dto.Name.Trim(), KgPerUnit = dto.KgPerUnit, IsActive = true };
            _context.CatalogVariants.Add(variant);
            await SaveVariantAsync(cancellationToken);
            return ToVariantDto(variant);
        }

        public async Task<VariantDto> UpdateVariantAsync(Guid itemId, Guid variantId, VariantDto dto, CancellationToken cancellationToken = default)
        {
            var variant = await RequireVariantAsync(itemId, variantId, cancellationToken);
            if (dto.RowVersion == Guid.Empty) throw new ArgumentException("Reload the variant before editing it.");
            if (variant.RowVersion != dto.RowVersion) throw new InvalidOperationException("VARIANT_VERSION_CONFLICT");
            await ValidateVariantAsync(itemId, dto, variantId, cancellationToken);
            variant.Name = dto.Name.Trim(); variant.KgPerUnit = dto.KgPerUnit;
            variant.UpdatedAt = DateTime.UtcNow; variant.RowVersion = Guid.NewGuid();
            await SaveVariantAsync(cancellationToken);
            return ToVariantDto(variant);
        }

        public async Task DeleteVariantAsync(Guid itemId, Guid variantId, Guid expectedVersion, CancellationToken cancellationToken = default)
        {
            if (_currentUser.Role != "Owner") throw new UnauthorizedAccessException("Only the business owner can delete variants.");
            var variant = await RequireVariantAsync(itemId, variantId, cancellationToken);
            if (expectedVersion == Guid.Empty) throw new ArgumentException("Reload the variant before deleting it.");
            if (variant.RowVersion != expectedVersion) throw new InvalidOperationException("VARIANT_VERSION_CONFLICT");
            _context.CatalogVariants.Remove(variant);
            await SaveVariantAsync(cancellationToken);
        }

        private async Task RequireVariantParentAsync(Guid itemId, CancellationToken cancellationToken)
        {
            if (!_currentUser.BusinessId.HasValue) throw new UnauthorizedAccessException("Select a business first.");
            if (!await _context.CatalogItems.AnyAsync(i => i.Id == itemId && i.BusinessId == _currentUser.BusinessId, cancellationToken))
                throw new KeyNotFoundException("Catalog item not found.");
        }

        private async Task<CatalogVariant> RequireVariantAsync(Guid itemId, Guid variantId, CancellationToken cancellationToken)
        {
            await RequireVariantParentAsync(itemId, cancellationToken);
            return await _context.CatalogVariants.FirstOrDefaultAsync(v => v.Id == variantId && v.CatalogItemId == itemId
                && v.BusinessId == _currentUser.BusinessId, cancellationToken) ?? throw new KeyNotFoundException("Variant not found.");
        }

        private async Task ValidateVariantAsync(Guid itemId, VariantDto dto, Guid? exclude, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Trim().Length > 512)
                throw new ArgumentException("Enter a variant name of 1 to 512 characters.");
            if (dto.KgPerUnit.HasValue && (dto.KgPerUnit <= 0 || dto.KgPerUnit > PurchaseAssistant.Application.DTOs.Purchase.PurchaseInputLimits.MaxValue || decimal.Round(dto.KgPerUnit.Value, 4) != dto.KgPerUnit))
                throw new ArgumentException("Weight must be positive, within the supported numeric range and have at most four decimal places.");
            var normalized = dto.Name.Trim().ToLowerInvariant();
            if (await _context.CatalogVariants.AnyAsync(v => v.BusinessId == _currentUser.BusinessId && v.CatalogItemId == itemId
                && v.Id != exclude && v.Name.Trim().ToLower() == normalized, cancellationToken))
                throw new InvalidOperationException("DUPLICATE_VARIANT_NAME");
        }

        private async Task SaveVariantAsync(CancellationToken cancellationToken)
        {
            try { await _context.SaveChangesAsync(cancellationToken); }
            catch (DbUpdateConcurrencyException) { throw new InvalidOperationException("VARIANT_VERSION_CONFLICT"); }
            catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException { SqlState: "23505" })
            { throw new InvalidOperationException("DUPLICATE_VARIANT_NAME"); }
        }

        private static VariantDto ToVariantDto(CatalogVariant v) => new() { Id = v.Id, Name = v.Name, Code = v.Code,
            Barcode = v.Barcode, AttributesJson = v.AttributesJson, IsActive = v.IsActive, KgPerUnit = v.KgPerUnit, RowVersion = v.RowVersion };

        private async Task ValidateReferencesAsync(CatalogItemDto dto, CancellationToken cancellationToken)
        {
            var businessId = _currentUser.BusinessId ?? throw new InvalidOperationException("BUSINESS_CONTEXT_REQUIRED");
            if (!await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId && c.BusinessId == businessId, cancellationToken))
                throw new ArgumentException("Choose a category in the current business.");
            if (dto.TypeId.HasValue && !await _context.CategoryTypes.AnyAsync(t => t.Id == dto.TypeId && t.BusinessId == businessId && t.CategoryId == dto.CategoryId, cancellationToken))
                throw new ArgumentException("Choose a type belonging to the selected category in the current business.");
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var item = await _context.CatalogItems.FirstOrDefaultAsync(i => i.Id == id && i.BusinessId == _currentUser.BusinessId, cancellationToken);
            if (item == null) throw new KeyNotFoundException("CATALOG_ITEM_NOT_FOUND");

            if (await _context.StockMovements.AnyAsync(m => m.CatalogItemId == id, cancellationToken)
                || await _context.PurchaseItems.AnyAsync(i => i.CatalogItemId == id, cancellationToken))
                throw new InvalidOperationException("CATALOG_ITEM_IN_USE");

            _context.CatalogItems.Remove(item);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task ArchiveAsync(Guid id, Guid expectedVersion, CancellationToken cancellationToken = default)
        {
            var item = await _context.CatalogItems.FirstOrDefaultAsync(i => i.Id == id && i.BusinessId == _currentUser.BusinessId, cancellationToken)
                ?? throw new KeyNotFoundException("Catalog item not found.");
            if (expectedVersion == Guid.Empty) throw new ArgumentException("Reload the item before archiving it.");
            if (item.RowVersion != expectedVersion) throw new InvalidOperationException("CATALOG_ITEM_VERSION_CONFLICT");
            item.IsActive = false; item.UpdatedAt = DateTime.UtcNow; item.RowVersion = Guid.NewGuid();
            try { await _context.SaveChangesAsync(cancellationToken); }
            catch (DbUpdateConcurrencyException) { throw new InvalidOperationException("CATALOG_ITEM_VERSION_CONFLICT"); }
        }

        public async Task<List<DuplicateCandidateDto>> GetDuplicateCandidatesAsync(int? minSimilarity = 70)
        {
            var activeItems = await _context.CatalogItems
                .Include(i => i.Category)
                .Include(i => i.Type)
                .Where(i => i.IsActive)
                .ToListAsync();

            var duplicates = new List<DuplicateCandidateDto>();
            var threshold = minSimilarity ?? 70;

            for (int i = 0; i < activeItems.Count; i++)
            {
                for (int j = i + 1; j < activeItems.Count; j++)
                {
                    var itemA = activeItems[i];
                    var itemB = activeItems[j];
                    if (itemA.Id.CompareTo(itemB.Id) > 0)
                    {
                        var temp = itemA;
                        itemA = itemB;
                        itemB = temp;
                    }

                    var score = 0;
                    var reasons = new List<string>();

                    bool sameCategory = itemA.CategoryId == itemB.CategoryId;
                    if (sameCategory)
                    {
                        score += 40;
                        reasons.Add("SameCategory");
                    }

                    bool samePrefix = false;
                    bool nameContains = false;

                    string nameA = itemA.Name.ToLowerInvariant();
                    string nameB = itemB.Name.ToLowerInvariant();

                    if (nameA.Contains(nameB) || nameB.Contains(nameA))
                    {
                        nameContains = true;
                    }

                    if (nameA.Length >= 4 && nameB.Length >= 4 && nameA.Substring(0, 4) == nameB.Substring(0, 4))
                    {
                        samePrefix = true;
                    }

                    if (nameContains)
                    {
                        score += 40;
                        if (!reasons.Contains("SimilarName")) reasons.Add("SimilarName");
                    }
                    else if (samePrefix)
                    {
                        score += 30;
                        if (!reasons.Contains("SimilarName")) reasons.Add("SimilarName");
                    }

                    if (!string.IsNullOrEmpty(itemA.ItemCode) && !string.IsNullOrEmpty(itemB.ItemCode) &&
                        itemA.ItemCode.Length >= 6 && itemB.ItemCode.Length >= 6 &&
                        itemA.ItemCode.Substring(0, 6).Equals(itemB.ItemCode.Substring(0, 6), StringComparison.OrdinalIgnoreCase))
                    {
                        score += 30;
                        reasons.Add("IdenticalCodePrefix");
                    }

                    if ((sameCategory && (nameContains || samePrefix)) || reasons.Contains("IdenticalCodePrefix"))
                    {
                        if (score >= threshold)
                        {
                            duplicates.Add(new DuplicateCandidateDto(
                                itemA.Id.ToString(),
                                itemA.Name,
                                itemA.ItemCode,
                                itemA.Barcode,
                                itemA.CategoryId.ToString(),
                                itemA.Category.Name,
                                itemA.Type?.Name,
                                itemB.Id.ToString(),
                                itemB.Name,
                                itemB.ItemCode,
                                itemB.Barcode,
                                itemB.CategoryId.ToString(),
                                itemB.Category.Name,
                                itemB.Type?.Name,
                                score,
                                reasons
                            ));
                        }
                    }
                }
            }

            return duplicates;
        }
    }
}
