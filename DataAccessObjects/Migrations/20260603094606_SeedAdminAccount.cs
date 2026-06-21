using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObjects.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AppUsers",
                columns: new[] { "UserId", "Address", "CodeExpiryTime", "CreatedAt", "Email", "FullName", "IsActive", "PasswordHash", "PhoneNumber", "RoleId", "VerificationCode" },
                values: new object[] { 99, null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@group7.com", "System Admin", true, "$2a$11$jI66yMj5egkHkF0.UF41C.Wls0ZtKv2WbVNqemyJvJjo7OlT8r.4u", "0987654321", 1, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppUsers",
                keyColumn: "UserId",
                keyValue: 99);
        }
    }
}
