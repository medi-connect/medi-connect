namespace UserService.Models;

public class RegisterDTO
{
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string Email { get; set; }

    public required string Password { get; set; }
    public required DateTime BirthDate { get; set; }
}