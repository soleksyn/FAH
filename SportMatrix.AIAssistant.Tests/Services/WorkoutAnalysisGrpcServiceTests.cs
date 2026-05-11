namespace SportMatrix.AIAssistant.Tests.Services;

using FluentAssertions;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;
using SportMatrix.AIAssistant.Application.DTOs;
using SportMatrix.AIAssistant.Application.Interfaces;
using SportMatrix.AIAssistant.UI.API.Services;
using Xunit;

public class WorkoutAnalysisGrpcServiceTests
{
    private readonly Mock<IWorkoutAnalysisService> _analysisServiceMock;
    private readonly Mock<IWorkoutDataProvider> _dataProviderMock;
    private readonly Mock<ILogger<WorkoutAnalysisGrpcService>> _loggerMock;
    private readonly WorkoutAnalysisGrpcService _grpcService;

    public WorkoutAnalysisGrpcServiceTests()
    {
        _analysisServiceMock = new Mock<IWorkoutAnalysisService>();
        _dataProviderMock = new Mock<IWorkoutDataProvider>();
        _loggerMock = new Mock<ILogger<WorkoutAnalysisGrpcService>>();
        
        _grpcService = new WorkoutAnalysisGrpcService(
            _analysisServiceMock.Object,
            _dataProviderMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CheckHealth_ShouldReturnHealthy()
    {
        // Arrange
        var request = new Sportmatrix.HealthCheckRequest();
        var context = new Mock<ServerCallContext>().Object;

        // Act
        var response = await _grpcService.CheckHealth(request, context);

        // Assert
        response.IsHealthy.Should().BeTrue();
        response.Message.Should().Contain("healthy");
    }

    [Fact]
    public async Task GetPerformanceTrends_ShouldCallAnalysisService()
    {
        // Arrange
        var request = new Sportmatrix.PerformanceTrendsRequest { AthleteId = 1, TimeFrame = "month" };
        var contextMock = new Mock<ServerCallContext>();
        
        var workouts = new List<WorkoutDataDto> 
        { 
            new() { Date = DateTime.UtcNow, ActivityType = "Run" } 
        };
        
        _dataProviderMock
            .Setup(x => x.GetRecentWorkoutsAsync(It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(workouts);

        _analysisServiceMock
            .Setup(x => x.AnalyzeWorkoutsAsync(It.IsAny<WorkoutAnalysisRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WorkoutAnalysisResponseDto 
            { 
                Analysis = "Trends analysis", 
                GeneratedAt = DateTime.UtcNow 
            });

        // Act
        var response = await _grpcService.GetPerformanceTrends(request, contextMock.Object);

        // Assert
        response.Should().NotBeNull();
        response.Analysis.Should().Be("Trends analysis");
        _analysisServiceMock.Verify(x => x.AnalyzeWorkoutsAsync(It.IsAny<WorkoutAnalysisRequestDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
