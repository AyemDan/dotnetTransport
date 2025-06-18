using Transport.Domain.Enums;

namespace Transport.Domain.Entities
{
    public class Student : User
    {
        public Student() { } // Parameterless constructor

        public Student(string email, string password)
            : base(email, password)
        {
            Role = UserRole.Student;
        }

        public string Institution { get; private set; } = string.Empty;
        public string MatricNo { get; private set; } = string.Empty;
        public string College { get; private set; } = string.Empty;
        public string Department { get; private set; } = string.Empty;
    }
}
