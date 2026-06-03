using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessObjects.Migrations
{
    /// <inheritdoc />
    public partial class SeedAppUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AppUsers",
                columns: new[] { "UserId", "Address", "CodeExpiryTime", "CreatedAt", "Email", "FullName", "IsActive", "PasswordHash", "PhoneNumber", "RoleId", "VerificationCode" },
                values: new object[,]
                {
                    { 1, "Hanoi", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@carshowroom.com", "System Admin", true, "$2a$11$ivuFcskipHfVJyUk7X7Cy.72DYWJAKQhFt7uaF2kMrwZ/LAHW1cWO", "0987654321", 1, null },
                    { 2, "HCM City", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "customer@carshowroom.com", "John Customer", true, "$2a$11$iR0JU.l1mLeRCyKuClJFxuWqtweaw2kS3oZSRG/lAcD00M603P5Mm", "0123456789", 2, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppUsers",
                keyColumn: "UserId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AppUsers",
                keyColumn: "UserId",
                keyValue: 2);
        }
    }
}
