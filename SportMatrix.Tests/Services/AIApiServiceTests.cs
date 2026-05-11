namespace SportMatrix.Tests.Services;

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Moq;
using Moq.Protected;
using SportMatrix.Frontend.Models.ApiClient;
using SportMatrix.Frontend.Services;
using Xunit;

public class AIApiServiceTests
{
    private readonly Mock<HttpMessageHandler> _handlerMock;
    private readonly HttpClient _httpClient;
    private readonly AIApiService _aiApiService;
    private readonly DemoDataService _demoDataService;

    public AIApiServiceTests()
    {
        _handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        _httpClient = new HttpClient(_handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };
        _demoDataService = new DemoDataService();
        _aiApiService = new AIApiService(_httpClient, _demoDataService);
    }

    [Fact]
    public async Task AnalyzeWorkoutAsync_WhenSuccessful_ShouldReturnAnalysis()
    {
        // Arrange
        var responseDto = new WorkoutAnalysisResponseDto
        {
            Analysis = "Great progress!",
            KeyInsights = new List<string> { "Consistent pace" },
            Recommendations = new List<string> { "Keep it up" }
        };

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(responseDto)
            });

        var workouts = new List<WorkoutDataDto>
        {
            new() { Date = "2026-05-10", ActivityType = "Run", Distance = 5.0, MovingTime = "00:30:00", Calories = 300 }
        };

        // Act
        var result = await _aiApiService.AnalyzeWorkoutAsync(1, workouts);

        // Assert
        result.Should().NotBeNull();
        result!.Analysis.Should().Be("Great progress!");
        result.KeyInsights.Should().Contain("Consistent pace");
    }

    [Fact]
    public async Task AnalyzeWorkoutAsync_WhenApiFails_ShouldReturnNull()
    {
        // Arrange
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

        // Act
        var result = await _aiApiService.AnalyzeWorkoutAsync(1, new List<WorkoutDataDto>());

        // Assert
        result.Should().BeNull();
    }
}
