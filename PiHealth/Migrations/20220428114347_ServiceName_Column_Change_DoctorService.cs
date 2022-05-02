using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class ServiceName_Column_Change_DoctorService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServieName",
                table: "DoctorService");

            migrationBuilder.AddColumn<string>(
                name: "ServiceName",
                table: "DoctorService",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceName",
                table: "DoctorService");

            migrationBuilder.AddColumn<string>(
                name: "ServieName",
                table: "DoctorService",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
