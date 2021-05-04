using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OTUS.HomeWork.EShop.Migrations
{
    public partial class addErrorDescriptionColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.AddColumn<string>(
                name: "error_description",
                table: "orders",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "error_description",
                table: "orders");

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    price = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.id);
                });

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
        }
    }
}
