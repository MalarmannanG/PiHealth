using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class PatientProfileDataMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatientProfileDataMapping",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientProfileId = table.Column<long>(nullable: false),
                    PatientProfileDataId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientProfileDataMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientProfileDataMapping_PatientProfileData_PatientProfileDataId",
                        column: x => x.PatientProfileDataId,
                        principalTable: "PatientProfileData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientProfileDataMapping_PatientProfile_PatientProfileId",
                        column: x => x.PatientProfileId,
                        principalTable: "PatientProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientProfileDataMapping_PatientProfileDataId",
                table: "PatientProfileDataMapping",
                column: "PatientProfileDataId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientProfileDataMapping_PatientProfileId",
                table: "PatientProfileDataMapping",
                column: "PatientProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientProfileDataMapping");
        }
    }
}
