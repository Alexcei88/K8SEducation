using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OTUS.HomeWork.DeliveryService.Migrations
{
    public partial class initMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "delivery",
                columns: table => new
                {
                    order_number = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_delivery", x => x.order_number);
                });

            migrationBuilder.CreateTable(
                name: "delivery_product",
                columns: table => new
                {
                    order_number = table.Column<string>(type: "text", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    weight = table.Column<double>(type: "double precision", nullable: false),
                    space = table.Column<double>(type: "double precision", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_delivery_product", x => new { x.order_number, x.product_id });
                    table.ForeignKey(
                        name: "fk_delivery_product_delivery_order_number",
                        column: x => x.order_number,
                        principalTable: "delivery",
                        principalColumn: "order_number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "location",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_number = table.Column<string>(type: "text", nullable: false),
                    current_address = table.Column<string>(type: "text", nullable: true),
                    estimated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    shipment_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    delivery_address = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_location", x => x.id);
                    table.ForeignKey(
                        name: "fk_location_delivery_order_number",
                        column: x => x.order_number,
                        principalTable: "delivery",
                        principalColumn: "order_number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_location_order_number",
                table: "location",
                column: "order_number",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "delivery_product");

            migrationBuilder.DropTable(
                name: "location");

            migrationBuilder.DropTable(
                name: "delivery");
        }
    }
}
