using DoctorService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DoctorService.Controllers;


[ApiController]
[Route("/api/v1/[controller]")]
public class DoctorController: ControllerBase
{
    
    private readonly string _connectionString;
    private readonly SqlConnection connection; 
    public DoctorController(IConfiguration configuration)
    {
        _connectionString = configuration["DB_CONNECTION_STRING"] ?? throw new Exception("Connection string not found");
		connection = new SqlConnection(_connectionString);
        connection.Open();
    }
    
    [HttpGet("getDoctor")]
    public DoctorModel GetDoctor()
    {
        return new DoctorModel
        {
            FirstName = "Doctor 1",
            LastName = "Doctor 2",
            Speciality = "Hospital1"
        };
    }
    
    [HttpGet("getDoctor/{id}")]
    public List<DoctorModel> GetDoctor([FromRoute] string id)
    {
        var rows = new List<DoctorModel>();
        var command = new SqlCommand("SELECT * FROM Doctor WHERE id = @id", connection);
        command.Parameters.AddWithValue("@id", id);
        using SqlDataReader reader = command.ExecuteReader();

        if (reader.HasRows)
        {
            while (reader.Read())
            {
                rows.Add(new DoctorModel
                {
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    Email = reader.GetString(3),
                    Speciality = reader.GetString(4),
                    BirthDate = reader.GetDateTime(5),
                }); 
            }
        }

        return rows;
    }

    [HttpPost("registerDoctor")]
    public async Task<ActionResult> AddDoctor([FromBody] DoctorModel? doctor)
    {
        try
        {
            if (doctor == null)
            {
                return BadRequest("Doctor information is incomplete.");
            }
            
			var command = new SqlCommand(
            	"INSERT INTO Doctor (first_name, surname, email, speciality, sys_timestamp, sys_created) VALUES (@firstName, @lastName, @email, @speciality, @birthDate, @sysCreated); SELECT SCOPE_IDENTITY();", connection);
            command.Parameters.AddWithValue("@firstName", doctor.FirstName);
            command.Parameters.AddWithValue("@lastName", doctor.LastName);
            command.Parameters.AddWithValue("@email", doctor.Email);
            command.Parameters.AddWithValue("@speciality", doctor.Speciality);
            command.Parameters.AddWithValue("@birthDate", doctor.BirthDate);
			command.Parameters.AddWithValue("@sysCreated", DateTime.UtcNow);
                
            var newId = (int)(await command.ExecuteScalarAsync() ?? 0);
            return newId > 0 ? CreatedAtAction(nameof(GetDoctor), new { id = newId }, doctor) : StatusCode(500, "Error while inserting.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        } 
    }
}