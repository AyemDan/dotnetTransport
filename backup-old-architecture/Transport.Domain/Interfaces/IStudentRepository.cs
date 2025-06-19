using Transport.Domain.Entities;

namespace Transport.Domain.Interfaces;

public interface IStudentRepository : IUserRepository
{
    Task<List<Student>> GetAllStudentsAsync();
    // Task<List<Student>> GetStudentsWithPreferencesAsync();
}
