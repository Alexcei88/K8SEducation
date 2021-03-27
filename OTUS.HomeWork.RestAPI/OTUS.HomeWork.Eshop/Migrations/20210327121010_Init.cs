using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OTUS.HomeWork.EShop.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    order_number = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    total_price = table.Column<decimal>(type: "numeric", nullable: false),
                    billing_id = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_on_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    paid_date_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_orders", x => x.order_number);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "order_item",
                columns: table => new
                {
                    order_number_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    order_number1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_item", x => new { x.product_id, x.order_number_id });
                    table.ForeignKey(
                        name: "fk_order_item_orders_order_number",
                        column: x => x.order_number_id,
                        principalTable: "orders",
                        principalColumn: "order_number",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_order_item_orders_order_number1",
                        column: x => x.order_number1,
                        principalTable: "orders",
                        principalColumn: "order_number",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "id", "description", "name", "price" },
                values: new object[,]
                {
                    { new Guid("9b52dbca-8b0b-4ab7-8dfa-0a5c994b2169"), "", "Футбольный Мяч", 100m },
                    { new Guid("f3775353-956b-45c3-9c44-ed367439bdfe"), "", "Футбольная сетка", 400m },
                    { new Guid("bfa42e50-b8d3-4500-b90c-4670f8754930"), "", "Футболка", 20m },
                    { new Guid("c5d606b7-0deb-4da6-9cc5-e9f878adc165"), "", "Шорты", 20m }
                });

            migrationBuilder.CreateIndex(
                name: "ix_order_item_order_number_id",
                table: "order_item",
                column: "order_number_id");

            migrationBuilder.CreateIndex(
                name: "ix_order_item_order_number1",
                table: "order_item",
                column: "order_number1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_item");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "orders");
        }
    }
}
