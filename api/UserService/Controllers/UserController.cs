using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!UserExists(request.Email))
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }
        
        return await LoginUser(request);
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
    {
        
        if (UserExists(registerDTO.Email))
        {
            return BadRequest("Email already exists");
        }

        return await RegisterUser(registerDTO);
    }

    private bool UserExists(string email)
    {
        return dbContext.UserAccount.Any(u => u.Email == email);
    }

    private async Task<IActionResult> RegisterUser(RegisterDTO registerDTO)
    {
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

    private async Task<IActionResult> LoginUser(LoginRequest request)
    {
        var emailParam = new SqlParameter("@Email", request.Email);
        var userIdParam = new SqlParameter
        {
            ParameterName = "@UserId",
            SqlDbType = SqlDbType.NVarChar,
            Size = 255,
            Direction = ParameterDirection.Output
        };
        var passwordParam = new SqlParameter
        {
            ParameterName = "@Password",
            SqlDbType = SqlDbType.NVarChar,
            Size = 255,
            Direction = ParameterDirection.Output
        };
        try
        {
            const string command = "EXEC dbo.GetUserCredentials @Email, @UserId OUTPUT, @Password OUTPUT";
            await dbContext.Database.ExecuteSqlRawAsync(
                command,
                emailParam,
                userIdParam,
                passwordParam
            );
            
            var userId = userIdParam.Value as string;
            var passwordHash = passwordParam.Value as string;
            if (string.IsNullOrEmpty(passwordHash)|| string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }
            
            if (BCrypt.Net.BCrypt.Verify(request.Password, passwordHash))
            {
                return Ok(GenerateAuthResponse(request.Email, userId));
            }
            return Unauthorized(new { message = "Invalid username or password." });
        }
        catch (Exception e)
        {
            return BadRequest("Error occured: " + e.Message);
        }
    }

    private AuthResponse GenerateAuthResponse(string email, string userId)
    {
        var expiration = DateTime.UtcNow.AddHours(1);
        var token = GenerateJwtToken(email, userId, expiration);

        var authResponse = new AuthResponse
        {
            Token = token,
            UserId = userId,
            Expiration = expiration
        };
        
        return authResponse;
    }

    private bool VerifyPassword(string inputPassword, string storedHash)
    {
        return BCrypt.Net.BCrypt.Verify(inputPassword, storedHash);
    }

    private string GenerateJwtToken(string email, string userId, DateTime expiration)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email)
        };
        
        var jwToken = new JwtSecurityToken(
        claims: claims,
        notBefore: DateTime.UtcNow,
        expires: expiration,
        signingCredentials: new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SIGNING_KEY")!)),
                SecurityAlgorithms.HmacSha256Signature)
            );
        return new JwtSecurityTokenHandler().WriteToken(jwToken);
    }
}