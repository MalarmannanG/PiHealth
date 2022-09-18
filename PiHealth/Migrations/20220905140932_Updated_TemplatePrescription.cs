using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class Updated_TemplatePrescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Sos",
                table: "TemplatePrescription",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Stat",
                table: "TemplatePrescription",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Units",
                table: "TemplatePrescription",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sos",
                table: "TemplatePrescription");

            migrationBuilder.DropColumn(
                name: "Stat",
                table: "TemplatePrescription");

            migrationBuilder.DropColumn(
                name: "Units",
                table: "TemplatePrescription");
        }
    }
}
