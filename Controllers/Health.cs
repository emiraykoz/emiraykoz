using Microsoft.AspNetCore.Mvc;
using node_daemon.Infrastructure;

namespace node_daemon.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    private readonly PodmanService podman;

    public HealthController(PodmanService podmanService)
    {
        podman = podmanService;
    }

    [HttpGet("status")]
    public async Task<IActionResult> Status()
    {
        var status = await podman.GetStatusAsync();

        if (!((dynamic)status).Online)
            return StatusCode(503, status);

        return Ok(status);
    }
}