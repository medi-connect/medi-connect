using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UserController : ControllerBase
{
    private readonly string _connectionString;
    private readonly ApplicationDbContext dbContext;

    public UserController(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
    {
        
        if (await dbContext.UserAccount.AnyAsync(u => u.Email == registerDTO.Email))
        {
            return BadRequest("Username already exists");
        }
        
        var patient = new PatientDTO()
        {
            Name = registerDTO.Name,
            Surname = registerDTO.Surname,
            Email = registerDTO.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password),
            BirthDate = registerDTO.BirthDate
        };

        var emailParam = new SqlParameter("@Email", patient.Email);
        var passwordParam = new SqlParameter("@Password", patient.Password);
        var statusParam = new SqlParameter("@Status", patient.Status);

        try
        {
            const string command = "EXEC InsertUserAccount @Email, @Password, @Status";
            var rowsAffected = await dbContext.Database.ExecuteSqlRawAsync(
                command,
                emailParam, passwordParam, statusParam
            );
            
            if (rowsAffected > 0)
            {
                return Ok($"Registration successful, {patient.Email}");
            }

            return BadRequest($"Registration failed");
        }
        catch (Exception e)
        {
            return BadRequest("Error occured: " + e.Message);
        }
        
    }
}