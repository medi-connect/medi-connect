using FeedbackService.Models;
using Microsoft.EntityFrameworkCore;

namespace FeedbackService.Utils;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<FeedbackModel> Feedback { get; set; }    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FeedbackModel>(entity =>
        {
            entity.ToTable("Feedback", "dbo"); // Base table
            entity.HasKey(e => e.FeedbackId);
            entity.Property(e => e.FeedbackId).HasColumnName("feedback_id").ValueGeneratedOnAdd();
            entity.Property(e => e.Rate).HasColumnName("rate");
            entity.Property(e => e.Review).HasColumnName("review");
            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
        });
        
    }

}