using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class procedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatientProcedure",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Diagnosis = table.Column<string>(nullable: true),
                    Date = table.Column<string>(nullable: true),
                    Procedurename = table.Column<string>(nullable: true),
                    DoctorMasterId = table.Column<long>(nullable: true),
                    Anesthesia = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Complication = table.Column<string>(nullable: true),
                    Others = table.Column<string>(nullable: true),
                    ActualCost = table.Column<double>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<long>(nullable: false),
                    ModifiedBy = table.Column<long>(nullable: true),
                    PatientProfileId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientProcedure", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientProcedure_DoctorMaster_DoctorMasterId",
                        column: x => x.DoctorMasterId,
                        principalTable: "DoctorMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientProcedure_PatientProfile_PatientProfileId",
                        column: x => x.PatientProfileId,
                        principalTable: "PatientProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientProcedure_DoctorMasterId",
                table: "PatientProcedure",
                column: "DoctorMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientProcedure_PatientProfileId",
                table: "PatientProcedure",
                column: "PatientProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientProcedure");
        }
    }
}
