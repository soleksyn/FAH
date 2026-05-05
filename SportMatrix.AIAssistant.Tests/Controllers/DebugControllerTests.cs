using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SportMatrix.AIAssistant.UI.API.Controllers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SportMatrix.AIAssistant.Tests.Controllers;

public class DebugControllerTests
{
    private readonly Mock<IConfiguration> mockConfiguration;
    private readonly Mock<ILogger<DebugController>> mockLogger;
    private readonly DebugController controller;

    public DebugControllerTests()
    {
        this.mockConfiguration = new Mock<IConfiguration>();
        this.mockLogger = new Mock<ILogger<DebugController>>();
        this.controller = new DebugController(this.mockConfiguration.Object, this.mockLogger.Object);
    }

    [Fact]
    public void ConfigCheck_ReturnsOk()
    {
        this.mockConfiguration.Setup(c => c["GoogleAI:ApiKey"]).Returns("test-key");
        var result = this.controller.ConfigCheck();
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void HealthCheck_ReturnsOk()
    {
        var result = this.controller.HealthCheck();
        Assert.IsType<OkObjectResult>(result);
    }
}
