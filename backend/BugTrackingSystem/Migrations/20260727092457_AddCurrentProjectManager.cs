using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTrackingSystem.Migrations
{
    public partial class AddCurrentProjectManager : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectManagerId",
                table: "Projects",
                type: "int",
                nullable: true);

            // Give every existing project its creator
            // as the initial current Project Manager.
            migrationBuilder.Sql(
                """
                UPDATE Projects
                SET ProjectManagerId = CreatedBy
                WHERE ProjectManagerId IS NULL
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectManagerId",
                table: "Projects",
                column: "ProjectManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Users_ProjectManagerId",
                table: "Projects",
                column: "ProjectManagerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_ProjectManagerId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectManagerId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectManagerId",
                table: "Projects");
        }
    }
}