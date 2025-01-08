namespace UserService.Models;

public class UserModel
{
    public int? UserId { get; set; }
    public string? Email { get; set;}
    public string? Password { get; set;}
    public byte? Status { get; set;}
    public byte? IsDoctor { get; set;}

}