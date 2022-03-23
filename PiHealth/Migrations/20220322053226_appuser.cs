using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class appuser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUser_Specialization_SpecializationId",
                table: "AppUser");

            migrationBuilder.DropIndex(
                name: "IX_AppUser_SpecializationId",
                table: "AppUser");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AppUser_SpecializationId",
                table: "AppUser",
                column: "SpecializationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUser_Specialization_SpecializationId",
                table: "AppUser",
                column: "SpecializationId",
                principalTable: "Specialization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
