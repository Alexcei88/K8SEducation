using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OTUS.HomeWork.WarehouseService.Migrations
{
    public partial class initMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    weight = table.Column<double>(type: "double precision", nullable: false),
                    space = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "shipments",
                columns: table => new
                {
                    order_number = table.Column<string>(type: "text", nullable: false),
                    delivery_address = table.Column<string>(type: "text", nullable: true),
                    product_ids = table.Column<List<Guid>>(type: "uuid[]", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shipments", x => x.order_number);
                });

            migrationBuilder.CreateTable(
                name: "counters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    remain_count = table.Column<long>(type: "bigint", nullable: false),
                    sold_count = table.Column<long>(type: "bigint", nullable: false),
                    reserve_count = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_counters", x => x.id);
                    table.ForeignKey(
                        name: "fk_counters_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reserves",
                columns: table => new
                {
                    order_number = table.Column<string>(type: "text", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    count = table.Column<long>(type: "bigint", nullable: false),
                    reserve_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reserves", x => new { x.product_id, x.order_number });
                    table.ForeignKey(
                        name: "fk_reserves_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "id", "description", "name", "price", "space", "weight" },
                values: new object[,]
                {
                    { new Guid("b2e63ad6-84e5-49a8-aae1-c5c47cf6272c"), "", "Футбольный Мяч", 100m, 1.0, 2.0 },
                    { new Guid("0292db37-df46-4700-a2ee-452265d1ff21"), "", "Футбольная сетка", 400m, 0.5, 1.5 },
                    { new Guid("d46e9ca8-c3ff-49f5-8568-20e393d84535"), "", "Футболка", 20m, 0.10000000000000001, 0.69999999999999996 },
                    { new Guid("d87a45ce-6434-461f-aedc-485bcb449d5a"), "", "Шорты", 20m, 0.10000000000000001, 0.40000000000000002 }
                });

            migrationBuilder.InsertData(
                table: "counters",
                columns: new[] { "id", "product_id", "remain_count", "reserve_count", "sold_count" },
                values: new object[,]
                {
                    { new Guid("23f5353c-c3fc-477b-998e-b2674f3ab274"), new Guid("b2e63ad6-84e5-49a8-aae1-c5c47cf6272c"), 10L, 0L, 0L },
                    { new Guid("97198e97-698d-4889-b0e6-8d8c7f6d2ef5"), new Guid("0292db37-df46-4700-a2ee-452265d1ff21"), 5L, 0L, 0L },
                    { new Guid("45048509-9723-47a9-97e0-f4c54b6e7774"), new Guid("d46e9ca8-c3ff-49f5-8568-20e393d84535"), 10L, 4L, 0L },
                    { new Guid("8b7f112f-82ee-4604-84a7-0c0339ff098e"), new Guid("d87a45ce-6434-461f-aedc-485bcb449d5a"), 5L, 1L, 3L }
                });

            migrationBuilder.CreateIndex(
                name: "ix_counters_product_id",
                table: "counters",
                column: "product_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "counters");

            migrationBuilder.DropTable(
                name: "reserves");

            migrationBuilder.DropTable(
                name: "shipments");

            migrationBuilder.DropTable(
                name: "products");
        }
    }
}
