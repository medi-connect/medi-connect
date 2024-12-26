using UserService.Models;

namespace DoctorService.Models;

public class DoctorModel : UserModel
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Speciality { get; set; }
}