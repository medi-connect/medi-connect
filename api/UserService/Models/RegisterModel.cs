namespace UserService.Models;

public class RegisterModel : UserModel
{
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Speciality { get; set; }
}