using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class tablechanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ActualCost",
                table: "ProcedureMaster",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "ProcedureMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ProcedureMaster",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProcedureMaster",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ModifiedBy",
                table: "ProcedureMaster",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "ProcedureMaster",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ProcedureMaster");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ProcedureMaster");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProcedureMaster");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "ProcedureMaster");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "ProcedureMaster");

            migrationBuilder.AlterColumn<double>(
                name: "ActualCost",
                table: "ProcedureMaster",
                type: "float",
                nullable: true,
                oldClrType: typeof(long),
                oldNullable: true);
        }
    }
}
