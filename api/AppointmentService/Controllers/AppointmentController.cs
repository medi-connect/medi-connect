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
    
    /* =============================
    * GET METHODS
    =============================*/ 
    /// <summary>
    /// Retrieves a specific appointment by their ID.
    /// </summary>
    /// <param name="id">The ID of the appointment to retrieve.</param>
    /// <returns>The appointment with the specified ID.</returns>
    /// <response code="200">Returns the appointment with the specified ID</response>
    /// <response code="400">If the appointment is not found</response>
    /// <response code="500">If internal error occured</response>
    [HttpGet("getAppointment/{id}")]
    public async Task<ActionResult<AppointmentModel>> GetAppointment(int id)
    {
        try
        {
			var appointment = await dbContext.Appointment.FindAsync(id);
        	if (appointment == null)
            	return BadRequest("Appointment not found.");

        	return Ok(appointment);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Retreives all appointments for a specific patient.
    /// </summary>
    /// <param name="patientId">The ID of the patient.</param>
    /// <returns>List of appointments for specified patient.</returns>
    /// <response code="200">Returns list of appointments for specified patient.</response>
    /// <response code="400">If the appointments for specific patient are not found</response>
    /// <response code="500">If internal error occured</response>
    [HttpGet("getAppointmentsForPatient/{patientId}")]
    public async Task<ActionResult<List<AppointmentModel>>> GetAppointmentsForPatient(int patientId)
    {
        try
        {
            var appointments = await dbContext.Appointment
                .Where(a => a.PatientId == patientId)
                .ToListAsync();

            if (!appointments.Any())
                return BadRequest("No appointments found for the specified patient.");

            return Ok(appointments);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Retreives all appointments for a specific doctor.
    /// </summary>
    /// <param name="doctorId">The ID of the doctor.</param>
    /// <returns>List of appointments for specified doctor.</returns>
    /// <response code="200">Returns list of appointments for specified doctor.</response>
    /// <response code="400">If the appointments for specific doctor are not found</response>
    /// <response code="500">If internal error occured</response>
    [HttpGet("getAppointmentsForDoctor/{doctorId}")]
    public async Task<ActionResult<List<AppointmentModel>>> GetAppointmentsForDoctor(int doctorId)
    {
        try
        {
            var appointments = await dbContext.Appointment
                .Where(a => a.DoctorId == doctorId)
                .ToListAsync();

            if (!appointments.Any())
                return BadRequest("No appointments found for the specified doctor.");

            return Ok(appointments);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Retrieves a list all appointments in status DONE.
    /// </summary>
    /// <returns>A list of appointments.</returns>
    /// <response code="200">Returns the list of done appointments</response>
    /// <response code="400">If no appointments are found</response>
    /// <response code="500">If internal error occured</response>
    [HttpGet("getDoneAppointments")]
    public async Task<ActionResult<List<AppointmentModel>>> FetDoneAppointments()
    {
        try
        {
            var appointments = await dbContext.Appointment
                .Where(a => a.Status == AppointmentStatus.DONE)
                .ToListAsync();

            if (!appointments.Any())
                return BadRequest("Appointments in status DONE do not exist.");

            return Ok(appointments);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    /* =============================
    * POST METHODS
    =============================*/ 
    /// <summary>
    /// Creates a new appointment.
    /// </summary>
    /// <param name="appointment">The appointment object to be created.</param>
    /// <returns>The created user.</returns>
    /// <response code="200">Returns confirmation</response>
    /// <response code="400">If the request body is invalid</response>
    /// <response code="500">If internal error occured</response>
    [HttpPost("createAppointment")]
    public async Task<ActionResult> CreateAppointment([FromBody] AppointmentModel appointment)
    {
        try
        {
            if (await dbContext.Appointment.AnyAsync(a => a.Id == appointment.Id))
                return BadRequest("Appointment with the given ID already exists.");

            var doctorExists = await CheckIfDoctorExists(appointment.DoctorId);
            if (!doctorExists)
                return BadRequest("Doctor not found.");

            var patientExists = await CheckIfPatientExists(appointment.PatientId);
            if (!patientExists)
                return BadRequest("Patient not found.");

            dbContext.Appointment.Add(appointment);
            await dbContext.SaveChangesAsync();
            await NotifyByEmail(new EmailAppointmentModel
            {
                Id = appointment.Id,
                StartTime = appointment.StartTime,
                Title = appointment.Title,
                Description = appointment.Description
            });

            return Ok("Appointment added successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    /* =============================
    * PUT METHODS
    =============================*/ 
    /// <summary>
    /// Updates status for an existing appointment.
    /// </summary>
    /// <param name="id">The ID of the appointment to update.</param>
    /// <param name="statusDto">The new status.</param>
    /// <response code="200">Confirmation</response>
    /// <response code="400">If the request body is invalid</response>
    /// <response code="500">If internal error occured</response>
    [HttpPut("modifyStatus/{id}")]
    public async Task<ActionResult> ModifyStatus(int id, [FromBody] StatusDTO statusDto)
    {
        try
        {
            if (statusDto == null || !Enum.IsDefined(typeof(AppointmentStatus), statusDto.Status))
                return BadRequest("Invalid or missing status.");

            var affectedRows = await dbContext.Database.ExecuteSqlRawAsync(
                "UPDATE dbo.Appointment SET status = {0} WHERE appointment_id = {1}",
                statusDto.Status,
                id);

            if (affectedRows == 0)
                return BadRequest("Appointment not found.");

            return Ok($"Appointment status updated to '{statusDto.Status}'.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates description for an existing appointment.
    /// </summary>
    /// <param name="id">The ID of the appointment to update.</param>
    /// <param name="descriptionDto">The new description.</param>
    /// <response code="200">Confirmation</response>
    /// <response code="400">If the request body is invalid</response>
    /// <response code="500">If internal error occured</response>
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
    
    private async Task<bool> NotifyByEmail([FromBody] EmailAppointmentModel appointment)
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