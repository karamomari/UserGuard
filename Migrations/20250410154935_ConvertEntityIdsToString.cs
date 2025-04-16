using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserGuard_API.Migrations
{
    /// <inheritdoc />
    public partial class ConvertEntityIdsToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql("ALTER TABLE [Schedules] DROP CONSTRAINT [PK_Schedules]");

            // ثم حذف العمود القديم
            migrationBuilder.DropColumn(name: "Id", table: "Schedules");

            // إضافة العمود الجديد مع نوع البيانات الجديد
            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Schedules",
                type: "nvarchar(450)",
                nullable: false);



            migrationBuilder.Sql("ALTER TABLE [MaintenanceTasks] DROP CONSTRAINT [PK_MaintenanceTasks]");

            // ثم حذف العمود القديم
            migrationBuilder.DropColumn(name: "Id", table: "MaintenanceTasks");

            // إضافة العمود الجديد مع نوع البيانات الجديد
            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "MaintenanceTasks",
                type: "nvarchar(450)",
                nullable: false);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // استرجاع العمود القديم إذا أردت التراجع عن التغيير
          
        }
    }

}
