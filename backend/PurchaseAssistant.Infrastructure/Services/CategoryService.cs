using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs.Catalog;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Infrastructure.Data;

namespace PurchaseAssistant.Infrastructure.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<CategoryDto> CreateAsync(string name, CancellationToken cancellationToken = default);
        Task<CategoryDto> UpdateAsync(Guid id, string name, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }

    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;
        private readonly IEntityNormalizationService _normalization;
        private readonly ICurrentUserService _currentUser;

        public CategoryService(AppDbContext context, IEntityNormalizationService normalization, ICurrentUserService currentUser)
        {
            _context = context;
            _normalization = normalization;
            _currentUser = currentUser;
        }

        public async Task<List<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);

            var result = new List<CategoryDto>();
            foreach (var cat in categories)
            {
                var count = await _context.CatalogItems.CountAsync(i => i.CategoryId == cat.Id, cancellationToken);
                result.Add(new CategoryDto
                {
                    Id = cat.Id,
                    Name = cat.Name,
                    ItemCount = count
                });
            }
            return result;
        }

        public async Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var cat = await _context.Categories.FindAsync(new object[] { id }, cancellationToken);
            if (cat == null) return null;

            var count = await _context.CatalogItems.CountAsync(i => i.CategoryId == cat.Id, cancellationToken);
            return new CategoryDto
            {
                Id = cat.Id,
                Name = cat.Name,
                ItemCount = count
            };
        }

        public async Task<CategoryDto> CreateAsync(string name, CancellationToken cancellationToken = default)
        {
            var normalizedName = _normalization.NormalizeName(name);
            var businessId = _currentUser.BusinessId ?? throw new InvalidOperationException("BUSINESS_CONTEXT_REQUIRED");

            var existing = await _context.Categories
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.BusinessId == businessId && c.Name.ToLower() == normalizedName, cancellationToken);

            if (existing != null)
            {
                throw new InvalidOperationException("CATEGORY_EXISTS");
            }

            var category = new Category
            {
                BusinessId = businessId,
                Name = name.Trim()
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);

            return new CategoryDto { Id = category.Id, Name = category.Name, ItemCount = 0 };
        }

        public async Task<CategoryDto> UpdateAsync(Guid id, string name, CancellationToken cancellationToken = default)
        {
            var category = await _context.Categories.FindAsync(new object[] { id }, cancellationToken);
            if (category == null) throw new KeyNotFoundException("CATEGORY_NOT_FOUND");

            category.Name = name.Trim();
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            var count = await _context.CatalogItems.CountAsync(i => i.CategoryId == category.Id, cancellationToken);

            return new CategoryDto { Id = category.Id, Name = category.Name, ItemCount = count };
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var category = await _context.Categories.FindAsync(new object[] { id }, cancellationToken);
            if (category == null) throw new KeyNotFoundException("CATEGORY_NOT_FOUND");

            var hasItems = await _context.CatalogItems.AnyAsync(i => i.CategoryId == id, cancellationToken);
            if (hasItems)
            {
                throw new InvalidOperationException("CATEGORY_IN_USE");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
