using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class prescriptiontableupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sos",
                table: "PatientProfile");

            migrationBuilder.DropColumn(
                name: "Stat",
                table: "PatientProfile");

            migrationBuilder.AddColumn<bool>(
                name: "Sos",
                table: "Prescription",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Stat",
                table: "Prescription",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sos",
                table: "Prescription");

            migrationBuilder.DropColumn(
                name: "Stat",
                table: "Prescription");

            migrationBuilder.AddColumn<bool>(
                name: "Sos",
                table: "PatientProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Stat",
                table: "PatientProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
