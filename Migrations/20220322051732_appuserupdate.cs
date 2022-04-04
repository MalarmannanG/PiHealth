using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class appuserupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUser_Specialization_SpecializationId1",
                table: "AppUser");

            migrationBuilder.DropIndex(
                name: "IX_AppUser_SpecializationId1",
                table: "AppUser");

            migrationBuilder.DropColumn(
                name: "SpecializationId1",
                table: "AppUser");

            migrationBuilder.AlterColumn<long>(
                name: "SpecializationId",
                table: "AppUser",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUser_Specialization_SpecializationId",
                table: "AppUser");

            migrationBuilder.DropIndex(
                name: "IX_AppUser_SpecializationId",
                table: "AppUser");

            migrationBuilder.AlterColumn<string>(
                name: "SpecializationId",
                table: "AppUser",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SpecializationId1",
                table: "AppUser",
                type: "bigint",
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
    }
}
