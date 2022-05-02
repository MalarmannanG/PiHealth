using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class DoctorServiceIdChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<long>(
                name: "DoctorServiceId",
                table: "PatientProfile",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientProfile_DoctorServiceId",
                table: "PatientProfile",
                column: "DoctorServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientProfile_DoctorService_DoctorServiceId",
                table: "PatientProfile",
                column: "DoctorServiceId",
                principalTable: "DoctorService",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientProfile_DoctorService_DoctorServiceId",
                table: "PatientProfile");

            migrationBuilder.DropIndex(
                name: "IX_PatientProfile_DoctorServiceId",
                table: "PatientProfile");

            migrationBuilder.DropColumn(
                name: "DoctorServiceId",
                table: "PatientProfile");

            migrationBuilder.AddColumn<long>(
                name: "ServiceId",
                table: "PatientProfile",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ServiveId",
                table: "PatientProfile",
                type: "bigint",
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
    }
}
