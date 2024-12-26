using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Controllers;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<UserModel> UserAccount { get; set; }    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserModel>(entity =>
        {
            entity.ToTable("UserAccount", "dbo"); // Base table
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).HasColumnName("user_id").ValueGeneratedOnAdd();
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<PatientModel>(entity =>
        {
            entity.ToTable("Patient", "dbo"); // Derived table
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Surname).HasColumnName("surname");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");

        });
    }

}