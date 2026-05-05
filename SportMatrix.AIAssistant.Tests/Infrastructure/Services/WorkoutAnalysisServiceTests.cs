using Microsoft.Extensions.Logging;
using Moq;
using SportMatrix.AIAssistant.Application.DTOs;
using SportMatrix.AIAssistant.Application.Interfaces;
using SportMatrix.AIAssistant.Infrastructure.Services;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SportMatrix.AIAssistant.Tests.Infrastructure.Services;

public class WorkoutAnalysisServiceTests
{
    [Fact]
    public async Task AnalyzeWorkoutsAsync_ReturnsResult()
    {
        var mockPromptService = new Mock<IAIPromptService>();
        mockPromptService.Setup(s => s.GetFitnessAnalysisAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync("Test analysis");

        var mockLogger = new Mock<ILogger<WorkoutAnalysisService>>();

        var service = new WorkoutAnalysisService(mockPromptService.Object, mockLogger.Object);
        var request = new WorkoutAnalysisRequestDto { AnalysisType = "Performance" };

        var result = await service.AnalyzeWorkoutsAsync(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Gemini-AI", result.Provider);
    }
}
