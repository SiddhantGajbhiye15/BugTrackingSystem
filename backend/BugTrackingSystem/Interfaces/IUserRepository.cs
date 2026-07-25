using BugTrackingSystem.Entities;

namespace BugTrackingSystem.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByIdAsync(int userId);

        Task<List<User>> GetAllAsync();

        Task<bool> EmailExistsAsync(
            string email,
            int? excludeUserId = null
        );

        Task AddAsync(User user);

        Task SaveChangesAsync();
    }
}