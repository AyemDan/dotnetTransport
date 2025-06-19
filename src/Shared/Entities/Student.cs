using Transport.Shared.Enums;

namespace Transport.Shared.Entities;

public class Student : User
{
    public Student() { }
    public Student(string email, string password) : base(email, password)
    {
        Role = UserRole.Student;
    }

    public string? ParentName { get; set; }
    public string? ParentPhone { get; set; }
    public string? EmergencyContact { get; set; }
    public string? EmergencyPhone { get; set; }
    public string? Grade { get; set; }
    public string? School { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? BloodGroup { get; set; }
    public string? Allergies { get; set; }
    public string? MedicalConditions { get; set; }
    public string? SpecialNeeds { get; set; }
    public string? Notes { get; set; }
} 