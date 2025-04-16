using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserGuard_API.Migrations
{
    /// <inheritdoc />
    public partial class AddRelashinBetweenBranchAndMainten : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BranchId",
                table: "MaintenanceTasks",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
              "UPDATE MaintenanceTasks SET BranchId = '2' WHERE BranchId = ''");
 

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceTasks_BranchId",
                table: "MaintenanceTasks",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceTasks_Branches_BranchId",
                table: "MaintenanceTasks",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceTasks_Branches_BranchId",
                table: "MaintenanceTasks");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceTasks_BranchId",
                table: "MaintenanceTasks");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "MaintenanceTasks");
        }
    }
}
