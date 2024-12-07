using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserService.Enum;
using UserService.Models;

namespace UserService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UserController : ControllerBase
{
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
    
    public async Task<IActionResult> RegisterPatient(RegisterModel registerModel)
    {
        
        if (UserExists(registerModel.Email))
        {
            return BadRequest("User already exists.");
        }

        registerModel.Id = await RegisterUserInternal(registerModel);

        if (registerModel.Id is -1 or null)
        {
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }

        return await RegisterPatientInternal(registerModel);
    }
    
    [HttpPost("registerDoctor")]
    public async Task<IActionResult> RegisterDoctor([FromBody] RegisterModel registerModel)
    {
        
        if (UserExists(registerModel.Email))
        {
            return BadRequest("User already exists.");
        }

        registerModel.Id = await RegisterUserInternal(registerModel);

        if (registerModel.Id is -1 or null)
        {
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }

        return await RegisterDoctorInternal(registerModel);
    }
    public bool UserExists(string email)
    {
        return dbContext.UserAccount.Any(u => u.Email == email);
    }

    private async Task<int> RegisterUserInternal(UserModel userModel)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userModel.Password);
        userModel.Status = (short)UserStatusEnum.ACTIVE;
        var emailParam = new SqlParameter("@Email", userModel.Email);
        var passwordParam = new SqlParameter("@Password", hashedPassword);
        var statusParam = new SqlParameter("@Status", userModel.Status);
        var idParam = new SqlParameter
        {
            ParameterName = "@Id",
            SqlDbType = SqlDbType.Int,
            Direction = ParameterDirection.Output
        };

        try
        {
            const string command = "EXEC dbo.InsertUserAccount @Email, @Password, @Status, @Id OUTPUT;";
            var rowsAffected = await dbContext.Database.ExecuteSqlRawAsync(
                command,
                emailParam, passwordParam, statusParam, idParam
            );

            var userId = idParam.Value == DBNull.Value ? null : (int?)idParam.Value;
            if (rowsAffected > 0 && userId.HasValue)
            {
                return userId.Value;
            }

            return -1;
        }
        catch (Exception e)
        {
            return -1;
        }
    }
    
    private async Task<IActionResult> RegisterPatientInternal(RegisterModel registerModel)
    {
        var userIdParam = new SqlParameter("@UserId", registerModel.Id);
        var nameParam = new SqlParameter("@Name", registerModel.Name);
        var surnameParam = new SqlParameter("@Surname", registerModel.Surname);
        var birthDateParam = new SqlParameter("@BirthDate", registerModel.BirthDate);
        var idParam = new SqlParameter
        {
            ParameterName = "@Id",
            SqlDbType = SqlDbType.Int,
            Direction = ParameterDirection.Output
        };

        try
        {
            const string command = "EXEC dbo.InsertPatientAccount @UserId, @Name, @Surname, @BirthDate, @Id OUTPUT;";
            var rowsAffected = await dbContext.Database.ExecuteSqlRawAsync(
                command,
                userIdParam, nameParam, surnameParam, birthDateParam, idParam
            );
            var patientId = idParam.Value as int?;
            if (rowsAffected > 0 && patientId != null)
            {
                return Ok("Registration successful. " +  new
                {
                    Id = patientId,
                    Email = registerModel.Email,
                    Type = UserTypeEnum.PATIENT
                });
            }

            return StatusCode(500, "An unexpected error occurred.");
        }
        catch (Exception e)
        {
            return StatusCode(500, "An unexpected error occurred. Error: " + e.Message);
        }
    }
    
    private async Task<IActionResult> RegisterDoctorInternal(RegisterModel registerModel)
    {
        var userIdParam = new SqlParameter("@UserId", registerModel.Id);
        var nameParam = new SqlParameter("@Name", registerModel.Name);
        var surnameParam = new SqlParameter("@Surname", registerModel.Surname);
        var specialityParam = new SqlParameter("@Speciality", registerModel.Speciality);
        var idParam = new SqlParameter
        {
            ParameterName = "@Id",
            SqlDbType = SqlDbType.Int,
            Direction = ParameterDirection.Output
        };

        try
        {
            const string command = "EXEC dbo.InsertDoctorAccount @UserId, @Name, @Surname, @Speciality, @Id OUTPUT;";
            var rowsAffected = await dbContext.Database.ExecuteSqlRawAsync(
                command,
                userIdParam, nameParam, surnameParam, specialityParam, idParam
            );
            var doctorId = idParam.Value as int?;
            if (rowsAffected > 0 && doctorId != null)
            {
                return Ok("Registration successful. " +  new
                {
                    Id = doctorId,
                    Email = registerModel.Email,
                    Type = UserTypeEnum.DOCTOR
                });            }

            return StatusCode(500, "An unexpected error occurred.");
        }
        catch (Exception e)
        {
            return StatusCode(500, "An unexpected error occurred. Error: " + e.Message);
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
            
            if (VerifyPassword(request.Password, passwordHash))
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
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, email)
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