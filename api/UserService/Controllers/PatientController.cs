using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PatientController : ControllerBase
{
    private readonly ApplicationDbContext dbContext;
    private readonly UserService userService;

    public PatientController(ApplicationDbContext dbContext, UserService userService)
    {
        this.dbContext = dbContext;
        this.userService = userService;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] PatientModel patientModel)
    {
        if (userService.UserExists(patientModel.Email))
        {
            return BadRequest("User already exists.");
        }

        if (patientModel.BirthDate == null)
        {
            return BadRequest("Birthdate is required.");
        }

        patientModel.UserId = await userService.RegisterUserInternal(patientModel);

        if (patientModel.UserId is -1 or null)
        {
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }

        return await RegisterInternal(patientModel);
    }
    
    private async Task<IActionResult> RegisterInternal(PatientModel patientModel)
    {
        
        const string queryInsert = @"
        INSERT INTO dbo.Patient (user_id, name, surname, birth_date)
        VALUES (@user_id, @name, @surname, @birth_date);";
        
        try
        {
            await dbContext.Database.ExecuteSqlRawAsync(queryInsert, 
                new SqlParameter("@user_id", patientModel.UserId),
                new SqlParameter("@name", patientModel.Name),
                new SqlParameter("@surname", patientModel.Surname),
                new SqlParameter("@birth_date", patientModel.BirthDate)
            );

            return StatusCode(200);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("getPatient/{id}")]
    public async Task<PatientModel?> GetPatient(int id)
    {
        var query = @"SELECT user_id AS UserId, 
                             name AS Name, 
                             surname AS Surname, 
                             birth_date AS BirthDate,
                             null as Email,
                             null as Password,
                             null as Status
                      FROM dbo.Patient 
                      WHERE user_id = @UserId";

        var patient = await dbContext.Database
            .SqlQueryRaw<PatientModel>(query,  new SqlParameter("@UserId", id))
            .AsNoTracking()
            .FirstOrDefaultAsync();
            
        return patient;
    }
}