namespace AppointmentService.Models;

public class AppointmentModel
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; } 
    public string? Location { get; set; }
}