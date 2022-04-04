using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class ProcedureMasterId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ActualCost",
                table: "ProcedureMaster",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProcedureMasterId",
                table: "PatientProfile",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcedureMasterId",
                table: "PatientProfile");

            migrationBuilder.AlterColumn<long>(
                name: "ActualCost",
                table: "ProcedureMaster",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long));
        }
    }
}
