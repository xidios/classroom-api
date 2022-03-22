using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace classroom_api.Migrations
{
    public partial class SubdivisionAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SubdivisionModelId",
                table: "Courses",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Subdivisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subdivisions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_SubdivisionModelId",
                table: "Courses",
                column: "SubdivisionModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Subdivisions_SubdivisionModelId",
                table: "Courses",
                column: "SubdivisionModelId",
                principalTable: "Subdivisions",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Subdivisions_SubdivisionModelId",
                table: "Courses");

            migrationBuilder.DropTable(
                name: "Subdivisions");

            migrationBuilder.DropIndex(
                name: "IX_Courses_SubdivisionModelId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "SubdivisionModelId",
                table: "Courses");
        }
    }
}
