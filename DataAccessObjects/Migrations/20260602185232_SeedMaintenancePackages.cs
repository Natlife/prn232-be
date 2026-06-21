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
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kiểm tra toàn diện 30 điểm, thay nhớt động cơ và lọc nhớt, kiểm tra hệ thống phanh và bổ sung nước làm mát. Phù hợp cho bảo dưỡng định kỳ mỗi 5.000 km.", 120, "Bảo dưỡng Tiêu chuẩn", 1500000m, "Available" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kiểm tra hệ thống điện tử bằng máy chuyên dụng, vệ sinh buồng đốt, vệ sinh kim phun, đảo lốp, cân bằng động và thay toàn bộ chất lỏng (dầu máy, dầu phanh, nước làm mát).", 240, "Bảo dưỡng Toàn diện VIP", 4500000m, "Available" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kiểm tra áp suất lốp, độ mòn lốp, hệ thống chiếu sáng, hệ thống phanh, gạt mưa và bình ắc quy để đảm bảo an toàn tuyệt đối cho chuyến đi dài.", 60, "Kiểm tra Xe trước Chuyến đi", 500000m, "Available" }
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
