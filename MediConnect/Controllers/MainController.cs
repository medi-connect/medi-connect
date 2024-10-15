using Microsoft.AspNetCore.Mvc;

namespace MediConnect.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class MainController : ControllerBase
{

    [HttpGet("getHealthData")]
    public HealthDTO GetHealthData()
    {
        return new HealthDTO
        {
            Status = "OK",
            Health = 100,
            Unhealthy = false
        };
    }
}