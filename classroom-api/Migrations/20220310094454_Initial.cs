using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace classroom_api.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StatisticModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherInvitationCount = table.Column<int>(type: "integer", nullable: false),
                    StudentsInvitationCount = table.Column<int>(type: "integer", nullable: false),
                    ClassesCreatedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatisticModels", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatisticModels");
        }
    }
}
