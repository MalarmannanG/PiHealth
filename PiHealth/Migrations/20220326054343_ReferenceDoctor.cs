using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class ReferenceDoctor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferredBy",
                table: "PatientProfile");

            migrationBuilder.AddColumn<string>(
                name: "ReferredDoctor",
                table: "PatientProfile",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferredDoctor",
                table: "PatientProfile");

            migrationBuilder.AddColumn<string>(
                name: "ReferredBy",
                table: "PatientProfile",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
