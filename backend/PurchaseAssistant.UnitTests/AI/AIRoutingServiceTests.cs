using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PurchaseAssistant.Application.DTOs.AI;
using PurchaseAssistant.Application.Interfaces.AI;
using PurchaseAssistant.Infrastructure.Services.AI;
using Xunit;
using Moq;

namespace PurchaseAssistant.UnitTests.AI
{
    public class AIRoutingServiceTests
    {
        [Fact]
        public async Task ExecuteWithFailoverAsync_ReturnsDisabled_WhenAiOptionsDisabled()
        {
            // Arrange
            var options = Options.Create(new AiOptions { Enabled = false });
            var mockFactory = new Mock<IAIProviderFactory>();
            var mockLogger = new Mock<ILogger<AIRoutingService>>();
            var service = new AIRoutingService(mockFactory.Object, mockLogger.Object, options);
            var request = new AIRequest("Test prompt");

            // Act
            var response = await service.ExecuteWithFailoverAsync(request);

            // Assert
            Assert.False(response.Success);
            Assert.Equal("AI_DISABLED", response.Error);
            mockFactory.Verify(f => f.GetProvider(It.IsAny<AIProviderType>()), Times.Never);
        }
    }
}
