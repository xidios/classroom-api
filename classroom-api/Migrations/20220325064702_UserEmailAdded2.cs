using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace classroom_api.Migrations
{
    public partial class UserEmailAdded2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailModel_Users_UserId",
                table: "EmailModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailModel",
                table: "EmailModel");

            migrationBuilder.RenameTable(
                name: "EmailModel",
                newName: "Emails");

            migrationBuilder.RenameIndex(
                name: "IX_EmailModel_UserId",
                table: "Emails",
                newName: "IX_Emails_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Emails",
                table: "Emails",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Emails_Users_UserId",
                table: "Emails",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emails_Users_UserId",
                table: "Emails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Emails",
                table: "Emails");

            migrationBuilder.RenameTable(
                name: "Emails",
                newName: "EmailModel");

            migrationBuilder.RenameIndex(
                name: "IX_Emails_UserId",
                table: "EmailModel",
                newName: "IX_EmailModel_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailModel",
                table: "EmailModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailModel_Users_UserId",
                table: "EmailModel",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
