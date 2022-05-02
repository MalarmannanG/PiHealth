using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class DoctorServiceToPatientProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ServiceId",
                table: "PatientProfile",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ServiveId",
                table: "PatientProfile",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientProfile_ServiceId",
                table: "PatientProfile",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientProfile_DoctorService_ServiceId",
                table: "PatientProfile",
                column: "ServiceId",
                principalTable: "DoctorService",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientProfile_DoctorService_ServiceId",
                table: "PatientProfile");

            migrationBuilder.DropIndex(
                name: "IX_PatientProfile_ServiceId",
                table: "PatientProfile");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "PatientProfile");

            migrationBuilder.DropColumn(
                name: "ServiveId",
                table: "PatientProfile");
        }
    }
}
