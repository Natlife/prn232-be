using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObjects.Migrations
{
    /// <inheritdoc />
    public partial class AddChatbotAndComboOrder : Migration
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

            migrationBuilder.CreateTable(
                name: "ComboOrders",
                columns: table => new
                {
                    ComboOrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ShippingAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "manual"),
                    ChatSessionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ComboOrders", x => x.ComboOrderId);
                    table.ForeignKey(
                        name: "FK_ComboOrders_AppUsers",
                        column: x => x.CustomerId,
                        principalTable: "AppUsers",
                        principalColumn: "UserId");
                });

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

            migrationBuilder.CreateTable(
                name: "ComboOrderItems",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComboOrderId = table.Column<int>(type: "int", nullable: false),
                    ItemType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ReferenceId = table.Column<int>(type: "int", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ComboOrderItems", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_ComboOrderItems_ComboOrders",
                        column: x => x.ComboOrderId,
                        principalTable: "ComboOrders",
                        principalColumn: "ComboOrderId",
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
                name: "IX_ComboOrderItems_ComboOrderId",
                table: "ComboOrderItems",
                column: "ComboOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ComboOrders_CustomerId",
                table: "ComboOrders",
                column: "CustomerId");

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
                name: "ComboOrderItems");

            migrationBuilder.DropTable(
                name: "DepositCaptchas");

            migrationBuilder.DropTable(
                name: "ComboOrders");

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
