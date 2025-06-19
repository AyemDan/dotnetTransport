using Microsoft.EntityFrameworkCore;
using Transport.Domain.Interfaces;
using Transport.Domain.Entities;
using Transport.Infrastructure.Data;

namespace Transport.Infrastructure.Repositories;

public class StudentRepository : UserRepository, IStudentRepository
{
    public StudentRepository(PostgresDbContext context) : base(context) {}

    public async Task<List<Student>> GetAllStudentsAsync()
    {
        return await _context.Set<Student>().ToListAsync();
    }

    // public async Task<List<Student>> GetStudentsWithPreferencesAsync()
    // {
    //     return await _context.Set<Student>().Include(s => s.Preferences).ToListAsync();
    // }
}
