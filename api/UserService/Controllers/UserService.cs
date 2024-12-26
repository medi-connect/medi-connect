using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserService.Enum;
using UserService.Models;

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
    
    public async Task<int?> RegisterUserInternal(UserModel userModel)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userModel.Password);
        userModel.Password = hashedPassword;
        userModel.Status = (short)UserStatusEnum.ACTIVE;
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
    
    public async Task<(string userId, string password)?> GetUserIdAndPasswordByEmailAsync(string email)
    {
        

        var result = await dbContext.UserAccount
            .Where(u => u.Email == email) // Filter by email
            .Select(u => new { u.UserId, u.Password }) // Select only userId and password
            .FirstOrDefaultAsync(); // Fetch first matching record

        return result == null ? null : (result.UserId.ToString(), result.Password);
        
    }

    public AuthResponse GenerateAuthResponse(string email, string userId)
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