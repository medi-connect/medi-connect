using Microsoft.AspNetCore.Mvc;
using UserService.Models;

namespace UserService.Controllers;


[ApiController]
[Route("api/v1/[controller]")]
public class PatientController : ControllerBase
{
    private UserController userController;

    public PatientController(ApplicationDbContext dbContext)
    {
        userController = new UserController(dbContext);
    }
    [HttpPost("registerPatient")]
    public async Task<IActionResult> Register([FromBody] PatientModel patientModel)
    {

        return await userController.RegisterPatient(patientModel);
    }

}