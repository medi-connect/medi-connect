using Microsoft.EntityFrameworkCore;
using AppointmentService.Models;

namespace AppointmentService.Utils;

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<AppointmentModel> Appointment { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppointmentModel>(entity =>
        {
            entity.ToTable("Appointment", "dbo");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("appointment_id").ValueGeneratedOnAdd();
            entity.Property(e => e.StartTime).HasColumnName("start_time").IsRequired();
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(255).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Status).HasColumnName("status").IsRequired().HasConversion<int>();
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id").IsRequired();
            entity.Property(e => e.PatientId).HasColumnName("patient_id").IsRequired();
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired();
            entity.Property(e => e.SysTimestamp).HasColumnName("sys_timestamp").IsRequired();
            entity.Property(e => e.SysCreated).HasColumnName("sys_created").IsRequired();
        });
    }
}