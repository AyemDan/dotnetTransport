using Transport.Shared.Enums;

namespace Transport.Shared.Entities;

public class Provider : User
{
    public Provider() 
    {
        Employees = new List<Employee>(); // Ensure Employees is initialized
        IsActive = false; // Default to inactive when created
    }

    public Provider(string email, string password) : base(email, password)
    {
        Role = UserRole.Provider;
        Employees = new List<Employee>(); // Ensure Employees is initialized
        IsActive = false; // Default to inactive when created
    }

    public string CompanyName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string? Owner { get; set; } 
    public string? Manager { get; set; } 
    public string? ContactNumber { get; set; } 
    public string? Address { get; set; } 
    public List<Employee> Employees { get; set; }
    public bool IsActive { get; set; }
} 