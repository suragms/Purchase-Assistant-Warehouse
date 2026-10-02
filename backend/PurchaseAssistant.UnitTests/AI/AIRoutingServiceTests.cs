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
        [Theory]
        [InlineData(true)] [InlineData(false)]
        public async Task RealFailoverRecordsOneLogicalRequestWithoutPassingThePrompt(bool success)
        {
            var factory = new Mock<IAIProviderFactory>(); var provider = new Mock<IAIProvider>(); var usage = new Mock<IAIUsageRecorder>();
            usage.Setup(x => x.RecordAsync(It.IsAny<AIResponse>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            factory.Setup(x => x.GetProviderAsync(It.IsAny<AIProviderType>(), It.IsAny<CancellationToken>())).ThrowsAsync(new NotSupportedException());
            factory.Setup(x => x.GetProviderAsync(AIProviderType.Gemini, It.IsAny<CancellationToken>())).ReturnsAsync(provider.Object);
            provider.Setup(x => x.SendRequestAsync(It.IsAny<AIRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new AIResponse(success, "PRIVATE CONTENT", "private-key", "Gemini", "model", 3));
            var service = new AIRoutingService(factory.Object, Mock.Of<ILogger<AIRoutingService>>(), Options.Create(new AiOptions { Enabled = true }), usage.Object);
            Assert.Equal(success, (await service.ExecuteWithFailoverAsync(new AIRequest("PRIVATE PROMPT"))).Success);
            usage.Verify(x => x.RecordAsync(It.Is<AIResponse>(r => r.LatencyMs >= 0), false, It.IsAny<CancellationToken>()), Times.Once);
        }
        [Fact]
        public async Task DisabledAiDoesNotCreateAnInventedRequestCount()
        {
            var usage = new Mock<IAIUsageRecorder>(); var service = new AIRoutingService(Mock.Of<IAIProviderFactory>(), Mock.Of<ILogger<AIRoutingService>>(), Options.Create(new AiOptions { Enabled = false }), usage.Object);
            await service.ExecuteWithFailoverAsync(new AIRequest("private")); usage.VerifyNoOtherCalls();
        }
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
