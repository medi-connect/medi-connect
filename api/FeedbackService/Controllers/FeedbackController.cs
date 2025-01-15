using FeedbackService.Models;
using FeedbackService.Utils;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FeedbackService.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class FeedbackController : ControllerBase
{
    private readonly ApplicationDbContext dbContext;

    public FeedbackController(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    
    /* =============================
    * GET METHODS
    =============================*/ 
    
    /// <summary>
    /// Retrieves a specific feedback by their ID.
    /// </summary>
    /// <param name="id">The ID of the feedback to retrieve.</param>
    /// <returns>The feedback with the specified ID.</returns>
    /// <response code="200">Returns the feedback with the specified ID</response>
    /// <response code="400">If the feedback is not found or required parameter is null</response>
    /// <response code="500">If error occurs</response>
    [HttpGet("getFeedback/{id}")]
    public async Task<ActionResult<FeedbackModel>> GetFeedback([FromRoute] int? id)
    {
        if (id is null or < 1)
        {
            return BadRequest("Invalid id");
        }

        try
        {
            var query = @"SELECT feedback_id AS FeedbackId, 
                             rate AS Rate, 
                             review AS Review, 
                             appointment_id AS AppointmentId
                      FROM dbo.Feedback 
                      WHERE appointment_id = @AppointmentId";

            var feedback = await dbContext.Database
                .SqlQueryRaw<FeedbackModel>(query, new SqlParameter("@AppointmentId", id))
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (feedback is null)
            {
                return BadRequest();
            }

            return Ok(feedback);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    
    [NonAction]
    [HttpGet("getFeedbacksForDoneAppointments")]
    public async Task<ActionResult<List<FeedbackModel>>> GetFeedbackForDoneAppointments()
    {
        try
        {
            using var channel = GrpcChannel.ForAddress("http://appointment-service:8004");
            var client = new DoneAppointmentsService.DoneAppointmentsServiceClient(channel);
            var request = new AppointmentRequest();

            var response = client.GetDoneAppointments(request);
            var appointments = response.Appointments.ToList();
            var appointmentsIds = appointments.Select(appt => appt.Id).ToList();

            var feedbacks = await dbContext.Feedback
                .Where(feedback => appointmentsIds.Contains((int)feedback.AppointmentId))
                .ToListAsync();
            return Ok(feedbacks);

        }
        catch (Exception e)
        {
            return BadRequest(e);   
        }
        
    }
    
    /* =============================
    * POST METHODS
    =============================*/ 
    /// <summary>
    /// Creates a new feedback.
    /// </summary>
    /// <param name="feedback">The feedback object to be created.</param>
    /// <returns>Confirmation.</returns>
    /// <response code="200">Returns confirmation that feedback was created</response>
    /// <response code="400">If the request body is invalid</response>
    /// <response code="500">If error occured</response>
    [HttpPost("addFeedback")]
    public async Task<ActionResult<FeedbackModel>> AddFeedback([FromBody] FeedbackModel? feedback)
    {
        if (feedback is null)
        {
            return BadRequest("Invalid id");
        }

        if (feedback.Rate is null || feedback.Review is null || feedback.AppointmentId is null)
        {
            return BadRequest("Required fields are missing \n" + feedback.ToString());
        }

        if (feedback.Rate < 1 || feedback.Rate > 5)
        {
            return BadRequest("Rate must be between 1 and 5");
        }
        
        try
        {
            dbContext.Feedback.Add(feedback);
            await dbContext.SaveChangesAsync();
            var feedbackId = feedback.FeedbackId;

            return Ok($"Created feedback, ID: {feedbackId}");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}