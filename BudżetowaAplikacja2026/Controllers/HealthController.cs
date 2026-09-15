using Microsoft.AspNetCore.Mvc;

namespace BudżetowaAplikacja2026.Controllers
{
    [ApiController]
    [Route("api")]
    public class HealthController : ControllerBase
    {
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "Backend is running", timestamp = DateTime.UtcNow });
        }
    }
}
