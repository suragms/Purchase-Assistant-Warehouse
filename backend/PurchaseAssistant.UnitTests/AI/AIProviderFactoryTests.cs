using Moq;
using PurchaseAssistant.Application.DTOs.AI;
using PurchaseAssistant.Application.Interfaces.AI;
using PurchaseAssistant.Infrastructure.Services.AI;
using Xunit;

namespace PurchaseAssistant.UnitTests.AI
{
    public class AIProviderFactoryTests
    {
        private sealed class CaptureHandler : HttpMessageHandler
        {
            public string? Key { get; private set; }
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
            {
                Key = request.Headers.Authorization?.Parameter;
                return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new StringContent("{\"choices\":[{\"message\":{\"content\":\"candidate\"}}]}") });
            }
        }
        [Fact]
        public async Task ConfiguredBusinessKeyIsUsedByExistingProviderTransport()
        {
            var resolver = new Mock<IProviderCredentialResolver>(); resolver.Setup(x => x.ResolveAsync("openai_key", It.IsAny<CancellationToken>())).ReturnsAsync("business-test-key");
            var handler = new CaptureHandler(); using var client = new HttpClient(handler);
            var factory = new AIProviderFactory([], resolver.Object, _ => client);
            var provider = await factory.GetProviderAsync(AIProviderType.OpenAI);
            Assert.True((await provider.SendRequestAsync(new AIRequest("Test"))).Success); Assert.Equal("business-test-key", handler.Key);
        }
        [Fact]
        public async Task MissingBusinessKeyPreservesServerFallbackAndStubNeverFabricatesSuccess()
        {
            var resolver = new Mock<IProviderCredentialResolver>(); resolver.Setup(x => x.ResolveAsync("openai_key", It.IsAny<CancellationToken>())).ReturnsAsync((string?)null);
            var provider = new Mock<IAIProvider>(); provider.Setup(x => x.ProviderType).Returns(AIProviderType.OpenAI);
            var factory = new AIProviderFactory([provider.Object], resolver.Object);
            Assert.Same(provider.Object, await factory.GetProviderAsync(AIProviderType.OpenAI));
            var stub = await new StubAIProvider().SendRequestAsync(new AIRequest("Test")); Assert.False(stub.Success); Assert.Null(stub.Content);
        }
        [Fact]
        public void ExternalProviderWithoutKeyReportsUnconfigured()
        {
            using var client = new HttpClient(new CaptureHandler());
            var provider = new OpenAIProvider(client, "  ");
            Assert.False(((IAIProviderReadiness)provider).IsConfigured);
        }
        [Fact]
        public async Task RoutingSkipsUnconfiguredProviderWithoutSendingRequest()
        {
            var handler = new CaptureHandler(); using var client = new HttpClient(handler);
            var provider = new OpenAIProvider(client, "");
            var factory = new Mock<IAIProviderFactory>();
            factory.Setup(x => x.GetProviderAsync(AIProviderType.OpenAI, It.IsAny<CancellationToken>())).ReturnsAsync(provider);
            factory.Setup(x => x.GetProviderAsync(It.Is<AIProviderType>(type => type != AIProviderType.OpenAI), It.IsAny<CancellationToken>())).ThrowsAsync(new NotSupportedException());
            var service = new AIRoutingService(factory.Object, Mock.Of<Microsoft.Extensions.Logging.ILogger<AIRoutingService>>(), Microsoft.Extensions.Options.Options.Create(new AiOptions { Enabled = true }));

            var result = await service.ExecuteWithFailoverAsync(new AIRequest("Test"));

            Assert.False(result.Success);
            Assert.Null(handler.Key);
        }
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
