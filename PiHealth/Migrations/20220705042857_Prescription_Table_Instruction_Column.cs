using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class Prescription_Table_Instruction_Column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Instructions",
                table: "TemplatePrescription",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Instructions",
                table: "PrescriptionMaster",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Instructions",
                table: "Prescription",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Instructions",
                table: "TemplatePrescription");

            migrationBuilder.DropColumn(
                name: "Instructions",
                table: "PrescriptionMaster");

            migrationBuilder.DropColumn(
                name: "Instructions",
                table: "Prescription");
        }
    }
}
