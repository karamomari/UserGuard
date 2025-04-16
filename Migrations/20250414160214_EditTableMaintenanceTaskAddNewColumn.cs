using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserGuard_API.Migrations
{
    /// <inheritdoc />
    public partial class EditTableMaintenanceTaskAddNewColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "MaintenanceTasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "MaintenanceTasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "MaintenanceTasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "TaskEnd",
                table: "MaintenanceTasks",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "MaintenanceTasks");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "MaintenanceTasks");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MaintenanceTasks");

            migrationBuilder.DropColumn(
                name: "TaskEnd",
                table: "MaintenanceTasks");
        }
    }
}
