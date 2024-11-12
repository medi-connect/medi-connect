using FeedbackService.Models;
using Microsoft.AspNetCore.Mvc;

namespace FeedbackService.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class MainController : ControllerBase
{
    [HttpGet("getFeedback")]
    public FeedbackModel GetFeedback()
    {
        return new FeedbackModel
        {
            Id = Guid.NewGuid(),
            Title = "Feedback Service",
            Content = "This is a feedback service.",
            Rating = 9
        };
    }
    
    
}