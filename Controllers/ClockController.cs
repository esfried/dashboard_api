using Microsoft.AspNetCore.Mvc;

namespace SimpleDashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClockController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
             return Ok(DateTime.Now.ToString());
        }
    }
}
