using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace classroom_api.Migrations
{
    public partial class UserModelAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubdivisionModelUserModel",
                columns: table => new
                {
                    ModeratorsId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubdivisionsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubdivisionModelUserModel", x => new { x.ModeratorsId, x.SubdivisionsId });
                    table.ForeignKey(
                        name: "FK_SubdivisionModelUserModel_Subdivisions_SubdivisionsId",
                        column: x => x.SubdivisionsId,
                        principalTable: "Subdivisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubdivisionModelUserModel_Users_ModeratorsId",
                        column: x => x.ModeratorsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubdivisionModelUserModel_SubdivisionsId",
                table: "SubdivisionModelUserModel",
                column: "SubdivisionsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubdivisionModelUserModel");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
