using Microsoft.AspNetCore.Mvc;

namespace PlanejadorCompras.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            message = "API funcionando",
            timestamp = DateTime.UtcNow
        });
    }
}
