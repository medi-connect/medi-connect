using AppointmentService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentService.Utils;

namespace AppointmentService.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class AppointmentController: ControllerBase
{
    private readonly HttpClient httpClient;
    private readonly ApplicationDbContext dbContext;

    public AppointmentController(HttpClient httpClient, ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
        this.httpClient = httpClient;
    }
    
    [HttpPost("createAppointment")]
    public async Task<ActionResult> CreateAppointment([FromBody] AppointmentModel appointment)
    {
        if (await dbContext.Appointment.AnyAsync(a => a.Id == appointment.Id))
            return BadRequest("Appointment with the given ID already exists.");

        /*var doctorExists = await CheckIfDoctorExists(appointment.DoctorId);
        if (!doctorExists)
            return NotFound("Doctor not found.");

        var patientExists = await CheckIfPatientExists(appointment.PatientId);
        if (!patientExists)
            return NotFound("Patient not found.");*/

        dbContext.Appointment.Add(appointment);
        await dbContext.SaveChangesAsync();

        return Ok("Appointment added successfully.");
    }
    // TODO: connect these with dbcontext of Patient/Doctor (usage in addAppointment - function above)
    /*private async Task<bool> CheckIfDoctorExists(int doctorId)
    {
        return await dbContext.Appointment.AnyAsync(a => a.DoctorId == doctorId);
    }

    private async Task<bool> CheckIfPatientExists(int patientId)
    {
        return await dbContext.Appointment.AnyAsync(a => a.PatientId == patientId);
    }*/
    [HttpGet("getAppointment/{id}")]
    public async Task<ActionResult<AppointmentModel>> GetAppointment(int id)
    {
        try
        {
			var appointment = await dbContext.Appointment.FindAsync(id);
        	if (appointment == null)
            	return NotFound("Appointment not found.");

        	return Ok(appointment);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("getAppointmentsForPatient/{patientId}")]
    public async Task<ActionResult<List<AppointmentModel>>> GetAppointmentsForPatient(int patientId)
    {
        var appointments = await dbContext.Appointment
            .Where(a => a.PatientId == patientId)
            .ToListAsync();

        if (!appointments.Any())
            return NotFound("No appointments found for the specified patient.");

        return Ok(appointments);
    }

    [HttpGet("getAppointmentsForDoctor/{doctorId}")]
    public async Task<ActionResult<List<AppointmentModel>>> GetAppointmentsForDoctor(int doctorId)
    {
        var appointments = await dbContext.Appointment
            .Where(a => a.DoctorId == doctorId)
            .ToListAsync();

        if (!appointments.Any())
            return NotFound("No appointments found for the specified doctor.");

        return Ok(appointments);
    }
    //TODO: FOR FIX.
    /*[HttpPut("modifyStatus/{id}")]
    public async Task<ActionResult> ModifyStatus(int id, [FromBody] StatusDTO updateDto)
    {
        if (updateDto == null || !Enum.IsDefined(typeof(AppointmentStatus), updateDto.Status))
            return BadRequest("Invalid or missing status.");

        var appointment = await dbContext.Appointment.FindAsync(id);
        if (appointment == null)
            return NotFound("Appointment not found.");

        appointment.Status = updateDto.Status;
        dbContext.Appointment.Update(appointment);
        await dbContext.SaveChangesAsync();

        return Ok($"Appointment status updated to '{updateDto.Status}'.");
    }*/
}