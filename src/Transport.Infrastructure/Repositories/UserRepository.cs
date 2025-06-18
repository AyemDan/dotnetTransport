using Microsoft.EntityFrameworkCore;
using Transport.Domain.Entities;
using Transport.Domain.Interfaces;
using Transport.Infrastructure.Data;

namespace Transport.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    protected readonly PostgresDbContext _context;

    public UserRepository(PostgresDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _context.Set<User>().FindAsync(userId);
    }

public async Task<User?> GetUserByEmailAsync(string email)
{
    var student = await _context.Set<Student>().FirstOrDefaultAsync(u => u.Email == email);
    if (student != null) return student;

    var provider = await _context.Set<Provider>().FirstOrDefaultAsync(u => u.Email == email);
    if (provider != null) return provider;

    return null; // No user found
}



    public async Task AddUserAsync(User user)
    {
        _context.Set<User>().Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Set<User>().Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        var user = await _context.Set<User>().FindAsync(userId);
        if (user != null)
        {
            _context.Set<User>().Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
