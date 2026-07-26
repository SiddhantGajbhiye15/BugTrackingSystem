using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTrackingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddBugReportingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bugs_AssignedDeveloperId",
                table: "Bugs");

            migrationBuilder.DropIndex(
                name: "IX_Bugs_ProjectId",
                table: "Bugs");

            migrationBuilder.AddColumn<string>(
                name: "ActualOutput",
                table: "Bugs",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EvidenceLink",
                table: "Bugs",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExpectedOutput",
                table: "Bugs",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StepsToReproduce",
                table: "Bugs",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Bugs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Bugs_AssignedDeveloperId_Status",
                table: "Bugs",
                columns: new[] { "AssignedDeveloperId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Bugs_ProjectId_Status",
                table: "Bugs",
                columns: new[] { "ProjectId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bugs_AssignedDeveloperId_Status",
                table: "Bugs");

            migrationBuilder.DropIndex(
                name: "IX_Bugs_ProjectId_Status",
                table: "Bugs");

            migrationBuilder.DropColumn(
                name: "ActualOutput",
                table: "Bugs");

            migrationBuilder.DropColumn(
                name: "EvidenceLink",
                table: "Bugs");

            migrationBuilder.DropColumn(
                name: "ExpectedOutput",
                table: "Bugs");

            migrationBuilder.DropColumn(
                name: "StepsToReproduce",
                table: "Bugs");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Bugs");

            migrationBuilder.CreateIndex(
                name: "IX_Bugs_AssignedDeveloperId",
                table: "Bugs",
                column: "AssignedDeveloperId");

            migrationBuilder.CreateIndex(
                name: "IX_Bugs_ProjectId",
                table: "Bugs",
                column: "ProjectId");
        }
    }
}
