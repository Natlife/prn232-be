using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessObjects.Migrations
{
    /// <inheritdoc />
    public partial class SeedMaintenancePackages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "MaintenancePackages",
                columns: new[] { "PackageId", "CreatedAt", "Description", "EstimatedDuration", "PackageName", "Price", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ki?m tra to?n di?n 30 di?m, thay nh?t d?ng co v? l?c nh?t, ki?m tra h? th?ng phanh v? b? sung nu?c l?m m?t. Ph? h?p cho b?o du?ng d?nh k? m?i 5.000 km.", 120, "B?o du?ng Ti?u chu?n", 1500000m, "Available" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ki?m tra h? th?ng di?n t? b?ng m?y chuy?n d?ng, v? sinh bu?ng d?t, v? sinh kim phun, d?o l?p, c?n b?ng d?ng v? thay to?n b? ch?t l?ng (d?u m?y, d?u phanh, nu?c l?m m?t).", 240, "B?o du?ng To?n di?n VIP", 4500000m, "Available" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ki?m tra ?p su?t l?p, d? m?n l?p, h? th?ng chi?u s?ng, h? th?ng phanh, g?t mua v? b?nh ?c quy d? d?m b?o an to?n tuy?t d?i cho chuy?n di d?i.", 60, "Ki?m tra Xe tru?c Chuy?n di", 500000m, "Available" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MaintenancePackages",
                keyColumn: "PackageId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MaintenancePackages",
                keyColumn: "PackageId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MaintenancePackages",
                keyColumn: "PackageId",
                keyValue: 3);
        }
    }
}
