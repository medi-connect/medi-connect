using AppointmentService.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentService.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class MainContoller: ControllerBase
{
    public AppointmentModel GetAppointment()
    {
        return new AppointmentModel
        {
            Id = Guid.NewGuid(),
            Date = DateTime.Now,
            Location = "New York",
        };
    }
}