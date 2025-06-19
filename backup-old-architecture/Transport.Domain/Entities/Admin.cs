using Transport.Domain.Enums;

namespace Transport.Domain.Entities;

public class Admin : User
{
    public Admin(string fullName, string email, string password, AdminLevel level) : base(email, password)
    {
        Level = level;
    }

    public AdminLevel Level { get; private set; }  // Prevents external modification

    public void Promote(AdminLevel newLevel)
    {
        // Only allow promotion if needed
        if (newLevel > Level)
        {
            Level = newLevel;
        }
    }
}
