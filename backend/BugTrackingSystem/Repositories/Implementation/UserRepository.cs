using BugTrackingSystem.Data;
using BugTrackingSystem.Entities;
using BugTrackingSystem.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTrackingSystem.Repositories.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .OrderBy(u => u.UserId)
                .ToListAsync();
        }

        public async Task<bool> EmailExistsAsync(
            string email,
            int? excludeUserId = null)
        {
            return await _context.Users.AnyAsync(u =>
                u.Email == email &&
                (!excludeUserId.HasValue ||
                 u.UserId != excludeUserId.Value));
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}