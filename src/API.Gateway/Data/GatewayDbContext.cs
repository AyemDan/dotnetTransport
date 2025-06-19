using Microsoft.EntityFrameworkCore;
using Transport.Shared.Entities;

namespace API.Gateway.Data;

public class GatewayDbContext : DbContext
{
    public GatewayDbContext(DbContextOptions<GatewayDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Provider> Providers { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Organization> Organizations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=TransportApp;Username=postgres;Password=root"
            );
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure User table
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Role).IsRequired();
        });

        // Configure Student table
        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Students");
            entity.HasKey(e => e.Id);
        });

        // Configure Provider table
        modelBuilder.Entity<Provider>(entity =>
        {
            entity.ToTable("Providers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.LicenseNumber).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.LicenseNumber).IsUnique();
        });

        // Configure Admin table
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.ToTable("Admins");
            entity.HasKey(e => e.Id);
        });

        // Configure Organization table
        modelBuilder.Entity<Organization>(entity =>
        {
            entity.ToTable("Organizations");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
        });

        base.OnModelCreating(modelBuilder);
    }
} 