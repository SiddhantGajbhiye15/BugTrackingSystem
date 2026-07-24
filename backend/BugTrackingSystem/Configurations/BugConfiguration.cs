using BugTrackingSystem.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BugTrackingSystem.Configurations
{
    public class BugConfiguration : IEntityTypeConfiguration<Bug>
    {
        public void Configure(EntityTypeBuilder<Bug> builder)
        {
            // Primary Key
            builder.HasKey(b => b.BugId);

            // Properties
            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(b => b.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(b => b.Priority)
                .IsRequired();

            builder.Property(b => b.Status)
                .IsRequired();

            builder.Property(b => b.CreatedAt)
                .IsRequired();

            // Relationships

            builder.HasOne(b => b.Project)
                .WithMany(p => p.Bugs)
                .HasForeignKey(b => b.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(b => b.ReportedByUser)
                .WithMany(u => u.ReportedBugs)
                .HasForeignKey(b => b.ReportedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.AssignedDeveloper)
                .WithMany(u => u.AssignedBugs)
                .HasForeignKey(b => b.AssignedDeveloperId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.Comments)
                .WithOne(c => c.Bug)
                .HasForeignKey(c => c.BugId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}