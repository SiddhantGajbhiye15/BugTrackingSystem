using BugTrackingSystem.Entities;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetByIdAsync(int userId);

    Task<List<User>> GetAllAsync();

    Task<bool> EmailExistsAsync(string email);

    Task AddAsync(User user);

    Task SaveChangesAsync();
}