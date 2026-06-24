using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObjects.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelWithContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CaptchaCode",
                table: "PurchaseRequests",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DepositAmount",
                table: "PurchaseRequests",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DepositDate",
                table: "PurchaseRequests",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DepositExpiry",
                table: "PurchaseRequests",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShippingAddress",
                table: "PartOrders",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryMethod",
                table: "PartOrders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Pickup");

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingFee",
                table: "PartOrders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "DepositCaptchas",
                columns: table => new
                {
                    CaptchaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UsedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepositCaptchas", x => x.CaptchaId);
                    table.ForeignKey(
                        name: "FK_DepositCaptchas_Cars",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "CarId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Email",
                value: "admin@gmail.com");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Email",
                value: "customer@gmail.com");

            migrationBuilder.CreateIndex(
                name: "IX_DepositCaptchas_CarId",
                table: "DepositCaptchas",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_DepositCaptchas_Code",
                table: "DepositCaptchas",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepositCaptchas");

            migrationBuilder.DropColumn(
                name: "CaptchaCode",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "DepositAmount",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "DepositDate",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "DepositExpiry",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "DeliveryMethod",
                table: "PartOrders");

            migrationBuilder.DropColumn(
                name: "ShippingFee",
                table: "PartOrders");

            migrationBuilder.AlterColumn<string>(
                name: "ShippingAddress",
                table: "PartOrders",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Email",
                value: "admin@carshowroom.com");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Email",
                value: "customer@carshowroom.com");
        }
    }
}
