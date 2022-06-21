using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace classroom_api.Migrations
{
    public partial class AddedSubdivisionToCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Subdivisions_SubdivisionModelId",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "SubdivisionModelId",
                table: "Courses",
                newName: "SubdivisionId");

            migrationBuilder.RenameIndex(
                name: "IX_Courses_SubdivisionModelId",
                table: "Courses",
                newName: "IX_Courses_SubdivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Subdivisions_SubdivisionId",
                table: "Courses",
                column: "SubdivisionId",
                principalTable: "Subdivisions",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Subdivisions_SubdivisionId",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "SubdivisionId",
                table: "Courses",
                newName: "SubdivisionModelId");

            migrationBuilder.RenameIndex(
                name: "IX_Courses_SubdivisionId",
                table: "Courses",
                newName: "IX_Courses_SubdivisionModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Subdivisions_SubdivisionModelId",
                table: "Courses",
                column: "SubdivisionModelId",
                principalTable: "Subdivisions",
                principalColumn: "Id");
        }
    }
}
