using Microsoft.AspNetCore.Mvc;

namespace MediConnect.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class MainController : ControllerBase
{

    [HttpGet("getPersonData")]
    public PersonDTO GetHealthData()
    {
        return new PersonDTO
        {
            Name = "Jamess",
            Surname = "Bond",
            Health = new List<HealthDTO>
            {
                new()
                {
                    Health = 100,
                    Status = "ok",
                    Unhealthy = false
                },
                new()
                {
                    Health = 60,
                    Status = "Kinda ok",
                    Unhealthy = false
                },
                new()
                {
                    Health = 12,
                    Status = "Nearly dead",
                    Unhealthy = true
                }
            }
        };
    }
}