namespace DoctorService.Models;

public class UserModel
{
    public int? UserId { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set;}
    public short? Status { get; set;}
}