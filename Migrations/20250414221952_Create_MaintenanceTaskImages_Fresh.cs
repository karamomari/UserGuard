using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserGuard_API.Migrations
{
    /// <inheritdoc />
    public partial class Create_MaintenanceTaskImages_Fresh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
           name: "MaintenanceTaskImages",
           columns: table => new
           {
               Id = table.Column<string>(nullable: false),
               MaintenanceTaskId = table.Column<string>(nullable: false),
               ImageUrl = table.Column<string>(nullable: true)
           },
           constraints: table =>
           {
               table.PrimaryKey("PK_MaintenanceTaskImages", x => x.Id);
               table.ForeignKey(
                   name: "FK_MaintenanceTaskImages_MaintenanceTasks_MaintenanceTaskId",
                   column: x => x.MaintenanceTaskId,
                   principalTable: "MaintenanceTasks",
                   principalColumn: "Id",
                   onDelete: ReferentialAction.Cascade);
           });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceTaskImages_MaintenanceTaskId",
                table: "MaintenanceTaskImages",
                column: "MaintenanceTaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
