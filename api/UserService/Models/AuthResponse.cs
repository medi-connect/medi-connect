namespace UserService.Models;

public class AuthResponse
{
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
    public string UserId { get; set; }
    public bool IsDoctor { get; set; }
}