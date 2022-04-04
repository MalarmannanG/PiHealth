using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class ProcdueMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProcedureMasterId",
                table: "PatientProcedure",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProcedureMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Diagnosis = table.Column<string>(nullable: true),
                    Date = table.Column<string>(nullable: true),
                    Procedurename = table.Column<string>(nullable: true),
                    Anesthesia = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Complication = table.Column<string>(nullable: true),
                    Others = table.Column<string>(nullable: true),
                    ActualCost = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcedureMaster", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientProcedure_ProcedureMasterId",
                table: "PatientProcedure",
                column: "ProcedureMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientProcedure_ProcedureMaster_ProcedureMasterId",
                table: "PatientProcedure",
                column: "ProcedureMasterId",
                principalTable: "ProcedureMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientProcedure_ProcedureMaster_ProcedureMasterId",
                table: "PatientProcedure");

            migrationBuilder.DropTable(
                name: "ProcedureMaster");

            migrationBuilder.DropIndex(
                name: "IX_PatientProcedure_ProcedureMasterId",
                table: "PatientProcedure");

            migrationBuilder.DropColumn(
                name: "ProcedureMasterId",
                table: "PatientProcedure");
        }
    }
}
