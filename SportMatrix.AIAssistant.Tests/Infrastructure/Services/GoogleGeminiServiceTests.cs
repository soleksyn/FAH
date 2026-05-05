using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SportMatrix.AIAssistant.Infrastructure.Configuration;
using SportMatrix.AIAssistant.Infrastructure.Services;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SportMatrix.AIAssistant.Tests.Infrastructure.Services;

public class GoogleGeminiServiceTests
{
    [Fact]
    public async Task GetFitnessAnalysisAsync_ReturnsExpectedResult()
    {
        var mockLogger = new Mock<ILogger<GoogleGeminiService>>();
        var options = new GoogleAIOptions { ApiKey = "test-key" };
        var mockOptions = Options.Create(options);
        var httpClient = new HttpClient(new MockHttpMessageHandler())
        {
            BaseAddress = new System.Uri("https://test.com")
        };

        var service = new GoogleGeminiService(httpClient, mockOptions, mockLogger.Object);

        // This would actually make a request if the handler wasn't mocked properly,
        // but since we return fallback on error, we can just test that.
        var result = await service.GetFitnessAnalysisAsync("test", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Contains("System currently unavailable", result);
    }
}

public class MockHttpMessageHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest));
    }
}
