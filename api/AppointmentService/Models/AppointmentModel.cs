namespace AppointmentService.Models;

public class AppointmentModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; } 
    public string? Location { get; set; }
    public DoctorModel doctor { get; set; }
}