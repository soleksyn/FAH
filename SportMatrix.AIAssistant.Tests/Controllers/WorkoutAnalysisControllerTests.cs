using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SportMatrix.AIAssistant.Application.DTOs;
using SportMatrix.AIAssistant.Application.Interfaces;
using SportMatrix.AIAssistant.UI.API.Controllers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SportMatrix.AIAssistant.Tests.Controllers;

public class WorkoutAnalysisControllerTests
{
    private readonly Mock<IWorkoutAnalysisService> mockWorkoutAnalysisService;
    private readonly Mock<ILogger<WorkoutAnalysisController>> mockLogger;
    private readonly WorkoutAnalysisController controller;

    public WorkoutAnalysisControllerTests()
    {
        this.mockWorkoutAnalysisService = new Mock<IWorkoutAnalysisService>();
        this.mockLogger = new Mock<ILogger<WorkoutAnalysisController>>();
        this.controller = new WorkoutAnalysisController(
            this.mockWorkoutAnalysisService.Object,
            this.mockLogger.Object);
    }

    [Fact]
    public async Task AnalyzeWorkouts_WithValidRequest_ReturnsAnalysisResult()
    {
        var request = new WorkoutAnalysisRequestDto { AnalysisType = "Performance" };
        var expectedResponse = new WorkoutAnalysisResponseDto { Analysis = "Test", Provider = "Gemini-AI" };

        this.mockWorkoutAnalysisService
            .Setup(s => s.AnalyzeWorkoutsAsync(It.IsAny<WorkoutAnalysisRequestDto>(), CancellationToken.None))
            .ReturnsAsync(expectedResponse);

        var result = await this.controller.AnalyzeWorkouts(request, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<WorkoutAnalysisResponseDto>(okResult.Value);
        Assert.Equal("Test", response.Analysis);
    }

    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        var expectedResponse = new WorkoutAnalysisResponseDto { Analysis = "Healthy" };
        this.mockWorkoutAnalysisService
            .Setup(s => s.AnalyzeWorkoutsAsync(It.IsAny<WorkoutAnalysisRequestDto>(), CancellationToken.None))
            .ReturnsAsync(expectedResponse);

        var result = await this.controller.HealthCheck(CancellationToken.None);

        Assert.IsType<OkObjectResult>(result);
    }
}