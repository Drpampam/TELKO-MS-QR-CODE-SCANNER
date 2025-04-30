using Application.Features.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller
{
    [Route("/")]
    public class HomeController : ControllerBase
    {
        [HttpGet("/")]
        public IActionResult HealthCheck()
        {
            return Ok($"{ResponseCodes.SUCCESS}  You have reached Test microservice");
        }
    }
}
