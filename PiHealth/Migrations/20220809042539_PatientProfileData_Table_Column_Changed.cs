using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class PatientProfileData_Table_Column_Changed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "PatientProfileData",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "PatientProfileData",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PatientProfileData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ModifiedBy",
                table: "PatientProfileData",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "PatientProfileData",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "PatientProfileData");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "PatientProfileData");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PatientProfileData");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "PatientProfileData");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "PatientProfileData");
        }
    }
}
