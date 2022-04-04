using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class referredbycolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUser_Specialization_SpecializationId",
                table: "AppUser");

            migrationBuilder.DropIndex(
                name: "IX_AppUser_SpecializationId",
                table: "AppUser");

            migrationBuilder.AlterColumn<long>(
                name: "SpecializationId",
                table: "AppUser",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "ReferredBy",
                table: "Appointment",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUser_SpecializationId",
                table: "AppUser",
                column: "SpecializationId",
                unique: true,
                filter: "[SpecializationId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUser_Specialization_SpecializationId",
                table: "AppUser",
                column: "SpecializationId",
                principalTable: "Specialization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUser_Specialization_SpecializationId",
                table: "AppUser");

            migrationBuilder.DropIndex(
                name: "IX_AppUser_SpecializationId",
                table: "AppUser");

            migrationBuilder.DropColumn(
                name: "ReferredBy",
                table: "Appointment");

            migrationBuilder.AlterColumn<long>(
                name: "SpecializationId",
                table: "AppUser",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUser_SpecializationId",
                table: "AppUser",
                column: "SpecializationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUser_Specialization_SpecializationId",
                table: "AppUser",
                column: "SpecializationId",
                principalTable: "Specialization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
