using Microsoft.AspNetCore.Mvc;
using UserService.Models;

namespace UserService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MainController : ControllerBase
{
    [HttpGet("getMainData")]
    public UserModel GetMainData()
    {
        var a = new UserModel
        {
            Name = "Test",
            Email = "test@test.com",
            Age = 21
        };
        return a;
    }
}