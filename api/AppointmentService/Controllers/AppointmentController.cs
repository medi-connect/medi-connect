using System.Data;
using AppointmentService.Enums;
using AppointmentService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AppointmentService.Controllers;

[ApiController]
[Route("/api/v1/appointment")]
public class AppointmentController: ControllerBase
{
    private readonly SqlConnection connection; 
    private readonly string _connectionString;
    private readonly HttpClient httpClient;
    private readonly DbContext dbContext;

    public AppointmentController(IConfiguration configuration, HttpClient httpClient, DbContext dbContext)
    {
        _connectionString = configuration["DB_CONNECTION_STRING"] ?? throw new Exception("Connection string not found");
        connection = new SqlConnection(_connectionString);
        connection.Open();
        this.dbContext = dbContext;
        this.httpClient = httpClient;
    }
    
    [HttpGet("getAppointment/{id}")]
    public async Task<ActionResult<AppointmentModel>> GetAppointment(int id)
    {
        try
        {
            string query = "SELECT * FROM Appointment WHERE id = @Id";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return Ok(new AppointmentModel
                {
                    Id = reader.GetInt32("id"),
                    StartTime = reader.GetDateTime("start_time"),
                    EndTime = reader.IsDBNull("end_time") ? (DateTime?)null : reader.GetDateTime("end_time"),
                    Title = reader.GetString("title"),
                    Description = reader.IsDBNull("description") ? null : reader.GetString("description"),
                    Status = reader.GetString("status"),
                    DoctorId = reader.GetInt32("doctor_id"),
                    PatientId = reader.GetInt32("patient_id"),
                    CreatedBy = reader.GetBoolean("created_by"),
                    SysTimestamp = reader.GetDateTime("sys_timestamp"),
                    SysCreated = reader.GetDateTime("sys_created")
                });
            }

            return NotFound("Appointment not found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("addAppointment")]
    public async Task<ActionResult> AddDoctor([FromBody] AppointmentModel appointment)
    {
		
        try
        {
			//using var connection = new SqlConnection(_connectionString);
            //await connection.OpenAsync();
            
            bool doctorExists = await CheckIfDoctorExists(appointment.DoctorId);
            if (!doctorExists)
            {
                return NotFound("Doctor not found.");
            }
            
            bool patientExists = await CheckIfPatientExists(appointment.PatientId);
            if (!patientExists)
            {
                return NotFound("Patient not found.");
            }
            
            string commandText = @"
                INSERT INTO Appointment 
                (start_time, end_time, title, description, status, doctor_id, patient_id, created_by, sys_timestamp, sys_created)
                VALUES (@StartTime, @EndTime, @Title, @Description, @Status, @DoctorId, @PatientId, @CreatedBy, @SysTimestamp, @SysCreated)";

			using var command = new SqlCommand(commandText, connection);
            command.Parameters.AddWithValue("@StartTime", appointment.StartTime);
            command.Parameters.AddWithValue("@EndTime", (object?)appointment.EndTime ?? DBNull.Value);
            command.Parameters.AddWithValue("@Title", appointment.Title);
            command.Parameters.AddWithValue("@Description", (object?)appointment.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", appointment.Status);
            command.Parameters.AddWithValue("@DoctorId", appointment.DoctorId);
            command.Parameters.AddWithValue("@PatientId", appointment.PatientId);
            command.Parameters.AddWithValue("@CreatedBy", appointment.CreatedBy);
            command.Parameters.AddWithValue("@SysTimestamp", appointment.SysTimestamp);
            command.Parameters.AddWithValue("@SysCreated", appointment.SysCreated);

            await command.ExecuteNonQueryAsync();
            return Ok("Appointment added successfully.");
        }
		catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("getAppointmentsForPatient/{patientId}")]
    public async Task<ActionResult<List<AppointmentModel>>> GetAppointmentsForPatient(int patientId)
    {
        try
        {
            string commandText = "SELECT * FROM Appointment WHERE patient_id = @PatientId";
            using var command = new SqlCommand(commandText, connection);
            command.Parameters.AddWithValue("@PatientId", patientId);

            using var reader = await command.ExecuteReaderAsync();
            var appointments = new List<AppointmentModel>();

            while (await reader.ReadAsync())
            {
                appointments.Add(new AppointmentModel
                {
                    Id = reader.GetInt32("id"),
                    StartTime = reader.GetDateTime("start_time"),
                    EndTime = reader.IsDBNull("end_time") ? (DateTime?)null : reader.GetDateTime("end_time"),
                    Title = reader.GetString("title"),
                    Description = reader.IsDBNull("description") ? null : reader.GetString("description"),
                    Status = reader.GetString("status"),
                    DoctorId = reader.GetInt32("doctor_id"),
                    PatientId = reader.GetInt32("patient_id"),
                    CreatedBy = reader.GetBoolean("created_by"),
                    SysTimestamp = reader.GetDateTime("sys_timestamp"),
                    SysCreated = reader.GetDateTime("sys_created")
                });
            }

            return Ok(appointments);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("getAppointmentsForDoctor/{doctorId}")]
    public async Task<ActionResult<List<AppointmentModel>>> GetAppointmentsForDoctor(int doctorId)
    {
        try
        {
            string commandText = "SELECT * FROM Appointment WHERE doctor_id = @DoctorId";
            using var command = new SqlCommand(commandText, connection);
            command.Parameters.AddWithValue("@DoctorId", doctorId);

            using var reader = await command.ExecuteReaderAsync();
            var appointments = new List<AppointmentModel>();

            while (await reader.ReadAsync())
            {
                appointments.Add(new AppointmentModel
                {
                    Id = reader.GetInt32("id"),
                    StartTime = reader.GetDateTime("start_time"),
                    EndTime = reader.IsDBNull("end_time") ? (DateTime?)null : reader.GetDateTime("end_time"),
                    Title = reader.GetString("title"),
                    Description = reader.IsDBNull("description") ? null : reader.GetString("description"),
                    Status = reader.GetString("status"),
                    DoctorId = reader.GetInt32("doctor_id"),
                    PatientId = reader.GetInt32("patient_id"),
                    CreatedBy = reader.GetBoolean("created_by"),
                    SysTimestamp = reader.GetDateTime("sys_timestamp"),
                    SysCreated = reader.GetDateTime("sys_created")
                });
            }

            return Ok(appointments);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    public async Task<bool> CheckIfDoctorExists(int id)
    {
        try
        {
            string query = "SELECT COUNT(1) FROM Doctor WHERE id = @id";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
        
            var result = (int)(await command.ExecuteScalarAsync() ?? 0);
            return result > 0;
        }
        catch (Exception ex)
        {
            throw new Exception("Error while checking if doctor exists: " + ex.Message);
        }
    }
    
    public async Task<bool> CheckIfPatientExists(int id)
    {
        try
        {
            string query = "SELECT COUNT(1) FROM Patient WHERE id = @id";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
        
            var result = (int)(await command.ExecuteScalarAsync() ?? 0);
            return result > 0;
        }
        catch (Exception ex)
        {
            throw new Exception("Error while checking if patient exists: " + ex.Message);
        }
    }
    
    public async Task<bool> CheckIfAppointmentExists(int id)
    {
        try
        {
            string query = "SELECT COUNT(1) FROM Appointment WHERE appointment_id = @id";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
        
            var result = (int)(await command.ExecuteScalarAsync() ?? 0);
            return result > 0;
        }
        catch (Exception ex)
        {
            throw new Exception("Error while checking if appointment exists: " + ex.Message);
        }
    }
    
    [HttpPut("modifyStatus/{id}")]
    public async Task<ActionResult> ModifyStatus(int id, [FromBody] AppointmentStatus? status)
    {
        try
        {
            //todo: check appointmentExists - another response received upon testing
            bool appointmentExists = await CheckIfAppointmentExists(id);
            if (!appointmentExists)
            {
                return NotFound($"No appointment found with ID {id}.");
            }
            if (status == null)
            {
                return BadRequest("Status cannot be empty.");
            }

            string commandText = "UPDATE Appointment SET status = @Status WHERE appointment_id = @Id";
            using var command = new SqlCommand(commandText, connection);
            command.Parameters.AddWithValue("@status", status);
            command.Parameters.AddWithValue("@appointment_id", id);

            int rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                return Ok($"Appointment with ID {id} successfully updated to status '{status}'.");
            }
            return BadRequest($"Error");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

}