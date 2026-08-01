using BugTrackingSystem.Entities;
using BugTrackingSystem.Enums;
using BugTrackingSystem.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BugTrackingSystem.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAdminAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            var configuration = scope.ServiceProvider
                .GetRequiredService<IConfiguration>();

            var logger = scope.ServiceProvider
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("DatabaseSeeder");

            var email = configuration["AdminSeed:Email"]
                ?.Trim()
                .ToLowerInvariant();

            var password = configuration["AdminSeed:Password"];
            var firstName = configuration["AdminSeed:FirstName"]?.Trim();
            var lastName = configuration["AdminSeed:LastName"]?.Trim();

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName))
            {
                logger.LogWarning(
                    "Admin seed configuration is missing. Admin was not created.");

                return;
            }

            var adminExists = await dbContext.Users
                .AnyAsync(user => user.Role == UserRole.Admin);

            if (adminExists)
            {
                logger.LogInformation(
                    "Admin already exists. Admin seeding skipped.");

                return;
            }

            var emailExists = await dbContext.Users
                .AnyAsync(user => user.Email == email);

            if (emailExists)
            {
                throw new InvalidOperationException(
                    "The configured Admin email is already being used.");
            }

            var admin = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = PasswordHasher.HashPassword(password),
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            dbContext.Users.Add(admin);
            await dbContext.SaveChangesAsync();

            logger.LogInformation(
                "Initial Admin account created successfully.");
        }
    }
}