using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SportMatrix.AIAssistant.Infrastructure.Configuration;
using SportMatrix.AIAssistant.Infrastructure.Services;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SportMatrix.AIAssistant.Tests.Infrastructure.Services;

public class GoogleGeminiServiceTests
{
    [Fact]
    public void Constructor_WithValidOptions_CreatesInstance()
    {
        var mockLogger = new Mock<ILogger<GoogleGeminiService>>();
        var options = new GoogleAIOptions { ApiKey = "test-key" };
        var mockOptions = Options.Create(options);

        var service = new GoogleGeminiService(mockOptions, mockLogger.Object);

        Assert.NotNull(service);
    }

    [Fact]
    public async Task GetFitnessAnalysisAsync_WithInvalidApiKey_ThrowsOrReturnsFallback()
    {
        var mockLogger = new Mock<ILogger<GoogleGeminiService>>();
        var options = new GoogleAIOptions { ApiKey = "invalid-key" };
        var mockOptions = Options.Create(options);

        var service = new GoogleGeminiService(mockOptions, mockLogger.Object);

        // With an invalid API key, the service should handle the error gracefully
        var result = await service.GetFitnessAnalysisAsync("test prompt", CancellationToken.None);

        // The service returns a fallback response when the API call fails
        Assert.NotNull(result);
    }
}
