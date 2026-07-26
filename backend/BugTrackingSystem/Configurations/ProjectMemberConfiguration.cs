using BugTrackingSystem.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BugTrackingSystem.Configurations
{
    public class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
    {
        public void Configure(EntityTypeBuilder<ProjectMember> builder)
        {
            // Primary Key
            builder.HasKey(pm => pm.ProjectMemberId);

            // Properties
            builder.Property(pm => pm.JoinedDate)
                .IsRequired();

            // Prevent duplicate membership
            builder.HasIndex(pm => new { pm.ProjectId, pm.UserId })
                .IsUnique();
            builder.HasIndex(pm => pm.UserId)
                .IsUnique()
                .HasFilter("[RemovedDate] IS NULL");

            // Relationships

            builder.HasOne(pm => pm.Project)
                .WithMany(p => p.ProjectMembers)
                .HasForeignKey(pm => pm.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pm => pm.User)
                .WithMany(u => u.ProjectMemberships)
                .HasForeignKey(pm => pm.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}