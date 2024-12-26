using DoctorService.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace UserService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService userService;
    public UserController(UserService userService)
    {
        this.userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!userService.UserExists(request.Email))
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }
        
        return await LoginUser(request);
    }
    
    private async Task<IActionResult> LoginUser(LoginRequest request)
    {
        try
        {
            var result = await userService.GetUserIdAndPasswordByEmailAsync(request.Email);
            var passwordHash = result.Value.password;
            var userId = result.Value.userId;
            if (string.IsNullOrEmpty(passwordHash) || string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { message = "Invalid username or password." });
            }
            
            if (userService.VerifyPassword(request.Password, passwordHash))
            {
                return Ok(userService.GenerateAuthResponse(request.Email, userId));
            }
            return BadRequest(new { message = "Invalid username or password." });
        }
        catch (Exception ex)
        {
            return BadRequest("Error occured: " + ex.Message);
        }
    }
    
    [HttpPost("registerDoctor")]
    public async Task<IActionResult> RegisterDoctor([FromBody] DoctorModel doctorModel)
    {
        
        if (userService.UserExists(doctorModel.Email))
        {
            return BadRequest("User already exists.");
        }

        doctorModel.UserId = await userService.RegisterUserInternal(doctorModel);

        if (doctorModel.UserId is -1 or null)
        {
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }

        return Ok(doctorModel);
    }
    public bool UserExistsStatic(string email)
    {
        return userService.UserExists(email);
    }
    
}