using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class Updated_PrescriptionMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BeforeFood",
                table: "PrescriptionMaster",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Morning",
                table: "PrescriptionMaster",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Night",
                table: "PrescriptionMaster",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NoOfDays",
                table: "PrescriptionMaster",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Noon",
                table: "PrescriptionMaster",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Sos",
                table: "PrescriptionMaster",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Stat",
                table: "PrescriptionMaster",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BeforeFood",
                table: "PrescriptionMaster");

            migrationBuilder.DropColumn(
                name: "Morning",
                table: "PrescriptionMaster");

            migrationBuilder.DropColumn(
                name: "Night",
                table: "PrescriptionMaster");

            migrationBuilder.DropColumn(
                name: "NoOfDays",
                table: "PrescriptionMaster");

            migrationBuilder.DropColumn(
                name: "Noon",
                table: "PrescriptionMaster");

            migrationBuilder.DropColumn(
                name: "Sos",
                table: "PrescriptionMaster");

            migrationBuilder.DropColumn(
                name: "Stat",
                table: "PrescriptionMaster");
        }
    }
}
