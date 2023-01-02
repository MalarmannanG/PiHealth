using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class Added_Otp_And_OtdSentDateTime_Column_In_Appuser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Otp",
                table: "AppUser",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OtpSentDateTime",
                table: "AppUser",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Otp",
                table: "AppUser");

            migrationBuilder.DropColumn(
                name: "OtpSentDateTime",
                table: "AppUser");
        }
    }
}
