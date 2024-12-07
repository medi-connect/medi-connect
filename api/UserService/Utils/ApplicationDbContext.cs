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
            // Specify the table name and schema here
            entity.ToTable("UserAccount", "dbo");

            entity.HasKey(e => e.Id);
            // Map properties to columns, specifying snake_case names
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Status).HasColumnName("status");
        });
    }
}