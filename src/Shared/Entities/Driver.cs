using Transport.Shared.Enums;

namespace Transport.Shared.Entities;

public class Driver : User
{
    public Driver() { }

    public Driver(string email, string password)
        : base(email, password)
    {
        Role = UserRole.Driver;
        IsVerified = false;
    }

    public bool IsVerified { get; set; } = false;
    public List<Guid> AssignedTripIds { get; set; } = new();
    public string? VehicleInfo { get; set; }
    // Add more driver-specific fields as needed
}
