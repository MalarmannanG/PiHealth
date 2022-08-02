using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class Prescription_Master_Mapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PrescriptionMasterId",
                table: "PrescriptionMaster",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PrescriptionMasterId",
                table: "Prescription",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_PrescriptionMasterId",
                table: "Prescription",
                column: "PrescriptionMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prescription_PrescriptionMaster_PrescriptionMasterId",
                table: "Prescription",
                column: "PrescriptionMasterId",
                principalTable: "PrescriptionMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prescription_PrescriptionMaster_PrescriptionMasterId",
                table: "Prescription");

            migrationBuilder.DropIndex(
                name: "IX_Prescription_PrescriptionMasterId",
                table: "Prescription");

            migrationBuilder.DropColumn(
                name: "PrescriptionMasterId",
                table: "PrescriptionMaster");

            migrationBuilder.DropColumn(
                name: "PrescriptionMasterId",
                table: "Prescription");
        }
    }
}
