using FeedbackService.Models;
using FeedbackService.Utils;
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
                      WHERE feedback_id = @FeedbackId";

            var feedback = await dbContext.Database
                .SqlQueryRaw<FeedbackModel>(query, new SqlParameter("@FeedbackId", id))
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (feedback is null)
            {
                return NotFound();
            }

            return Ok(feedback);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
    
    /* =============================
    * POST METHODS
    =============================*/ 
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
            return BadRequest(e);
        }
    }
}