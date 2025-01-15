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

    /// <summary>
    /// Process login request.
    /// </summary>
    /// <param name="request">Login request.</param>
    /// <returns>The created user.</returns>
    /// <response code="200">Returns JWT token</response>
    /// <response code="400">If error occured or required fields are null</response>
    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest request)
    {
        if (!userService.UserExists(request.Email))
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }
        
        return await LoginUser(request);
    }
    
    /// <summary>
    /// Registers a doctor.
    /// </summary>
    /// <param name="doctor">The doctor object to be created.</param>
    /// <returns>The created doctor.</returns>
    /// <response code="200">Returns the newly created doctor</response>
    /// <response code="400">If the doctor already exists</response>
    /// <response code="500">If error occured</response>
    [HttpPost("registerDoctor")]
    public async Task<ActionResult> RegisterDoctor([FromBody] DoctorModel doctor)
    {
        
        if (userService.UserExists(doctor.Email))
        {
            return BadRequest("User already exists.");
        }

        doctor.UserId = await userService.RegisterUserInternal(doctor, true);

        if (doctor.UserId is -1 or null)
        {
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }

        return Ok(doctor);
    }

    private async Task<ActionResult> LoginUser(LoginRequest request)
    {
        try
        {
            var result = await userService.GetUserIdAndPasswordByEmailAsync(request.Email);
            var passwordHash = result.Value.password;
            var userId = result.Value.userId;
            var isDoctor = result.Value.isDoctor;
            if (string.IsNullOrEmpty(passwordHash) || string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { message = "Invalid username or password." });
            }
            
            if (userService.VerifyPassword(request.Password, passwordHash))
            {
                return Ok(userService.GenerateAuthResponse(request.Email, userId, isDoctor));
            }
            return BadRequest(new { message = "Invalid username or password." });
        }
        catch (Exception ex)
        {
            return BadRequest("Error occured: " + ex.Message);
        }
    }
    
    [NonAction]
    public bool UserExistsStatic(string email)
    {
        return userService.UserExists(email);
    }
    
}