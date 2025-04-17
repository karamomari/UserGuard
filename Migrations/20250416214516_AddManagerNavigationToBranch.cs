using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserGuard_API.Migrations
{
    /// <inheritdoc />
    public partial class AddManagerNavigationToBranch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ManagerId",
                table: "Branches",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branches_ManagerId",
                table: "Branches",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Employees_ManagerId",
                table: "Branches",
                column: "ManagerId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Employees_ManagerId",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Branches_ManagerId",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Branches");
        }
    }
}
