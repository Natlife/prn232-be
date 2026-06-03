using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessObjects.Migrations
{
    /// <inheritdoc />
    public partial class MoveSeedToContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AppRoles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Customer" }
                });

            migrationBuilder.InsertData(
                table: "CarBrands",
                columns: new[] { "BrandId", "BrandName", "Country", "Description" },
                values: new object[,]
                {
                    { 1, "Toyota", "Japan", "Toyota Motor Corporation" },
                    { 2, "Ford", "USA", "Ford Motor Company" },
                    { 3, "VinFast", "Vietnam", "VinFast Vietnam" },
                    { 4, "BMW", "Germany", "Bayerische Motoren Werke AG" }
                });

            migrationBuilder.InsertData(
                table: "PartCategories",
                columns: new[] { "CategoryId", "CategoryName", "Description" },
                values: new object[,]
                {
                    { 1, "Động cơ & Truyền động", "Các bộ phận liên quan đến động cơ, hộp số và truyền động." },
                    { 2, "Hệ thống điện & Ắc quy", "Ắc quy, máy phát điện, đèn và hệ thống điện." },
                    { 3, "Dầu nhớt & Hóa chất", "Dầu máy, nước làm mát, dầu phanh và hóa chất bảo dưỡng." },
                    { 4, "Ngoại thất & Phụ kiện", "Lốp xe, gạt mưa, gương và các phụ kiện trang trí ngoại thất." }
                });

            migrationBuilder.InsertData(
                table: "Cars",
                columns: new[] { "CarId", "BrandId", "CarName", "Color", "CreatedAt", "Description", "FuelType", "ImageUrl", "Mileage", "Model", "Price", "Status", "Transmission", "Year" },
                values: new object[,]
                {
                    { 1, 1, "Toyota Camry 2.5Q", "Black", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Xe sang trọng, lịch lãm, gia đình sử dụng kỹ, bảo dưỡng chính hãng.", "Gasoline", "https://images.unsplash.com/photo-1621007947382-bb3c3994e3fb?auto=format&fit=crop&w=600&q=80", 15000, "Camry", 1350000000m, "Available", "Automatic", 2022 },
                    { 2, 1, "Toyota Vios 1.5G", "White", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Xe quốc dân tiết kiệm nhiên liệu, vận hành bền bỉ.", "Gasoline", "https://images.unsplash.com/photo-1605559424843-9e4c228bf1c2?auto=format&fit=crop&w=600&q=80", 28000, "Vios", 520000000m, "Available", "Automatic", 2021 },
                    { 3, 2, "Ford Ranger Wildtrak 2.0L", "Orange", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Vua bán tải, phiên bản cao cấp nhất Wildtrak 2 cầu, đầy đủ công nghệ.", "Diesel", "https://images.unsplash.com/photo-1533473359331-0135ef1b58bf?auto=format&fit=crop&w=600&q=80", 8000, "Ranger", 960000000m, "Available", "Automatic", 2023 },
                    { 4, 3, "VinFast VF8 Plus", "Blue", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Xe điện thông minh Việt Nam, bản Plus pin SDI, công nghệ ADAS hiện đại.", "Electric", "https://images.unsplash.com/photo-1563720223185-11003d516935?auto=format&fit=crop&w=600&q=80", 5000, "VF8", 1100000000m, "Available", "Automatic", 2023 },
                    { 5, 4, "BMW 320i Sport Line", "Red", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dòng sedan thể thao lái cực hay, ngoại hình trẻ trung năng động.", "Gasoline", "https://images.unsplash.com/photo-1555215695-3004980ad54e?auto=format&fit=crop&w=600&q=80", 35000, "3 Series", 1250000000m, "Available", "Automatic", 2020 },
                    { 6, 3, "VinFast VF5 Plus", "Gray", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Xe đô thị cỡ nhỏ thông minh, cực kỳ tiết kiệm và nhỏ gọn.", "Electric", "https://images.unsplash.com/photo-1617788138017-80ad40651399?auto=format&fit=crop&w=600&q=80", 2000, "VF5", 450000000m, "Available", "Automatic", 2023 }
                });

            migrationBuilder.InsertData(
                table: "Parts",
                columns: new[] { "PartId", "Brand", "CategoryId", "CreatedAt", "Description", "ImageUrl", "PartCode", "PartName", "Price", "Quantity", "Status" },
                values: new object[,]
                {
                    { 1, "Michelin", 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lốp hiệu năng cao, bám đường cực tốt trong mọi điều kiện thời tiết.", "https://images.unsplash.com/photo-1578844251758-2f71da64c96f?auto=format&fit=crop&w=600&q=80", "PT-MIC-PS4", "Lốp xe Michelin Pilot Sport 4", 3200000m, 40, "Available" },
                    { 2, "GS Battery", 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ắc quy khô miễn bảo dưỡng, độ bền cao, khởi động mạnh mẽ.", "https://images.unsplash.com/photo-1619642751034-765dfdf7c58e?auto=format&fit=crop&w=600&q=80", "PT-GS-12V45", "Ắc quy GS 12V 45Ah", 1450000m, 25, "Available" },
                    { 3, "Castrol", 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dầu nhớt công nghệ tổng hợp hoàn toàn bảo vệ động cơ ngay khi khởi động.", "https://images.unsplash.com/photo-1622560480605-d83c853bc5c3?auto=format&fit=crop&w=600&q=80", "PT-CAS-5W30", "Dầu nhớt Castrol Magnatec 5W-30", 850000m, 50, "Available" },
                    { 4, "Bosch", 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gạt mưa cao cấp từ Bosch Đức, gạt sạch nước nhẹ nhàng, êm ái.", "https://images.unsplash.com/photo-1517524206127-48bbd363f3d7?auto=format&fit=crop&w=600&q=80", "PT-BOS-AERO", "Gạt mưa Bosch Aerotwin", 450000m, 60, "Available" },
                    { 5, "Philips", 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bóng đèn LED H7 siêu sáng, gom sáng tốt, độ bền lên đến 5 năm.", "https://images.unsplash.com/photo-1508974239320-0a029497e820?auto=format&fit=crop&w=600&q=80", "PT-PHI-LEDH7", "Đèn pha LED Philips Ultinon Essential", 1200000m, 15, "Available" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppRoles",
                keyColumn: "RoleId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AppRoles",
                keyColumn: "RoleId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "CarId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "CarId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "CarId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "CarId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "CarId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "CarId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "PartCategories",
                keyColumn: "CategoryId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Parts",
                keyColumn: "PartId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Parts",
                keyColumn: "PartId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Parts",
                keyColumn: "PartId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Parts",
                keyColumn: "PartId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Parts",
                keyColumn: "PartId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "CarBrands",
                keyColumn: "BrandId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CarBrands",
                keyColumn: "BrandId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CarBrands",
                keyColumn: "BrandId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CarBrands",
                keyColumn: "BrandId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "PartCategories",
                keyColumn: "CategoryId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PartCategories",
                keyColumn: "CategoryId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PartCategories",
                keyColumn: "CategoryId",
                keyValue: 4);
        }
    }
}
