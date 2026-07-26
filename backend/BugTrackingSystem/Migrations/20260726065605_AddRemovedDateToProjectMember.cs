using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTrackingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddRemovedDateToProjectMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RemovedDate",
                table: "ProjectMembers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemovedDate",
                table: "ProjectMembers");
        }
    }
}
