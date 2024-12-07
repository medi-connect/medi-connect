namespace AppointmentService.Models;

public class AppointmentModel
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; } 
    public DateTime? EndTime { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "Pending";
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public bool CreatedBy { get; set; } // False for Patient, True for Doctor
    public DateTime SysTimestamp { get; set; } = DateTime.Now;
    public DateTime SysCreated { get; set; } = DateTime.Now;
    
    public AppointmentModel() { }
    public AppointmentModel(int id, DateTime startTime, DateTime? endTime, string title, string? description, string status, int doctorId, int patientId, bool createdBy)
    {
        Id = id;
        StartTime = startTime;
        EndTime = endTime;
        Title = title;
        Description = description;
        Status = status;
        DoctorId = doctorId;
        PatientId = patientId;
        CreatedBy = createdBy;
        SysTimestamp = DateTime.Now; // Initialized when created
        SysCreated = DateTime.Now;  // Initialized when created
    }
    public void UpdateStatus(string newStatus)
    {
        Status = newStatus;
        SysTimestamp = DateTime.Now; // Update timestamp when a change occurs
    }
    public void UpdateDetails(string title, string? description, DateTime startTime, DateTime? endTime)
    {
        Title = title;
        Description = description;
        StartTime = startTime;
        EndTime = endTime;
        SysTimestamp = DateTime.Now;
    }
    public override string ToString()
    {
        return $"Appointment [Id={Id}, Title={Title}, Status={Status}, StartTime={StartTime}, EndTime={EndTime}, DoctorId={DoctorId}, PatientId={PatientId}]";
    }
    public bool IsOverlapping(DateTime otherStartTime, DateTime otherEndTime)
    {
        return StartTime < otherEndTime && otherStartTime < EndTime;
    }
}