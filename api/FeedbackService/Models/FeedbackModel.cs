namespace FeedbackService.Models;

public class FeedbackModel
{
    public int? FeedbackId { get; set; }
    public byte? Rate { get; set; }
    public string? Review { get; set; }
    public int? AppointmentId { get; set; }
}