using Moq;
using PurchaseAssistant.Application.DTOs.AI;
using PurchaseAssistant.Application.Interfaces.AI;
using PurchaseAssistant.Infrastructure.Services.AI;
using Xunit;

namespace PurchaseAssistant.UnitTests.AI
{
    public class AIProviderFactoryTests
    {
        [Theory]
        [InlineData(AIProviderType.OpenAI)]
        [InlineData(AIProviderType.Gemini)]
        [InlineData(AIProviderType.Groq)]
        [InlineData(AIProviderType.OpenRouter)]
        [InlineData(AIProviderType.Stub)]
        public void GetProvider_ReturnsCorrectProvider_WhenRegistered(AIProviderType type)
        {
            // Arrange
            var mockProvider = new Mock<IAIProvider>();
            mockProvider.Setup(p => p.ProviderType).Returns(type);
            var factory = new AIProviderFactory(new[] { mockProvider.Object });

            // Act
            var provider = factory.GetProvider(type);

            // Assert
            Assert.Equal(type, provider.ProviderType);
        }

        [Fact]
        public void GetProvider_ThrowsNotSupportedException_WhenNotRegistered()
        {
            // Arrange
            var factory = new AIProviderFactory(Enumerable.Empty<IAIProvider>());

            // Act & Assert
            Assert.Throws<NotSupportedException>(() => factory.GetProvider(AIProviderType.OpenAI));
        }
    }
}
