using BugTrackingSystem.Configurations;
using BugTrackingSystem.Entities;
using Microsoft.EntityFrameworkCore;
namespace BugTrackingSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options) {
        }
        
        public DbSet<User> Users { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<ProjectMember> ProjectMembers { get; set; }

        public DbSet<Bug> Bugs { get; set; }

        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectMemberConfiguration());
            modelBuilder.ApplyConfiguration(new BugConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
        }

    }

}
