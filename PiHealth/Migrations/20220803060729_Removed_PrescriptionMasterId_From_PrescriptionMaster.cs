using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class Removed_PrescriptionMasterId_From_PrescriptionMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrescriptionMasterId",
                table: "PrescriptionMaster");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PrescriptionMasterId",
                table: "PrescriptionMaster",
                type: "bigint",
                nullable: true);
        }
    }
}
