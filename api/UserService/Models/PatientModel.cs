namespace UserService.Models;

public class PatientModel : UserModel
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public DateTime BirthDate { get; set; }
}