using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs.Catalog;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Infrastructure.Data;

namespace PurchaseAssistant.Infrastructure.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly AppDbContext _context;
        private readonly IEntityNormalizationService _normalization;
        private readonly ICurrentUserService _currentUser;

        public SupplierService(AppDbContext context, IEntityNormalizationService normalization, ICurrentUserService currentUser)
        {
            _context = context;
            _normalization = normalization;
            _currentUser = currentUser;
        }

        public async Task<List<SupplierDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var suppliers = await _context.Suppliers
                .OrderBy(s => s.Name)
                .ToListAsync(cancellationToken);

            var result = new List<SupplierDto>();
            foreach (var sup in suppliers)
            {
                var count = await _context.SupplierItems.CountAsync(i => i.SupplierId == sup.Id, cancellationToken);
                result.Add(new SupplierDto
                {
                    Id = sup.Id,
                    Name = sup.Name,
                    Phone = sup.Phone,
                    Address = sup.Address,
                    Notes = sup.Notes,
                    IsActive = sup.IsActive,
                    LinkedItemsCount = count
                });
            }
            return result;
        }

        public async Task<SupplierDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var sup = await _context.Suppliers.FindAsync(new object[] { id }, cancellationToken);
            if (sup == null) return null;

            var count = await _context.SupplierItems.CountAsync(i => i.SupplierId == sup.Id, cancellationToken);
            return new SupplierDto
            {
                Id = sup.Id,
                Name = sup.Name,
                Phone = sup.Phone,
                Address = sup.Address,
                Notes = sup.Notes,
                IsActive = sup.IsActive,
                LinkedItemsCount = count
            };
        }

        public async Task<SupplierDto> CreateAsync(SupplierDto dto, CancellationToken cancellationToken = default)
        {
            var normalizedName = _normalization.NormalizeName(dto.Name);
            var businessId = _currentUser.BusinessId ?? throw new InvalidOperationException("BUSINESS_CONTEXT_REQUIRED");

            var existing = await _context.Suppliers
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(s => s.BusinessId == businessId && s.Name.ToLower() == normalizedName, cancellationToken);

            if (existing != null)
            {
                throw new InvalidOperationException("SUPPLIER_EXISTS");
            }

            var supplier = new Supplier
            {
                BusinessId = businessId,
                Name = dto.Name.Trim(),
                Phone = _normalization.NormalizePhone(dto.Phone),
                Address = dto.Address?.Trim(),
                Notes = dto.Notes?.Trim(),
                IsActive = dto.IsActive
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync(cancellationToken);

            return new SupplierDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Phone = supplier.Phone,
                Address = supplier.Address,
                Notes = supplier.Notes,
                IsActive = supplier.IsActive,
                LinkedItemsCount = 0
            };
        }

        public async Task<SupplierDto> UpdateAsync(Guid id, SupplierDto dto, CancellationToken cancellationToken = default)
        {
            var supplier = await _context.Suppliers.FindAsync(new object[] { id }, cancellationToken);
            if (supplier == null) throw new KeyNotFoundException("SUPPLIER_NOT_FOUND");

            supplier.Name = dto.Name.Trim();
            supplier.Phone = _normalization.NormalizePhone(dto.Phone);
            supplier.Address = dto.Address?.Trim();
            supplier.Notes = dto.Notes?.Trim();
            supplier.IsActive = dto.IsActive;
            supplier.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            var count = await _context.SupplierItems.CountAsync(i => i.SupplierId == supplier.Id, cancellationToken);

            return new SupplierDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Phone = supplier.Phone,
                Address = supplier.Address,
                Notes = supplier.Notes,
                IsActive = supplier.IsActive,
                LinkedItemsCount = count
            };
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var supplier = await _context.Suppliers.FindAsync(new object[] { id }, cancellationToken);
            if (supplier == null) throw new KeyNotFoundException("SUPPLIER_NOT_FOUND");

            // Check if in use in purchase orders (for now just check supplier items or let relational integrity handle)
            var hasItems = await _context.SupplierItems.AnyAsync(i => i.SupplierId == id, cancellationToken);
            if (hasItems)
            {
                throw new InvalidOperationException("SUPPLIER_IN_USE");
            }

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
