using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class PatientProfiletableupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvestigationResults",
                table: "PatientProfile",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PastHistory",
                table: "PatientProfile",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Sos",
                table: "PatientProfile",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Stat",
                table: "PatientProfile",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvestigationResults",
                table: "PatientProfile");

            migrationBuilder.DropColumn(
                name: "PastHistory",
                table: "PatientProfile");

            migrationBuilder.DropColumn(
                name: "Sos",
                table: "PatientProfile");

            migrationBuilder.DropColumn(
                name: "Stat",
                table: "PatientProfile");
        }
    }
}
