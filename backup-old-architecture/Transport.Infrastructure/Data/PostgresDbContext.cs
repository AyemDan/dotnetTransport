using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Data;

public class PostgresDbContext : DbContext
{
    public PostgresDbContext() { }

    public PostgresDbContext(DbContextOptions<PostgresDbContext> options)
        : base(options) { }

    public DbSet<Provider> Providers { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Employee> Employees { get; set; } // Added Employees DbSet

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=root"
            );
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("Users"); // Base table
        modelBuilder.Entity<Student>().ToTable("Students");
        modelBuilder.Entity<Provider>().ToTable("Providers");
        modelBuilder.Entity<Employee>().ToTable("Employees");

        // Define the one-to-many relationship between Provider and Employees
        modelBuilder
            .Entity<Employee>()
            .HasOne(e => e.Provider)
            .WithMany(p => p.Employees)
            .HasForeignKey(e => e.ProviderId)
            .OnDelete(DeleteBehavior.Cascade); // If a Provider is deleted, delete their Employees too
    }
}
