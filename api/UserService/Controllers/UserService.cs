using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserService.Enum;
using UserService.Models;
using UserService.Utils;

namespace UserService.Controllers;

public class UserService
{
    private readonly ApplicationDbContext dbContext;

    public UserService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public bool UserExists(string email)
    {
        return dbContext.UserAccount.Any(u => u.Email == email);
    }
    
    public async Task<int?> RegisterUserInternal(UserModel userModel, bool isDoctor = false)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userModel.Password);
        userModel.Password = hashedPassword;
        userModel.Status = (byte)UserStatusEnum.ACTIVE;
        userModel.IsDoctor = isDoctor ? (byte?)1 : (byte?)0;
        try
        {
            dbContext.UserAccount.Add(userModel);

            await dbContext.SaveChangesAsync();

            return userModel.UserId; // Return the generated user_id
        }
        catch (Exception e)
        {
            return -1;
        }
    }
    
    public async Task<(string userId, string password, bool isDoctor)?> GetUserIdAndPasswordByEmailAsync(string email)
    {
        

        var result = await dbContext.UserAccount
            .Where(u => u.Email == email) // Filter by email
            .Select(u => new { u.UserId, u.Password, u.IsDoctor }) // Select only userId and password
            .FirstOrDefaultAsync(); // Fetch first matching record
            
        return result == null ? null : (result.UserId.ToString(), result.Password, result.IsDoctor == 1);
        
    }

    public AuthResponse GenerateAuthResponse(string email, string userId, bool isDoctor)
    {
        var expiration = DateTime.UtcNow.AddHours(2);
        var token = GenerateJwtToken(email, userId, expiration);
        var authResponse = new AuthResponse
        {
            Token = token,
            UserId = userId,
            Expiration = expiration,
            IsDoctor = isDoctor
        };
        
        return authResponse;
    }

    public bool VerifyPassword(string inputPassword, string storedHash)
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