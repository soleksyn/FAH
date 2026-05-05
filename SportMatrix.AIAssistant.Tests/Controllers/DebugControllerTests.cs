using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SportMatrix.AIAssistant.UI.API.Controllers;
using Xunit;

namespace SportMatrix.AIAssistant.Tests.Controllers;

public class DebugControllerTests
{
    private readonly Mock<ILogger<DebugController>> mockLogger;
    private readonly DebugController controller;

    public DebugControllerTests()
    {
        this.mockLogger = new Mock<ILogger<DebugController>>();
        this.controller = new DebugController(this.mockLogger.Object);
    }

    [Fact]
    public void HealthCheck_ReturnsOk()
    {
        var result = this.controller.HealthCheck();
        Assert.IsType<OkObjectResult>(result);
    }
}
