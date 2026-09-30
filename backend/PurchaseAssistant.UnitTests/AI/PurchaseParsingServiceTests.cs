using Moq;
using Microsoft.Extensions.Logging;
using PurchaseAssistant.Application.DTOs.Purchase;
using PurchaseAssistant.Application.DTOs.Catalog;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Application.Interfaces.AI;
using PurchaseAssistant.Infrastructure.Services.AI;
using Xunit;
using PurchaseAssistant.Application.DTOs.AI;

namespace PurchaseAssistant.UnitTests.AI;

public class PurchaseParsingServiceTests
{
    private readonly Mock<IAIRoutingService> _mockRoutingService;
    private readonly Mock<ICatalogService> _mockCatalogService;
    private readonly Mock<ISupplierService> _mockSupplierService;
    private readonly Mock<ILogger<PurchaseParsingService>> _mockLogger;
    private readonly PurchaseParsingService _service;

    public PurchaseParsingServiceTests()
    {
        _mockRoutingService = new Mock<IAIRoutingService>();
        _mockCatalogService = new Mock<ICatalogService>();
        _mockSupplierService = new Mock<ISupplierService>();
        _mockLogger = new Mock<ILogger<PurchaseParsingService>>();
        _service = new PurchaseParsingService(
            _mockRoutingService.Object,
            _mockCatalogService.Object,
            _mockSupplierService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task ParseAsync_ReturnsError_WhenRoutingFails()
    {
        _mockRoutingService.Setup(r => r.ExecuteWithFailoverAsync(It.IsAny<AIRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AIResponse(false, null, "Error", "Provider", "Model", 0));

        var result = await _service.ParseAsync("Buy rice");

        Assert.Equal(IntentStatus.Error, result.Status);
    }

    [Fact]
    public async Task ParseAsync_ResolvesItem_WhenExactMatchFound()
    {
        var jsonResponse = @"{""Status"": ""Success"", ""Items"": [{""CatalogItemName"": ""Rice"", ""RequestedQuantity"": 10}]}";
        _mockRoutingService.Setup(r => r.ExecuteWithFailoverAsync(It.IsAny<AIRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AIResponse(true, jsonResponse, null, "Provider", "Model", 0));

        _mockCatalogService.Setup(c => c.GetAllAsync(1, 50, "Rice", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedResult<CatalogItemDto> { Data = new List<CatalogItemDto> { new CatalogItemDto { Id = Guid.NewGuid(), Name = "Rice" } } });

        var result = await _service.ParseAsync("Buy rice");

        Assert.Equal(IntentStatus.Success, result.Status);
        Assert.False(result.Items[0].IsAmbiguous);
    }
}
