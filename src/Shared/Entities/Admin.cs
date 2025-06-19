using Transport.Shared.Enums;

namespace Transport.Shared.Entities;

public class Admin : User
{
    public Admin() { }
    public Admin(string email, string password, AdminLevel level) : base(email, password)
    {
        Role = UserRole.Admin;
        Level = level;
    }

    public AdminLevel Level { get; set; }
} 