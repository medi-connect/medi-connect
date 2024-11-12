using DoctorService.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoctorService.Controllers;


[ApiController]
[Route("/api/v1/[controller]")]
public class MainController
{
    [HttpGet("getDoctor")]
    public DoctorModel GetDoctor()
    {
        return new DoctorModel
        {
            Name = "Doctor 1",
            Surname = "Doctor 2",
            HospitalName = "Hospital1"
        };
    }
}