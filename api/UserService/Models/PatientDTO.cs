namespace UserService.Models;

public class PatientDTO : UserDTO
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public DateTime BirthDate { get; set; }
}