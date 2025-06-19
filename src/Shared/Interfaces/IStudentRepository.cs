using Transport.Shared.Entities;

namespace Transport.Shared.Interfaces;

public interface IStudentRepository
{
    Task<Student?> GetStudentByIdAsync(Guid id);
    Task<Student?> GetStudentByEmailAsync(string email);
    Task<IEnumerable<Student>> GetAllStudentsAsync();
    Task<IEnumerable<Student>> GetStudentsByOrganizationAsync(Guid organizationId);
    Task AddStudentAsync(Student student);
    Task UpdateStudentAsync(Student student);
    Task DeleteStudentAsync(Guid id);
} 