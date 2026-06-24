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
                    { 1, "D?ng co & Truy?n d?ng", "C?c b? ph?n li?n quan d?n d?ng co, h?p s? v? truy?n d?ng." },
                    { 2, "H? th?ng di?n & ?c quy", "?c quy, m?y ph?t di?n, d?n v? h? th?ng di?n." },
                    { 3, "D?u nh?t & H?a ch?t", "D?u m?y, nu?c l?m m?t, d?u phanh v? h?a ch?t b?o du?ng." },
                    { 4, "Ngo?i th?t & Ph? ki?n", "L?p xe, g?t mua, guong v? c?c ph? ki?n trang tr? ngo?i th?t." }
                });

            migrationBuilder.InsertData(
                table: "Cars",
                columns: new[] { "CarId", "BrandId", "CarName", "Color", "CreatedAt", "Description", "FuelType", "ImageUrl", "Mileage", "Model", "Price", "Status", "Transmission", "Year" },
                values: new object[,]
                {
                    { 1, 1, "Toyota Camry 2.5Q", "Black", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Xe sang tr?ng, l?ch l?m, gia d?nh s? d?ng k?, b?o du?ng ch?nh h?ng.", "Gasoline", "https://images.unsplash.com/photo-1621007947382-bb3c3994e3fb?auto=format&fit=crop&w=600&q=80", 15000, "Camry", 1350000000m, "Available", "Automatic", 2022 },
                    { 2, 1, "Toyota Vios 1.5G", "White", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Xe qu?c d?n ti?t ki?m nhi?n li?u, v?n h?nh b?n b?.", "Gasoline", "https://images.unsplash.com/photo-1605559424843-9e4c228bf1c2?auto=format&fit=crop&w=600&q=80", 28000, "Vios", 520000000m, "Available", "Automatic", 2021 },
                    { 3, 2, "Ford Ranger Wildtrak 2.0L", "Orange", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Vua b?n t?i, phi?n b?n cao c?p nh?t Wildtrak 2 c?u, d?y d? c?ng ngh?.", "Diesel", "https://images.unsplash.com/photo-1533473359331-0135ef1b58bf?auto=format&fit=crop&w=600&q=80", 8000, "Ranger", 960000000m, "Available", "Automatic", 2023 },
                    { 4, 3, "VinFast VF8 Plus", "Blue", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Xe di?n th?ng minh Vi?t Nam, b?n Plus pin SDI, c?ng ngh? ADAS hi?n d?i.", "Electric", "https://images.unsplash.com/photo-1563720223185-11003d516935?auto=format&fit=crop&w=600&q=80", 5000, "VF8", 1100000000m, "Available", "Automatic", 2023 },
                    { 5, 4, "BMW 320i Sport Line", "Red", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D?ng sedan th? thao l?i c?c hay, ngo?i h?nh tr? trung nang d?ng.", "Gasoline", "https://images.unsplash.com/photo-1555215695-3004980ad54e?auto=format&fit=crop&w=600&q=80", 35000, "3 Series", 1250000000m, "Available", "Automatic", 2020 },
                    { 6, 3, "VinFast VF5 Plus", "Gray", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Xe d? th? c? nh? th?ng minh, c?c k? ti?t ki?m v? nh? g?n.", "Electric", "https://images.unsplash.com/photo-1617788138017-80ad40651399?auto=format&fit=crop&w=600&q=80", 2000, "VF5", 450000000m, "Available", "Automatic", 2023 }
                });

            migrationBuilder.InsertData(
                table: "Parts",
                columns: new[] { "PartId", "Brand", "CategoryId", "CreatedAt", "Description", "ImageUrl", "PartCode", "PartName", "Price", "Quantity", "Status" },
                values: new object[,]
                {
                    { 1, "Michelin", 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "L?p hi?u nang cao, b?m du?ng c?c t?t trong m?i di?u ki?n th?i ti?t.", "https://images.unsplash.com/photo-1578844251758-2f71da64c96f?auto=format&fit=crop&w=600&q=80", "PT-MIC-PS4", "L?p xe Michelin Pilot Sport 4", 3200000m, 40, "Available" },
                    { 2, "GS Battery", 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "?c quy kh? mi?n b?o du?ng, d? b?n cao, kh?i d?ng m?nh m?.", "https://images.unsplash.com/photo-1619642751034-765dfdf7c58e?auto=format&fit=crop&w=600&q=80", "PT-GS-12V45", "?c quy GS 12V 45Ah", 1450000m, 25, "Available" },
                    { 3, "Castrol", 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D?u nh?t c?ng ngh? t?ng h?p ho?n to?n b?o v? d?ng co ngay khi kh?i d?ng.", "https://images.unsplash.com/photo-1622560480605-d83c853bc5c3?auto=format&fit=crop&w=600&q=80", "PT-CAS-5W30", "D?u nh?t Castrol Magnatec 5W-30", 850000m, 50, "Available" },
                    { 4, "Bosch", 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "G?t mua cao c?p t? Bosch D?c, g?t s?ch nu?c nh? nh?ng, ?m ?i.", "https://images.unsplash.com/photo-1517524206127-48bbd363f3d7?auto=format&fit=crop&w=600&q=80", "PT-BOS-AERO", "G?t mua Bosch Aerotwin", 450000m, 60, "Available" },
                    { 5, "Philips", 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "B?ng d?n LED H7 si?u s?ng, gom s?ng t?t, d? b?n l?n d?n 5 nam.", "https://images.unsplash.com/photo-1508974239320-0a029497e820?auto=format&fit=crop&w=600&q=80", "PT-PHI-LEDH7", "D?n pha LED Philips Ultinon Essential", 1200000m, 15, "Available" }
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
