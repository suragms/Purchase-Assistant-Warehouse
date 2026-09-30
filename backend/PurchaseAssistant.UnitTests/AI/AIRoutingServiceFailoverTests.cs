using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PurchaseAssistant.Application.DTOs.AI;
using PurchaseAssistant.Application.Interfaces.AI;
using PurchaseAssistant.Infrastructure.Services.AI;
using Xunit;

namespace PurchaseAssistant.UnitTests.AI
{
    public class AIRoutingServiceFailoverTests
    {
        [Fact]
        public async Task ExecuteWithFailoverAsync_FallsBackToNextProvider_WhenPrimaryFails()
        {
            // Arrange
            var primaryProvider = new Mock<IAIProvider>();
            primaryProvider.Setup(p => p.ProviderType).Returns(AIProviderType.OpenRouter);
            primaryProvider.Setup(p => p.SendRequestAsync(It.IsAny<AIRequest>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new AIResponse(false, null, "Primary Error", "OpenRouter", "Model", 0));

            var fallbackProvider = new Mock<IAIProvider>();
            fallbackProvider.Setup(p => p.ProviderType).Returns(AIProviderType.Gemini);
            fallbackProvider.Setup(p => p.SendRequestAsync(It.IsAny<AIRequest>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new AIResponse(true, "Success", null, "Gemini", "Model", 0));

            var mockFactory = new Mock<IAIProviderFactory>();
            mockFactory.Setup(f => f.GetProvider(AIProviderType.OpenRouter)).Returns(primaryProvider.Object);
            mockFactory.Setup(f => f.GetProvider(AIProviderType.Gemini)).Returns(fallbackProvider.Object);

            var options = Options.Create(new AiOptions { Enabled = true });
            var mockLogger = new Mock<ILogger<AIRoutingService>>();
            var service = new AIRoutingService(mockFactory.Object, mockLogger.Object, options);

            // Act
            var response = await service.ExecuteWithFailoverAsync(new AIRequest("Test"));

            // Assert
            Assert.True(response.Success);
            Assert.Equal("Gemini", response.Provider);
            fallbackProvider.Verify(p => p.SendRequestAsync(It.IsAny<AIRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
