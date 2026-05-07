namespace SportMatrix.AIAssistant.UI.API.Controllers;

using SportMatrix.AIAssistant.Application.Interfaces;
using SportMatrix.AIAssistant.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DebugController : ControllerBase
{
    private readonly ILogger<DebugController> logger;

    public DebugController(ILogger<DebugController> logger)
    {
        this.logger = logger;
    }


    [HttpGet("health")]
    public ActionResult HealthCheck()
    {
        return this.Ok(new
        {
            status = "healthy",
            message = "Debug controller is responding",
            timestamp = DateTime.UtcNow,
        });
    }
}
