using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OTUS.HomeWork.WarehouseService.Migrations
{
    public partial class addProductCategoryColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "counters",
                keyColumn: "id",
                keyValue: new Guid("08cb69c4-02a2-4729-9fc9-cc456eadec61"));

            migrationBuilder.DeleteData(
                table: "counters",
                keyColumn: "id",
                keyValue: new Guid("5de243bb-8ce6-459d-a441-0ceb8a274c10"));

            migrationBuilder.DeleteData(
                table: "counters",
                keyColumn: "id",
                keyValue: new Guid("8386d901-a049-4d64-b5c7-875bd342720f"));

            migrationBuilder.DeleteData(
                table: "counters",
                keyColumn: "id",
                keyValue: new Guid("b0ec6094-15a2-490b-8c5d-845aeb0c37b4"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("876b7f79-dc25-45b6-ac0d-db8221ce9f96"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("af438e30-c6c5-4b66-822f-5fe7c5466535"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("ef5ebaaa-0505-4fd9-9216-4bd596a877b3"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("f6e1e19c-0ac0-47f1-b64e-ab7f2f1e3a8e"));

            migrationBuilder.AddColumn<string>(
                name: "category",
                table: "products",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "id", "base_price", "category", "description", "name", "space", "weight" },
                values: new object[,]
                {
                    { new Guid("d70af059-b117-4b8a-ae66-85a96647c6a8"), 100m, "Инвертарь", "", "Футбольный Мяч", 1.0, 2.0 },
                    { new Guid("5f386403-d370-4c06-87e5-f9413ae732fd"), 400m, "Инвентарь", "", "Футбольная сетка", 0.5, 1.5 },
                    { new Guid("b290b26b-e8da-4549-a34c-20145a17e98d"), 20m, "Одежда", "", "Футболка", 0.10000000000000001, 0.69999999999999996 },
                    { new Guid("745333a0-f931-48b8-be2b-250e2dba9a69"), 20m, "Одежда", "", "Шорты", 0.10000000000000001, 0.40000000000000002 },
                    { new Guid("76579bed-d0b7-43c4-a5e0-62672beed9fc"), 20m, "Обувь", "", "Бутсы", 0.10000000000000001, 0.40000000000000002 },
                    { new Guid("db607615-e366-4294-92d8-768c3042cf89"), 10m, "Инвентарь", "", "Футбольные фишки", 1.5, 0.29999999999999999 }
                });

            migrationBuilder.InsertData(
                table: "counters",
                columns: new[] { "id", "product_id", "remain_count", "reserve_count", "sold_count" },
                values: new object[,]
                {
                    { new Guid("8ab4c572-1bc7-4f5e-a59f-8213c410c55a"), new Guid("d70af059-b117-4b8a-ae66-85a96647c6a8"), 100L, 0L, 0L },
                    { new Guid("a0e09eac-e2db-42f8-887d-43694cf4db99"), new Guid("5f386403-d370-4c06-87e5-f9413ae732fd"), 50L, 0L, 0L },
                    { new Guid("4bcd8092-5d40-4e25-a96e-a8e0b1eed09e"), new Guid("b290b26b-e8da-4549-a34c-20145a17e98d"), 20L, 4L, 0L },
                    { new Guid("463249bd-4b40-42f1-b3c3-bb04e921a8e7"), new Guid("745333a0-f931-48b8-be2b-250e2dba9a69"), 25L, 1L, 3L },
                    { new Guid("28b0c05d-2a12-441b-952f-105aa7cae747"), new Guid("76579bed-d0b7-43c4-a5e0-62672beed9fc"), 25L, 1L, 3L },
                    { new Guid("ed7080b6-04e8-4cbd-8260-2b9efb4c28c5"), new Guid("db607615-e366-4294-92d8-768c3042cf89"), 25L, 1L, 3L }
                });

            migrationBuilder.CreateIndex(
                name: "ix_products_category",
                table: "products",
                column: "category");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_products_category",
                table: "products");

            migrationBuilder.DeleteData(
                table: "counters",
                keyColumn: "id",
                keyValue: new Guid("28b0c05d-2a12-441b-952f-105aa7cae747"));

            migrationBuilder.DeleteData(
                table: "counters",
                keyColumn: "id",
                keyValue: new Guid("463249bd-4b40-42f1-b3c3-bb04e921a8e7"));

            migrationBuilder.DeleteData(
                table: "counters",
                keyColumn: "id",
                keyValue: new Guid("4bcd8092-5d40-4e25-a96e-a8e0b1eed09e"));

            migrationBuilder.DeleteData(
                table: "counters",
                keyColumn: "id",
                keyValue: new Guid("8ab4c572-1bc7-4f5e-a59f-8213c410c55a"));

            migrationBuilder.DeleteData(
                table: "counters",
                keyColumn: "id",
                keyValue: new Guid("a0e09eac-e2db-42f8-887d-43694cf4db99"));

            migrationBuilder.DeleteData(
                table: "counters",
                keyColumn: "id",
                keyValue: new Guid("ed7080b6-04e8-4cbd-8260-2b9efb4c28c5"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("5f386403-d370-4c06-87e5-f9413ae732fd"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("745333a0-f931-48b8-be2b-250e2dba9a69"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("76579bed-d0b7-43c4-a5e0-62672beed9fc"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("b290b26b-e8da-4549-a34c-20145a17e98d"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("d70af059-b117-4b8a-ae66-85a96647c6a8"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("db607615-e366-4294-92d8-768c3042cf89"));

            migrationBuilder.DropColumn(
                name: "category",
                table: "products");

            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "id", "base_price", "description", "name", "space", "weight" },
                values: new object[,]
                {
                    { new Guid("af438e30-c6c5-4b66-822f-5fe7c5466535"), 100m, "", "Футбольный Мяч", 1.0, 2.0 },
                    { new Guid("ef5ebaaa-0505-4fd9-9216-4bd596a877b3"), 400m, "", "Футбольная сетка", 0.5, 1.5 },
                    { new Guid("f6e1e19c-0ac0-47f1-b64e-ab7f2f1e3a8e"), 20m, "", "Футболка", 0.10000000000000001, 0.69999999999999996 },
                    { new Guid("876b7f79-dc25-45b6-ac0d-db8221ce9f96"), 20m, "", "Шорты", 0.10000000000000001, 0.40000000000000002 }
                });

            migrationBuilder.InsertData(
                table: "counters",
                columns: new[] { "id", "product_id", "remain_count", "reserve_count", "sold_count" },
                values: new object[,]
                {
                    { new Guid("8386d901-a049-4d64-b5c7-875bd342720f"), new Guid("af438e30-c6c5-4b66-822f-5fe7c5466535"), 100L, 0L, 0L },
                    { new Guid("5de243bb-8ce6-459d-a441-0ceb8a274c10"), new Guid("ef5ebaaa-0505-4fd9-9216-4bd596a877b3"), 50L, 0L, 0L },
                    { new Guid("b0ec6094-15a2-490b-8c5d-845aeb0c37b4"), new Guid("f6e1e19c-0ac0-47f1-b64e-ab7f2f1e3a8e"), 20L, 4L, 0L },
                    { new Guid("08cb69c4-02a2-4729-9fc9-cc456eadec61"), new Guid("876b7f79-dc25-45b6-ac0d-db8221ce9f96"), 25L, 1L, 3L }
                });
        }
    }
}
