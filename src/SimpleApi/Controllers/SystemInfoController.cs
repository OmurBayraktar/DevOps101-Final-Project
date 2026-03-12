using Microsoft.AspNetCore.Mvc;

namespace SimpleApi.Controllers;

[ApiController]
[Route("")]
public class SystemInfoController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    public SystemInfoController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok("OK");
    }

    [HttpGet("api/info")]
    public IActionResult Info()
    {
        var environmentName = _environment.EnvironmentName;
        var studentName = Environment.GetEnvironmentVariable("STUDENT_NAME") ?? "Unknown Student";
        
        var response = new
        {
            student = studentName,
            environment = environmentName,
            serverTimeUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
        };
        
        return Ok(response);
    }
}
