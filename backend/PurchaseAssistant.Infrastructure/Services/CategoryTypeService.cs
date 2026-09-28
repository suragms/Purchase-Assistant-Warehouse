using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs.Catalog;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Infrastructure.Data;

namespace PurchaseAssistant.Infrastructure.Services
{
    public class CategoryTypeService : ICategoryTypeService
    {
        private readonly AppDbContext _context;
        private readonly IEntityNormalizationService _normalization;
        private readonly ICurrentUserService _currentUser;

        public CategoryTypeService(AppDbContext context, IEntityNormalizationService normalization, ICurrentUserService currentUser)
        {
            _context = context;
            _normalization = normalization;
            _currentUser = currentUser;
        }

        public async Task<List<CategoryTypeDto>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            var types = await _context.CategoryTypes
                .Include(t => t.Category)
                .Where(t => t.CategoryId == categoryId)
                .OrderBy(t => t.Name)
                .ToListAsync(cancellationToken);

            var result = new List<CategoryTypeDto>();
            foreach (var type in types)
            {
                var count = await _context.CatalogItems.CountAsync(i => i.TypeId == type.Id, cancellationToken);
                result.Add(new CategoryTypeDto
                {
                    Id = type.Id,
                    CategoryId = type.CategoryId,
                    CategoryName = type.Category.Name,
                    Name = type.Name,
                    ItemCount = count
                });
            }
            return result;
        }

        public async Task<CategoryTypeDto> CreateAsync(Guid categoryId, string name, CancellationToken cancellationToken = default)
        {
            var normalizedName = _normalization.NormalizeName(name);
            var businessId = _currentUser.BusinessId ?? throw new InvalidOperationException("BUSINESS_CONTEXT_REQUIRED");

            var existing = await _context.CategoryTypes
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.BusinessId == businessId && t.CategoryId == categoryId && t.Name.ToLower() == normalizedName, cancellationToken);

            if (existing != null)
            {
                throw new InvalidOperationException("CATEGORY_TYPE_EXISTS");
            }

            var category = await _context.Categories.FindAsync(new object[] { categoryId }, cancellationToken);
            if (category == null) throw new KeyNotFoundException("CATEGORY_NOT_FOUND");

            var type = new CategoryType
            {
                BusinessId = businessId,
                CategoryId = categoryId,
                Name = name.Trim()
            };

            _context.CategoryTypes.Add(type);
            await _context.SaveChangesAsync(cancellationToken);

            return new CategoryTypeDto
            {
                Id = type.Id,
                CategoryId = type.CategoryId,
                CategoryName = category.Name,
                Name = type.Name,
                ItemCount = 0
            };
        }

        public async Task<CategoryTypeDto> UpdateAsync(Guid id, string name, CancellationToken cancellationToken = default)
        {
            var type = await _context.CategoryTypes
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

            if (type == null) throw new KeyNotFoundException("CATEGORY_TYPE_NOT_FOUND");

            type.Name = name.Trim();
            type.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            var count = await _context.CatalogItems.CountAsync(i => i.TypeId == type.Id, cancellationToken);

            return new CategoryTypeDto
            {
                Id = type.Id,
                CategoryId = type.CategoryId,
                CategoryName = type.Category.Name,
                Name = type.Name,
                ItemCount = count
            };
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var type = await _context.CategoryTypes.FindAsync(new object[] { id }, cancellationToken);
            if (type == null) throw new KeyNotFoundException("CATEGORY_TYPE_NOT_FOUND");

            var hasItems = await _context.CatalogItems.AnyAsync(i => i.TypeId == id, cancellationToken);
            if (hasItems)
            {
                throw new InvalidOperationException("CATEGORY_TYPE_IN_USE");
            }

            _context.CategoryTypes.Remove(type);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
