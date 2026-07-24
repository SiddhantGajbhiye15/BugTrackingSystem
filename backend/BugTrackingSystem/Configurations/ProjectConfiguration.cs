using BugTrackingSystem.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BugTrackingSystem.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            // Primary Key for project 
            builder.HasKey(p => p.ProjectId);

            // Properties
            builder.Property(p => p.ProjectCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(p => p.ProjectCode)
                .IsUnique();

            builder.Property(p => p.ProjectName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .HasMaxLength(1000);

            builder.Property(p => p.Status)
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            // Relationships

            builder.HasOne(p => p.CreatedByUser)
                .WithMany(u => u.CreatedProjects)
                .HasForeignKey(p => p.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.ProjectMembers)
                .WithOne(pm => pm.Project)
                .HasForeignKey(pm => pm.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Bugs)
                .WithOne(b => b.Project)
                .HasForeignKey(b => b.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}