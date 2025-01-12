using System.Text;
using System.Text.Json;
using DoctorService.Models;
using DoctorService.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DoctorService.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class DoctorController: ControllerBase
{
    
    private readonly ApplicationDbContext dbContext;
    private readonly HttpClient httpClient;
    private readonly ILogger<DoctorController> _logger;
    public DoctorController(ApplicationDbContext dbContext, HttpClient httpClient, ILogger<DoctorController> logger)
    {
        this.dbContext = dbContext;
        this.httpClient = httpClient;
        _logger = logger;
    }
    
    /* =============================
     * GET METHODS
     =============================*/ 
    [HttpGet("getAllDoctors")]
    public async Task<List<DoctorModel>> GetAllDoctors()
    {
        var query = @"SELECT user_id AS UserId, 
                             name AS Name, 
                             surname AS Surname, 
                             speciality AS Speciality,
                             null as Email,
                             null as Password,
                             null as Status
                      FROM dbo.Doctor;";

        try
        {
            var doctors = await dbContext.Database
                .SqlQueryRaw<DoctorModel>(query)
                .ToListAsync();

            return doctors;
        }
        catch
        {
            return new List<DoctorModel>();
        }
    }
    
    [HttpGet("getDoctor/{id}")]
    public async Task<DoctorModel?> GetDoctor([FromRoute] int id)
    {
        var query = @"SELECT user_id AS UserId, 
                             name AS Name, 
                             surname AS Surname, 
                             speciality AS Speciality,
                             null as Email,
                             null as Password,
                             null as Status,
                             null as IsDoctor
                      FROM dbo.Doctor 
                      WHERE user_id = @UserId";

        var doctor = await dbContext.Database
            .SqlQueryRaw<DoctorModel>(query,  new SqlParameter("@UserId", id))
            .AsNoTracking()
            .FirstOrDefaultAsync();
            
        return doctor;
    }

    [HttpGet("getDoctorsBySpeciality/{speciality}")]
    public async Task<List<DoctorModel>> GetDoctorsBySpeciality([FromRoute] string speciality)
    {
        var query = @"SELECT user_id AS UserId, 
                             name AS Name, 
                             surname AS Surname, 
                             speciality AS Speciality,
                             null as Email,
                             null as Password,
                             null as Status
                      FROM dbo.Doctor 
                      WHERE speciality = @Speciality";

        var doctors = await dbContext.Database
            .SqlQueryRaw<DoctorModel>(query, new SqlParameter("@Speciality", speciality))
            .ToListAsync();
        
        return doctors;
    }
    /* =============================
     * POST METHODS
     =============================*/ 
    [HttpPost("register")]
    public async Task<ActionResult> RegisterDoctor([FromBody] DoctorModel? doctor)
    {
        if (doctor == null)
        {
            _logger.LogWarning("Doctor information is missing or invalid.");
            return BadRequest("Doctor information is missing or invalid.");
        }

        const string url = "http://user-service:8001/api/v1/user/registerDoctor";
        var content = new StringContent(JsonSerializer.Serialize(doctor), Encoding.UTF8, "application/json");

        try
        {
            _logger.LogInformation("Calling user-service to register doctor at URL: {Url}", url);

            var response = await httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to call user-service. StatusCode: {StatusCode}, Response: {Response}",
                    response.StatusCode, errorDetails);
                return StatusCode((int)response.StatusCode, errorDetails);
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Allows JSON property names to be case-insensitive
            };
            _logger.LogInformation("Received response from user-service: {Response}", responseString);
            var doctorModel = JsonSerializer.Deserialize<DoctorModel>(responseString, options);

            if (doctorModel?.UserId == null)
            {
                _logger.LogWarning("UserId is null in the response from user-service.");
                return BadRequest("Failed to obtain userId from user-service.");
            }

            return await RegisterDoctorInternal(doctorModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during doctor registration.");
            return StatusCode(500, ex.Message);
        }
    }

    /* =============================
     * PUT METHODS
     =============================*/ 
    [HttpPut("updateDoctor/{id}")]
    public async Task<ActionResult> UpdateDoctor([FromRoute] int id, [FromBody] DoctorModel doctor)
    {
        var query = @"UPDATE dbo.Doctor
                              SET name = @Name,
                                  surname = @Surname,
                                  speciality = @Speciality
                      WHERE user_id = @UserId";

        try
        {
            var rowsAffected = await dbContext.Database
                .ExecuteSqlRawAsync(query, new SqlParameter("@Name", doctor.Name),
                    new SqlParameter("@Surname", doctor.Surname),
                    new SqlParameter("@Speciality", doctor.Speciality),
                    new SqlParameter("@UserId", id));
            if (rowsAffected == 0)
            {
                return BadRequest("Doctor with ID; " + id + " does not exist");
            }

            return Ok(doctor);
        }
        catch (Exception ex)
        {
            return BadRequest("Error: " + ex.Message);
        }
    }
   
    private async Task<ActionResult> RegisterDoctorInternal(DoctorModel doctorModel)
    {
        const string queryInsert = @"
        INSERT INTO dbo.Doctor (user_id, name, surname, speciality)
        VALUES (@user_id, @name, @surname, @speciality);";
        
        try
        {
            await dbContext.Database.ExecuteSqlRawAsync(queryInsert, 
                    new SqlParameter("@user_id", doctorModel.UserId),
                    new SqlParameter("@name", doctorModel.Name),
                    new SqlParameter("@surname", doctorModel.Surname),
                    new SqlParameter("@speciality", doctorModel.Speciality ?? (object)DBNull.Value)
                );

        }
        catch (Exception ex)
        {
             StatusCode(500, ex.Message);
        }
        return StatusCode(200, "Registration successful");
    }
}