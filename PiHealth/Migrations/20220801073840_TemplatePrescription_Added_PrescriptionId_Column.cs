using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class TemplatePrescription_Added_PrescriptionId_Column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PrescriptionMasterId",
                table: "TemplatePrescription",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TemplatePrescription_PrescriptionMasterId",
                table: "TemplatePrescription",
                column: "PrescriptionMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_TemplatePrescription_PrescriptionMaster_PrescriptionMasterId",
                table: "TemplatePrescription",
                column: "PrescriptionMasterId",
                principalTable: "PrescriptionMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TemplatePrescription_PrescriptionMaster_PrescriptionMasterId",
                table: "TemplatePrescription");

            migrationBuilder.DropIndex(
                name: "IX_TemplatePrescription_PrescriptionMasterId",
                table: "TemplatePrescription");

            migrationBuilder.DropColumn(
                name: "PrescriptionMasterId",
                table: "TemplatePrescription");
        }
    }
}
