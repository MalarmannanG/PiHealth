using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class appuser_newcolumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RegistrationNo",
                table: "AppUser",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpecializationId",
                table: "AppUser",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SpecializationId1",
                table: "AppUser",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUser_SpecializationId1",
                table: "AppUser",
                column: "SpecializationId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUser_Specialization_SpecializationId1",
                table: "AppUser",
                column: "SpecializationId1",
                principalTable: "Specialization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUser_Specialization_SpecializationId1",
                table: "AppUser");

            migrationBuilder.DropIndex(
                name: "IX_AppUser_SpecializationId1",
                table: "AppUser");

            migrationBuilder.DropColumn(
                name: "RegistrationNo",
                table: "AppUser");

            migrationBuilder.DropColumn(
                name: "SpecializationId",
                table: "AppUser");

            migrationBuilder.DropColumn(
                name: "SpecializationId1",
                table: "AppUser");
        }
    }
}
