using AppointmentService.Enums;
using System.Net;
using System.Text;
using System.Text.Json;
using AppointmentService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentService.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

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

        var doctorExists = await CheckIfDoctorExists(appointment.DoctorId);
        if (!doctorExists)
            return NotFound("Doctor not found.");
        
        var patientExists = await CheckIfPatientExists(appointment.PatientId);
        if (!patientExists)
            return NotFound("Patient not found.");

        dbContext.Appointment.Add(appointment);
        await dbContext.SaveChangesAsync();
        await NotifyFunctionApp(new EmailAppointmentModel
        {
            Id = appointment.Id,
            StartTime = appointment.StartTime,
            Title = appointment.Title,
            Description = appointment.Description
        });
        
        return Ok("Appointment added successfully.");
    }

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
    
    [HttpGet("getDoneAppointments")]
    public async Task<ActionResult<List<AppointmentModel>>> FetDoneAppointments()
    {
        var appointments = await dbContext.Appointment
            .Where(a => a.Status == AppointmentStatus.DONE)
            .ToListAsync();

        if (!appointments.Any())
            return NotFound("Appointments in status DONE do not exist.");

        return Ok(appointments);
    }
    
    [HttpPut("modifyStatus/{id}")]
    public async Task<ActionResult> ModifyStatus(int id, [FromBody] StatusDTO updateDto)
    {
        if (updateDto == null || !Enum.IsDefined(typeof(AppointmentStatus), updateDto.Status))
            return BadRequest("Invalid or missing status.");

        var affectedRows = await dbContext.Database.ExecuteSqlRawAsync(
            "UPDATE dbo.Appointment SET status = {0} WHERE appointment_id = {1}",
            updateDto.Status,
            id);

        if (affectedRows == 0)
            return NotFound("Appointment not found.");

        return Ok($"Appointment status updated to '{updateDto.Status}'.");
    }

    [HttpPut("modifyDescription/{id}")]
    public async Task<ActionResult> ModifyDescription(int id, [FromBody] DescriptionDTO? descriptionDto)
    {
        if (descriptionDto == null || string.IsNullOrWhiteSpace(descriptionDto.Description))
            return BadRequest("Invalid or missing description.");

        const string query = @"
        UPDATE dbo.Appointment SET description = @description WHERE appointment_id = @id;";

        try
        {
            await dbContext.Database.ExecuteSqlRawAsync(
                query,
                new SqlParameter("@description", descriptionDto.Description),
                new SqlParameter("@id", id)
            );

            return Ok("Description updated successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    private async Task<bool> CheckIfDoctorExists(int doctorId)
    {
        try
        {
            var url = $"http://doctor-service:8003/api/v1/doctor/getDoctor/{doctorId}";
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NoContent)
            {
                return false;
            }

            var exists = await response.Content.ReadAsStringAsync();
            return !exists.IsNullOrEmpty();
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    private async Task<bool> CheckIfPatientExists(int patientId)
    {
        try
        {
            var url = $"http://user-service:8001/api/v1/patient/getPatient/{patientId}";
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NoContent)
            {
                return false;
            }

            var exists = await response.Content.ReadAsStringAsync();
            return !exists.IsNullOrEmpty();
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    [HttpPost("sendEmail")]
    public async Task<bool> NotifyFunctionApp([FromBody] EmailAppointmentModel appointment)
    {
        var httpClient = new HttpClient();
        var key = Environment.GetEnvironmentVariable("APPOINTMENT_FUNCTION_KEY");
        var functionUrl = $"http://SendAppointmentEmail.azurewebsites.net/api/SendAppointmentEmail?code={key}";
        var jsonPayload = JsonSerializer.Serialize(appointment);

        // Create an HttpContent object (StringContent) with the JSON payload
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        // Send POST request
        var response = await httpClient.PostAsync(functionUrl, content);
        return response.IsSuccessStatusCode;
    }
}