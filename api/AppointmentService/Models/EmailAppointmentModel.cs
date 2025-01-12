namespace AppointmentService.Models;

public class EmailAppointmentModel
{
    public int? Id { get; set; }
    public DateTime? StartTime { get; set; } 
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}