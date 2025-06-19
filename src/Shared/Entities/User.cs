using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Transport.Shared.Enums;

namespace Transport.Shared.Entities;

public abstract class User
{
    protected User() { } // Parameterless constructor (does nothing)

    protected User(string email, string password)
    {
        Id = Guid.NewGuid();
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PasswordHash = HashPassword(password ?? throw new ArgumentNullException(nameof(password)));
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

    [Required]
    [Column(TypeName = "varchar(20)")]
    public UserRole Role { get; set; }
    public string FullName { get; } = string.Empty;

    public Guid? OrganizationId { get; set; } // Null for platform Admins

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime? LastLogoutAt { get; set; }

    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public void UpdatePassword(string oldPassword, string newPassword)
    {
        if (VerifyPassword(oldPassword))
        {
            PasswordHash = HashPassword(newPassword);
        }
        else
        {
            throw new UnauthorizedAccessException("Old password is incorrect.");
        }
    }

    public bool VerifyPassword(string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
    }

    public void SetPassword(string password)
    {
        PasswordHash = HashPassword(password);
    }
}

public class OrganizationAdmin : User
{
    public OrganizationAdmin(string email, string password, Guid organizationId) : base(email, password)
    {
        Role = UserRole.OrganizationAdmin;
        OrganizationId = organizationId;
    }
}

public class AppUser : User
{
    public AppUser() : base() { }
    public AppUser(string email, string password) : base(email, password) { }
} 