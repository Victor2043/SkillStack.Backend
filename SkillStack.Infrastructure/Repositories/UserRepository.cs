
using Microsoft.EntityFrameworkCore;
using SkillStack.Application.Interfaces;
using SkillStack.Domain.Entities;
using SkillStack.Infrastructure.Persistence;

namespace SkillStack.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToUpper() == email.ToUpper());
    }
    
}
