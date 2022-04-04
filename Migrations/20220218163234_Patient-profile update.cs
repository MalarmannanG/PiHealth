using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class Patientprofileupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Prescription");

            migrationBuilder.DropColumn(
                name: "MedicinName",
                table: "Prescription");

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "Prescription",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MedicineName",
                table: "Prescription",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "Prescription",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Units",
                table: "Prescription",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "Prescription");

            migrationBuilder.DropColumn(
                name: "MedicineName",
                table: "Prescription");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "Prescription");

            migrationBuilder.DropColumn(
                name: "Units",
                table: "Prescription");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Prescription",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MedicinName",
                table: "Prescription",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
