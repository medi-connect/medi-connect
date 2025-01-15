using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using UserService.Models;
using UserService.Utils;

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
    
    /* =============================
    * GET METHODS
    =============================*/ 


    /// <summary>
    /// Retrieves a list of all patients.
    /// </summary>
    /// <returns>
    /// A list of patients.
    /// </returns>
    /// <response code="200">Returns the list of patients</response>
    /// <response code="500">If error occurs</response>
    [HttpGet("getAllPatients")]
    public async Task<ActionResult<List<PatientModel>>> GetAllPatients()
    {
        var query = @"SELECT user_id AS UserId, 
                             name AS Name, 
                             surname AS Surname, 
                             birth_date AS BirthDate,
                             null as Email,
                             null as Password,
                             null as Status,
                             null as IsDoctor
                      FROM dbo.Patient;";

        try
        {
            var patients = await dbContext.Database
                .SqlQueryRaw<PatientModel>(query)
                .ToListAsync();

            return Ok(patients);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                error = "An unexpected error occurred while processing request.",
            });
        }
    }
    
    /// <summary>
    /// Retrieves a specific patient by their ID.
    /// </summary>
    /// <param name="id">The ID of the patient to retrieve.</param>
    /// <returns>The patient with the specified ID.</returns>
    /// <response code="200">Returns the patient with the specified ID</response>
    /// <response code="500">If error occured.</response>
    [HttpGet("getPatient/{id}")]
    public async Task<ActionResult<PatientModel?>> GetPatient(int id)
    {
        try
        {
            var query = @"SELECT user_id AS UserId, 
                             name AS Name, 
                             surname AS Surname, 
                             birth_date AS BirthDate,
                             null as Email,
                             null as Password,
                             null as Status,
                             null as IsDoctor
                      FROM dbo.Patient 
                      WHERE user_id = @UserId";

            var patient = await dbContext.Database
                .SqlQueryRaw<PatientModel>(query, new SqlParameter("@UserId", id))
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return Ok(patient);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                error = "An unexpected error occurred while processing request.",
            });
        }
    }
    
    /* =============================
    * POST METHODS
    =============================*/ 

    
    /// <summary>
    /// Registers new patient.
    /// </summary>
    /// <param name="patient">The patient object to be created.</param>
    /// <returns>Confirmation.</returns>
    /// <response code="200">Returns code 200</response>
    /// <response code="400">If the patient exists or required fields are empty</response>
    /// <response code="500">If unexpected error occured</response>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] PatientModel patient)
    {
        if (userService.UserExists(patient.Email))
        {
            return BadRequest("User already exists.");
        }

        if (patient.BirthDate == null)
        {
            return BadRequest("Birthdate is required.");
        }

        patient.UserId = await userService.RegisterUserInternal(patient);

        if (patient.UserId is -1 or null)
        {
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }

        return await RegisterInternal(patient);
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


}