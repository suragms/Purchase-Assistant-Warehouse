using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs.Catalog;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Infrastructure.Data;

namespace PurchaseAssistant.Infrastructure.Services
{
    public class GlobalSearchService : IGlobalSearchService
    {
        private readonly AppDbContext _context;

        public GlobalSearchService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GlobalSearchResponseDto> SearchAsync(string query, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new GlobalSearchResponseDto();
            }

            var cleanQuery = query.Trim().ToLowerInvariant();

            // Perform queries in parallel or sequence for each domain
            var items = await _context.CatalogItems
                .Include(i => i.Category)
                .Where(i => i.Name.ToLower().Contains(cleanQuery) || i.ItemCode.ToLower().Contains(cleanQuery) || (i.Barcode != null && i.Barcode.Contains(cleanQuery)))
                .Take(10)
                .Select(i => new CatalogItemDto
                {
                    Id = i.Id,
                    ItemCode = i.ItemCode,
                    Barcode = i.Barcode,
                    Name = i.Name,
                    CategoryId = i.CategoryId,
                    CategoryName = i.Category.Name,
                    DefaultUnit = i.DefaultUnit,
                    CurrentStock = i.CurrentStock,
                    IsActive = i.IsActive
                })
                .ToListAsync(cancellationToken);

            var suppliers = await _context.Suppliers
                .Where(s => s.Name.ToLower().Contains(cleanQuery) || (s.Phone != null && s.Phone.Contains(cleanQuery)))
                .Take(10)
                .Select(s => new SupplierDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Phone = s.Phone,
                    Address = s.Address,
                    IsActive = s.IsActive
                })
                .ToListAsync(cancellationToken);

            var brokers = await _context.Brokers
                .Where(b => b.Name.ToLower().Contains(cleanQuery))
                .Take(10)
                .Select(b => new BrokerDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    IsActive = b.IsActive
                })
                .ToListAsync(cancellationToken);

            var categories = await _context.Categories
                .Where(c => c.Name.ToLower().Contains(cleanQuery))
                .Take(10)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync(cancellationToken);

            var types = await _context.CategoryTypes
                .Include(t => t.Category)
                .Where(t => t.Name.ToLower().Contains(cleanQuery))
                .Take(5)
                .Select(t => new CategoryTypeDto
                {
                    Id = t.Id,
                    CategoryId = t.CategoryId,
                    CategoryName = t.Category.Name,
                    Name = t.Name,
                    ItemCount = 0
                })
                .ToListAsync(cancellationToken);

            return new GlobalSearchResponseDto
            {
                Items = items,
                Suppliers = suppliers,
                Brokers = brokers,
                Categories = categories,
                Types = types
            };
        }
    }
}
