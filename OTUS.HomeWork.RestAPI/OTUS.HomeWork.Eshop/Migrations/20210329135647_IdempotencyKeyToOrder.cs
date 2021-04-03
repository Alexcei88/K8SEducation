using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OTUS.HomeWork.EShop.Migrations
{
    public partial class IdempotencyKeyToOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("9b52dbca-8b0b-4ab7-8dfa-0a5c994b2169"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("bfa42e50-b8d3-4500-b90c-4670f8754930"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("c5d606b7-0deb-4da6-9cc5-e9f878adc165"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("f3775353-956b-45c3-9c44-ed367439bdfe"));

            migrationBuilder.AddColumn<string>(
                name: "idempotency_key",
                table: "orders",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "id", "description", "name", "price" },
                values: new object[,]
                {
                    { new Guid("46a607cd-4abd-49d1-bd74-9980b9c415d4"), "", "Футбольный Мяч", 100m },
                    { new Guid("0dbfd70b-03d9-4fec-a3bb-42aac257ac64"), "", "Футбольная сетка", 400m },
                    { new Guid("9e6cc9d1-87d2-4ba8-b330-84d8046d5281"), "", "Футболка", 20m },
                    { new Guid("ea5ea612-fc48-4b1e-ae8b-fc5c38abcfbe"), "", "Шорты", 20m }
                });

            migrationBuilder.CreateIndex(
                name: "ix_orders_idempotency_key",
                table: "orders",
                column: "idempotency_key",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_orders_idempotency_key",
                table: "orders");

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("0dbfd70b-03d9-4fec-a3bb-42aac257ac64"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("46a607cd-4abd-49d1-bd74-9980b9c415d4"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("9e6cc9d1-87d2-4ba8-b330-84d8046d5281"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("ea5ea612-fc48-4b1e-ae8b-fc5c38abcfbe"));

            migrationBuilder.DropColumn(
                name: "idempotency_key",
                table: "orders");

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
        }
    }
}
