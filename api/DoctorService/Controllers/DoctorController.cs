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
    public DoctorController(ApplicationDbContext dbContext, HttpClient httpClient)
    {
        this.dbContext = dbContext;
        this.httpClient = httpClient;
    }
    
    [HttpGet("getDoctor/{id}")]
    public async Task<DoctorModel?> GetDoctor([FromRoute] string id)
    {
        var record = await dbContext.Doctor.FindAsync(id);
        return record;
    }

    [HttpPost("register")]
    public async Task<ActionResult> RegisterDoctor([FromBody] DoctorModel? doctor)
    {
        const string url = "http://localhost:8001/api/v1/user/registerDoctor";
        var content = new StringContent(JsonSerializer.Serialize(doctor), Encoding.UTF8, "application/json");
        try
        {
            if (doctor == null)
            {
                return BadRequest("Insufficient information.");
            }
            var response = await httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            
            var responseString = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Allows JSON property names to be case-insensitive
            };
            var doctorModel = JsonSerializer.Deserialize<DoctorModel>(responseString, options);
            if (doctorModel?.UserId == null)
            {
                return BadRequest("Failed to obtain userId.");
            }

            return await RegisterDoctorInternal(doctorModel);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
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

            Console.WriteLine("Doctor successfully added!");
        }
        catch (Exception ex)
        {
             StatusCode(500, ex.Message);
        }
        return StatusCode(200);
    }
}