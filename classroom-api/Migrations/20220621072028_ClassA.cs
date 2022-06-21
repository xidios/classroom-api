using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace classroom_api.Migrations
{
    public partial class ClassA : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseModelCourseUserModel");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseUserId",
                table: "Invitations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_CourseUserId",
                table: "Invitations",
                column: "CourseUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_CourseUsers_CourseUserId",
                table: "Invitations",
                column: "CourseUserId",
                principalTable: "CourseUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_CourseUsers_CourseUserId",
                table: "Invitations");

            migrationBuilder.DropIndex(
                name: "IX_Invitations_CourseUserId",
                table: "Invitations");

            migrationBuilder.DropColumn(
                name: "CourseUserId",
                table: "Invitations");

            migrationBuilder.CreateTable(
                name: "CourseModelCourseUserModel",
                columns: table => new
                {
                    CourseUsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoursesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseModelCourseUserModel", x => new { x.CourseUsersId, x.CoursesId });
                    table.ForeignKey(
                        name: "FK_CourseModelCourseUserModel_Courses_CoursesId",
                        column: x => x.CoursesId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseModelCourseUserModel_CourseUsers_CourseUsersId",
                        column: x => x.CourseUsersId,
                        principalTable: "CourseUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseModelCourseUserModel_CoursesId",
                table: "CourseModelCourseUserModel",
                column: "CoursesId");
        }
    }
}
