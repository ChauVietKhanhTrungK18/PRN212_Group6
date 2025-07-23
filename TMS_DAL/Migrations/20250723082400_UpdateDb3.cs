using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TMS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDb3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Description", "RoleName" },
                values: new object[,]
                {
                    { 1, "Manages the project and team members", "Project Manager" },
                    { 2, "Project team member", "Member" },
                    { 3, "Can view project information only", "Viewer" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "DateCreated", "Email", "FullName", "IsAdmin", "PasswordHash", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 7, 23, 15, 24, 0, 506, DateTimeKind.Local).AddTicks(5921), "admin@example.com", "Admin User", true, "0192023a7bbd73250516f069df18b500c33910a2d4d4074ec5b0d1b0e5b0c1c6", "admin" },
                    { 2, new DateTime(2025, 7, 23, 15, 24, 0, 506, DateTimeKind.Local).AddTicks(5938), "user@example.com", "Normal User", false, "6ad14ba9986e3615423dfca256d04e3f735357c3558b8b4319c49a23ea8bc99c", "user" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "Users");
        }
    }
}
