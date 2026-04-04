using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;
using node_daemon.Infrastructure;

namespace node_daemon.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    private readonly ContainerEngineService containerEngine;

    public HealthController(ContainerEngineService containerEngineService)
    {
        containerEngine = containerEngineService;
    }

    [HttpGet("live")]
    public IActionResult Live() => Ok("OK");

    [HttpGet("ready")]
    public async Task<IActionResult> Ready()
    {
        var status = await containerEngine.StatusAsync();

        if (!((dynamic)status).Online)
            return StatusCode(503, status);

        return Ok(status);
    }
}