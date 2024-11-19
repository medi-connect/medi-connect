namespace UserService.Models;

public class RegisterDTO
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }

    public string Password { get; set; }
    public DateTime BirthDate { get; set; }
}