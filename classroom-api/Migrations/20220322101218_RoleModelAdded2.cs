﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace classroom_api.Migrations
{
    public partial class RoleModelAdded2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Roles_RoleModelId",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_RoleModelId",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "RoleModelId",
                table: "Permissions");

            migrationBuilder.CreateTable(
                name: "PermissionModelRoleModel",
                columns: table => new
                {
                    PermissionsId = table.Column<Guid>(type: "uuid", nullable: false),
                    RolesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionModelRoleModel", x => new { x.PermissionsId, x.RolesId });
                    table.ForeignKey(
                        name: "FK_PermissionModelRoleModel_Permissions_PermissionsId",
                        column: x => x.PermissionsId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermissionModelRoleModel_Roles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PermissionModelRoleModel_RolesId",
                table: "PermissionModelRoleModel",
                column: "RolesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PermissionModelRoleModel");

            migrationBuilder.AddColumn<Guid>(
                name: "RoleModelId",
                table: "Permissions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_RoleModelId",
                table: "Permissions",
                column: "RoleModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Roles_RoleModelId",
                table: "Permissions",
                column: "RoleModelId",
                principalTable: "Roles",
                principalColumn: "Id");
        }
    }
}
