using BugTrackingSystem.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BugTrackingSystem.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            // Primary Key
            builder.HasKey(c => c.CommentId);

            // Properties
            builder.Property(c => c.CommentText)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(c => c.IsEdited)
                .HasDefaultValue(false);

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            // Relationships

            builder.HasOne(c => c.Bug)
                .WithMany(b => b.Comments)
                .HasForeignKey(c => c.BugId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}