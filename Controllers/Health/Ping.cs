using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace node_daemon.Controllers.Health
{
    [Route("health/ping")]
    [ApiController]
    public class Ping : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Content("Pong!", "text/plain");
    }
}
