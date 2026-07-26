using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTrackingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueActiveProjectMembershipIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProjectMembers_UserId",
                table: "ProjectMembers");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMembers_UserId",
                table: "ProjectMembers",
                column: "UserId",
                unique: true,
                filter: "[RemovedDate] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProjectMembers_UserId",
                table: "ProjectMembers");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMembers_UserId",
                table: "ProjectMembers",
                column: "UserId");
        }
    }
}
