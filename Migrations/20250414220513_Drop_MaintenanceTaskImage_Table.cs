using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserGuard_API.Migrations
{
    /// <inheritdoc />
    public partial class Drop_MaintenanceTaskImage_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
        name: "MaintenanceTaskImage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceTaskImage_MaintenanceTasks_MaintenanceTaskId1",
                table: "MaintenanceTaskImage");

            migrationBuilder.AlterColumn<string>(
                name: "MaintenanceTaskId1",
                table: "MaintenanceTaskImage",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "MaintenanceTaskImage",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceTaskImage_MaintenanceTasks_MaintenanceTaskId1",
                table: "MaintenanceTaskImage",
                column: "MaintenanceTaskId1",
                principalTable: "MaintenanceTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
