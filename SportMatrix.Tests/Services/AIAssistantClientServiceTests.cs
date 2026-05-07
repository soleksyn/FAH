namespace SportMatrix.Tests.Services;

using System.Net;
using System.Text;
using System.Text.Json;
using SportMatrix.Application.DTOs;
using SportMatrix.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

public class AIAssistantClientServiceTests : IDisposable
{
    private readonly Mock<ILogger<AIAssistantClientService>> mockLogger;
    private readonly Mock<IConfiguration> mockConfiguration;
    private readonly Mock<HttpMessageHandler> mockHttpMessageHandler;
    private readonly HttpClient httpClient;
    private readonly AIAssistantClientService service;

    public AIAssistantClientServiceTests()
    {
        this.mockLogger = new Mock<ILogger<AIAssistantClientService>>();
        this.mockConfiguration = new Mock<IConfiguration>();
        this.mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        // Setup configuration mock
        this.mockConfiguration.Setup(x => x["AIAssistant:BaseUrl"])
            .Returns("http://localhost:5169");

        // Create HttpClient with mocked handler
        this.httpClient = new HttpClient(this.mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://localhost:5169"),
        };

        this.service = new AIAssistantClientService(this.httpClient, this.mockLogger.Object, this.mockConfiguration.Object);
    }


    #region GetWorkoutAnalysisAsync Tests

    [Fact]
    public async Task GetWorkoutAnalysisAsync_WithValidRequest_ReturnsSuccessfulResponse()
    {
        // Arrange
        AIWorkoutAnalysisRequestDto request = new AIWorkoutAnalysisRequestDto
        {
            AthleteProfile = new AIAthleteProfileDto
            {
                Name = "Mike Johnson",
                FitnessLevel = "Advanced",
                PrimaryGoal = "Performance Improvement",
            },
            RecentWorkouts = new List<AIWorkoutDataDto>
            {
                new AIWorkoutDataDto
                {
                    Date = DateTime.Now.AddDays(-1),
                    ActivityType = "Run",
                    Distance = 10.0,
                    Duration = 2700,
                    Calories = 650,
                },
                new AIWorkoutDataDto
                {
                    Date = DateTime.Now.AddDays(-3),
                    ActivityType = "Ride",
                    Distance = 30.0,
                    Duration = 5400,
                    Calories = 1200,
                },
            },
            AnalysisType = "Performance",
            FocusAreas = new List<string> { "endurance", "pacing", "recovery" },
        };

        var expectedApiResponse = new
        {
            analysis = "Your recent workouts show excellent endurance progression.",
            keyInsights = new[]
            {
                "Average pace has improved by 15 seconds per kilometer",
                "Heart rate zones indicate optimal training intensity",
                "Recovery time between sessions is appropriate",
            },
            recommendations = new[]
            {
                "Continue current endurance base building",
                "Add one tempo run per week",
                "Consider incorporating strength training",
            },
        };

        string jsonResponse = JsonSerializer.Serialize(expectedApiResponse);
        HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json"),
        };

        this.mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri.ToString().Contains("/api/WorkoutAnalysis/analyze/googlegemini")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        AIWorkoutAnalysisResponseDto result = await this.service.GetWorkoutAnalysisAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedApiResponse.analysis, result.Analysis);
        Assert.Equal(expectedApiResponse.keyInsights.Length, result.KeyInsights.Count);
        Assert.Equal(expectedApiResponse.recommendations.Length, result.Recommendations.Count);
        Assert.Contains("Average pace has improved by 15 seconds per kilometer", result.KeyInsights);
        Assert.Contains("Continue current endurance base building", result.Recommendations);
        Assert.Equal("AIAssistant-HuggingFace", result.Source);

        // Verify logging
        this.VerifyLogCalled(LogLevel.Information, "Requesting workout analysis for 2 workouts, type: Performance");
    }

    [Fact]
    public async Task GetWorkoutAnalysisAsync_WithNoWorkouts_ReturnsSuccessfulResponse()
    {
        // Arrange
        AIWorkoutAnalysisRequestDto request = new AIWorkoutAnalysisRequestDto
        {
            AthleteProfile = new AIAthleteProfileDto
            {
                Name = "New User",
                FitnessLevel = "Beginner",
                PrimaryGoal = "Get Started",
            },
            RecentWorkouts = new List<AIWorkoutDataDto>(), // Empty list
            AnalysisType = "General",
        };

        var expectedApiResponse = new
        {
            analysis = "Welcome to your fitness journey! Focus on building consistent habits.",
            keyInsights = new[]
            {
                "Starting with 3 workouts per week is ideal for beginners",
                "Focus on form over intensity initially",
            },
            recommendations = new[]
            {
                "Begin with 20-30 minute walks",
                "Add bodyweight exercises 2x per week",
            },
        };

        string jsonResponse = JsonSerializer.Serialize(expectedApiResponse);
        HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json"),
        };

        this.mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        AIWorkoutAnalysisResponseDto result = await this.service.GetWorkoutAnalysisAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Welcome to your fitness journey", result.Analysis);
        Assert.Equal(2, result.KeyInsights.Count);
        Assert.Equal(2, result.Recommendations.Count);

        // Verify logging
        this.VerifyLogCalled(LogLevel.Information, "Requesting workout analysis for 0 workouts, type: General");
    }

    [Fact]
    public async Task GetWorkoutAnalysisAsync_WithHttpError_ReturnsFallbackAnalysis()
    {
        // Arrange
        AIWorkoutAnalysisRequestDto request = new AIWorkoutAnalysisRequestDto
        {
            AthleteProfile = new AIAthleteProfileDto
            {
                Name = "Test User",
                FitnessLevel = "Intermediate",
                PrimaryGoal = "Endurance",
            },
            RecentWorkouts = new List<AIWorkoutDataDto>
            {
                new AIWorkoutDataDto
                {
                    Date = DateTime.Now.AddDays(-1),
                    ActivityType = "Run",
                    Distance = 5.0,
                    Duration = 1800,
                    Calories = 400,
                },
            },
            AnalysisType = "Performance",
        };

        HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Internal server error", Encoding.UTF8, "text/plain"),
        };

        this.mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        AIWorkoutAnalysisResponseDto result = await this.service.GetWorkoutAnalysisAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Based on your 1 recent workouts covering 5.0km", result.Analysis);
        Assert.Contains("your training shows good consistency", result.Analysis);
        Assert.Equal(4, result.KeyInsights.Count);
        Assert.Equal(4, result.Recommendations.Count);
        Assert.Equal("Fallback", result.Source);
        Assert.Contains("Completed 1 workouts with strong consistency", result.KeyInsights);
        Assert.Contains("Continue current training schedule", result.Recommendations);

        // Verify error logging
        this.VerifyLogCalled(LogLevel.Error, "AIAssistant workout analysis request failed");
    }

    [Fact]
    public async Task GetWorkoutAnalysisAsync_WithNullAthleteProfile_HandlesGracefully()
    {
        // Arrange
        AIWorkoutAnalysisRequestDto request = new AIWorkoutAnalysisRequestDto
        {
            AthleteProfile = null,
            RecentWorkouts = new List<AIWorkoutDataDto>
            {
                new AIWorkoutDataDto
                {
                    Date = DateTime.Now.AddDays(-1),
                    ActivityType = "Run",
                    Distance = 5.0,
                    Duration = 1800,
                    Calories = 400,
                },
            },
            AnalysisType = "General",
        };

        var expectedApiResponse = new
        {
            analysis = "Your workout data shows good progress.",
            keyInsights = new[] { "Consistent training patterns observed" },
            recommendations = new[] { "Keep up the good work" },
        };

        string jsonResponse = JsonSerializer.Serialize(expectedApiResponse);
        HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json"),
        };

        this.mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act & Assert - Should not throw exception
        AIWorkoutAnalysisResponseDto result = await this.service.GetWorkoutAnalysisAsync(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Your workout data shows good progress.", result.Analysis);
    }

    #endregion

    #region IsHealthyAsync Tests

    [Fact]
    public async Task IsHealthyAsync_WithSuccessfulResponse_ReturnsTrue()
    {
        // Arrange
        HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"status\":\"healthy\"}", Encoding.UTF8, "application/json"),
        };

        this.mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri.ToString().Contains("/api/Debug/health")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        bool result = await this.service.IsHealthyAsync(CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsHealthyAsync_WithErrorResponse_ReturnsFalse()
    {
        // Arrange
        HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
        {
            Content = new StringContent("Service unavailable", Encoding.UTF8, "text/plain"),
        };

        this.mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        bool result = await this.service.IsHealthyAsync(CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsHealthyAsync_WithException_ReturnsFalseAndLogsWarning()
    {
        // Arrange
        this.mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act
        bool result = await this.service.IsHealthyAsync(CancellationToken.None);

        // Assert
        Assert.False(result);
        this.VerifyLogCalled(LogLevel.Warning, "AIAssistant health check failed");
    }

    #endregion

    #region NotImplemented Methods Tests

    [Fact]
    public Task GetPerformanceTrendsAsync_ThrowsNotImplementedException()
    {
        // Act & Assert
        return Assert.ThrowsAsync<NotImplementedException>(
            () => this.service.GetPerformanceTrendsAsync(1, CancellationToken.None, "month"));
    }

    [Fact]
    public Task GetTrainingRecommendationsAsync_ThrowsNotImplementedException()
    {
        // Act & Assert
        return Assert.ThrowsAsync<NotImplementedException>(
            () => this.service.GetTrainingRecommendationsAsync(1, CancellationToken.None));
    }

    [Fact]
    public Task AnalyzeHealthMetricsAsync_ThrowsNotImplementedException()
    {
        // Arrange
        List<AIWorkoutDataDto> workouts = new List<AIWorkoutDataDto>();

        // Act & Assert
        return Assert.ThrowsAsync<NotImplementedException>(
            () => this.service.AnalyzeHealthMetricsAsync(1, workouts, CancellationToken.None));
    }

    [Fact]
    public Task GetGoogleGeminiWorkoutAnalysisAsync_ThrowsNotImplementedException()
    {
        // Arrange
        AIWorkoutAnalysisRequestDto request = new AIWorkoutAnalysisRequestDto();

        // Act & Assert
        return Assert.ThrowsAsync<NotImplementedException>(
            () => this.service.GetGoogleGeminiWorkoutAnalysisAsync(request, CancellationToken.None));
    }

    #endregion

    #region Configuration Tests

    [Fact]
    public void Constructor_WithCustomBaseUrl_SetsCorrectBaseAddress()
    {
        // Arrange
        Mock<IConfiguration> customConfig = new Mock<IConfiguration>();
        customConfig.Setup(x => x["AIAssistant:BaseUrl"])
            .Returns("https://custom-ai-service.com");

        HttpClient httpClient = new HttpClient(this.mockHttpMessageHandler.Object);

        // Act
        AIAssistantClientService service = new AIAssistantClientService(httpClient, this.mockLogger.Object, customConfig.Object);

        // Assert
        Assert.Equal("https://custom-ai-service.com/", httpClient.BaseAddress?.ToString());
        this.VerifyLogCalled(LogLevel.Information, "AIAssistantClientService initialized with base URL: https://custom-ai-service.com");
    }

    [Fact]
    public void Constructor_WithNullBaseUrl_UsesDefaultUrl()
    {
        // Arrange
        Mock<IConfiguration> nullConfig = new Mock<IConfiguration>();
        nullConfig.Setup(x => x["AIAssistant:BaseUrl"])
            .Returns((string)null);

        HttpClient httpClient = new HttpClient(this.mockHttpMessageHandler.Object);

        // Act
        AIAssistantClientService service = new AIAssistantClientService(httpClient, this.mockLogger.Object, nullConfig.Object);

        // Assert
        Assert.Equal("http://localhost:5169/", httpClient.BaseAddress?.ToString());
        this.VerifyLogCalled(LogLevel.Information, "AIAssistantClientService initialized with base URL: http://localhost:5169");
    }

    #endregion

    #region Helper Methods

    private void VerifyLogCalled(LogLevel logLevel, string message)
    {
        this.mockLogger.Verify(
            x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(message)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    #endregion

    #region Dispose

    public void Dispose()
    {
        this.httpClient?.Dispose();
        GC.SuppressFinalize(this);
    }

    #endregion
}
