using UserService.Models;

namespace DoctorService.Models;

public class DoctorModel : UserDTO
{
	public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Speciality { get; set; }
    public DateTime BirthDate { get; set; }
}