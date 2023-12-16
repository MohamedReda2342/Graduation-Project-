namespace WebApi.Helpers;

using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

public class DataContext : DbContext
{
    protected readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        //// connect to sql server database
        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

    }

    public DbSet<User> Users { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Medicine> Medicines { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Your other entity configurations...

        // Apply the unique index for the combination of UserId and PhoneNumber in the Patient entity
        modelBuilder.Entity<Patient>()
            .HasIndex(p => new { p.UserId, p.PhoneNumber })
            .IsUnique();


    }


}