using Microsoft.EntityFrameworkCore.Migrations;

namespace PiHealth.Migrations
{
    public partial class Added_TemplateMasterDataMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TemplateMasterDataMapping",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateMasterId = table.Column<long>(nullable: false),
                    PatientProfileDataId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateMasterDataMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateMasterDataMapping_PatientProfileData_PatientProfileDataId",
                        column: x => x.PatientProfileDataId,
                        principalTable: "PatientProfileData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TemplateMasterDataMapping_TemplateMaster_TemplateMasterId",
                        column: x => x.TemplateMasterId,
                        principalTable: "TemplateMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TemplateMasterDataMapping_PatientProfileDataId",
                table: "TemplateMasterDataMapping",
                column: "PatientProfileDataId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateMasterDataMapping_TemplateMasterId",
                table: "TemplateMasterDataMapping",
                column: "TemplateMasterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemplateMasterDataMapping");
        }
    }
}
