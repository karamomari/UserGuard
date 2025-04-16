using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserGuard_API.Migrations
{
    /// <inheritdoc />
    public partial class ReturnPrimaryKeyToSchAndMain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddPrimaryKey(
           name: "PK_Schedules",
           table: "Schedules",
           column: "Id");

            migrationBuilder.AddPrimaryKey(
            name: "PK_MaintenanceTasks",
            table: "MaintenanceTasks",
            column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
