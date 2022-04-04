using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class RemovedDoctorForeignKeyfromProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientProcedure_PatientProfile_PatientProfileId",
                table: "PatientProcedure");

            migrationBuilder.DropIndex(
                name: "IX_PatientProcedure_PatientProfileId",
                table: "PatientProcedure");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
