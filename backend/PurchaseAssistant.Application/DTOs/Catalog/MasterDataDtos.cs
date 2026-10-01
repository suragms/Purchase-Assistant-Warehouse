using PurchaseAssistant.Application.DTOs;
using System;
using System.Collections.Generic;

namespace PurchaseAssistant.Application.DTOs.Catalog
{
    public class CatalogItemDto
    {
        public Guid Id { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public Guid? TypeId { get; set; }
        public string? TypeName { get; set; }
        public string DefaultUnit { get; set; } = "PCS";
        [OperationalNumeric]
        public decimal? KgPerUnit { get; set; }
        [OperationalNumeric]
        public decimal ReorderLevel { get; set; }
        [OperationalNumeric]
        public decimal CurrentStock { get; set; }
        public bool IsActive { get; set; }
        public Guid RowVersion { get; set; }
    }

    public class CatalogItemDetailDto : CatalogItemDto
    {
        public Guid? LastSupplierId { get; set; }
        public string? LastSupplierName { get; set; }
        public Guid? LastBrokerId { get; set; }
        public string? LastBrokerName { get; set; }
        public List<VariantDto> Variants { get; set; } = new();
        public List<SupplierSummaryDto> Suppliers { get; set; } = new();
    }

    public class VariantDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? Barcode { get; set; }
        public string? AttributesJson { get; set; }
        public bool IsActive { get; set; }
        [OperationalNumeric]
        public decimal? KgPerUnit { get; set; }
        public Guid RowVersion { get; set; }
    }

    public class SupplierSummaryDto
    {
        public Guid SupplierId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public string? SupplierItemCode { get; set; }
    }

    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ItemCount { get; set; }
    }

    public class CategoryTypeDto
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int ItemCount { get; set; }
    }

    public class SupplierDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public int LinkedItemsCount { get; set; }
    }

    public class BrokerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int LinkedSuppliersCount { get; set; }
    }

    public class GlobalSearchResponseDto
    {
        public List<CatalogItemDto> Items { get; set; } = new();
        public List<SupplierDto> Suppliers { get; set; } = new();
        public List<BrokerDto> Brokers { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = new();
        public List<CategoryTypeDto> Types { get; set; } = new();
    }

    public class PaginatedResult<T>
    {
        public List<T> Data { get; set; } = new();
        public PaginationMeta Meta { get; set; } = new();
    }

    public class PaginationMeta
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public record DuplicateCandidateDto(
        string ItemAId,
        string ItemAName,
        string ItemACode,
        string? ItemABarcode,
        string ItemACategoryId,
        string ItemACategoryName,
        string? ItemATypeName,
        string ItemBId,
        string ItemBName,
        string ItemBCode,
        string? ItemBBarcode,
        string ItemBCategoryId,
        string ItemBCategoryName,
        string? ItemBTypeName,
        int SimilarityScore,
        List<string> MatchReasons
    );
}
