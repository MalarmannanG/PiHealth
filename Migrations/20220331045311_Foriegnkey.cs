using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class Foriegnkey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientProcedure_DoctorMaster_DoctorMasterId",
                table: "PatientProcedure");

            migrationBuilder.DropIndex(
                name: "IX_PatientProcedure_DoctorMasterId",
                table: "PatientProcedure");

            migrationBuilder.CreateIndex(
                name: "IX_PatientProcedure_PatientProfileId",
                table: "PatientProcedure",
                column: "PatientProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientProcedure_PatientProfile_PatientProfileId",
                table: "PatientProcedure",
                column: "PatientProfileId",
                principalTable: "PatientProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientProcedure_PatientProfile_PatientProfileId",
                table: "PatientProcedure");

            migrationBuilder.DropIndex(
                name: "IX_PatientProcedure_PatientProfileId",
                table: "PatientProcedure");

            migrationBuilder.CreateIndex(
                name: "IX_PatientProcedure_DoctorMasterId",
                table: "PatientProcedure",
                column: "DoctorMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientProcedure_DoctorMaster_DoctorMasterId",
                table: "PatientProcedure",
                column: "DoctorMasterId",
                principalTable: "DoctorMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
