using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OTUS.HomeWork.WarehouseService.Migrations
{
    public partial class basePriceRename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "counters",
                keyColumn: "id",
                keyValue: new Guid("23f5353c-c3fc-477b-998e-b2674f3ab274"));

            migrationBuilder.DeleteData(
                table: "counters",
                keyColumn: "id",
                keyValue: new Guid("45048509-9723-47a9-97e0-f4c54b6e7774"));

            migrationBuilder.DeleteData(
                table: "counters",
                keyColumn: "id",
                keyValue: new Guid("8b7f112f-82ee-4604-84a7-0c0339ff098e"));

            migrationBuilder.DeleteData(
                table: "counters",
                keyColumn: "id",
                keyValue: new Guid("97198e97-698d-4889-b0e6-8d8c7f6d2ef5"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("0292db37-df46-4700-a2ee-452265d1ff21"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("b2e63ad6-84e5-49a8-aae1-c5c47cf6272c"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("d46e9ca8-c3ff-49f5-8568-20e393d84535"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("d87a45ce-6434-461f-aedc-485bcb449d5a"));

            migrationBuilder.RenameColumn(
                name: "price",
                table: "products",
                newName: "base_price");

            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "id", "base_price", "description", "name", "space", "weight" },
                values: new object[,]
                {
                    { new Guid("faa58733-ae65-45aa-abbd-f48736b3c05d"), 100m, "", "Футбольный Мяч", 1.0, 2.0 },
                    { new Guid("0588b8df-a478-4c6a-ae5a-95bcc1935cc5"), 400m, "", "Футбольная сетка", 0.5, 1.5 },
                    { new Guid("da437b1f-f8b3-49e0-9057-6429155ac063"), 20m, "", "Футболка", 0.10000000000000001, 0.69999999999999996 },
                    { new Guid("97726a0b-f8ff-4911-a5c5-863028571afb"), 20m, "", "Шорты", 0.10000000000000001, 0.40000000000000002 }
                });

            migrationBuilder.InsertData(
                table: "counters",
                columns: new[] { "id", "product_id", "remain_count", "reserve_count", "sold_count" },
                values: new object[,]
                {
                    { new Guid("bb3f4b33-4d9b-4af9-aeaf-44d8e4f91b06"), new Guid("faa58733-ae65-45aa-abbd-f48736b3c05d"), 10L, 0L, 0L },
                    { new Guid("51d386f4-919a-4f16-86b4-69d5a9da7a59"), new Guid("0588b8df-a478-4c6a-ae5a-95bcc1935cc5"), 5L, 0L, 0L },
                    { new Guid("c2702386-05d3-45f5-a98e-a0f2ff7acb8b"), new Guid("da437b1f-f8b3-49e0-9057-6429155ac063"), 10L, 4L, 0L },
                    { new Guid("b05d37e5-2ef2-439e-b14b-564bfa5f31da"), new Guid("97726a0b-f8ff-4911-a5c5-863028571afb"), 5L, 1L, 3L }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "counters",
                keyColumn: "id",
                keyValue: new Guid("51d386f4-919a-4f16-86b4-69d5a9da7a59"));

            migrationBuilder.DeleteData(
                table: "counters",
                keyColumn: "id",
                keyValue: new Guid("b05d37e5-2ef2-439e-b14b-564bfa5f31da"));

            migrationBuilder.DeleteData(
                table: "counters",
                keyColumn: "id",
                keyValue: new Guid("bb3f4b33-4d9b-4af9-aeaf-44d8e4f91b06"));

            migrationBuilder.DeleteData(
                table: "counters",
                keyColumn: "id",
                keyValue: new Guid("c2702386-05d3-45f5-a98e-a0f2ff7acb8b"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("0588b8df-a478-4c6a-ae5a-95bcc1935cc5"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("97726a0b-f8ff-4911-a5c5-863028571afb"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("da437b1f-f8b3-49e0-9057-6429155ac063"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("faa58733-ae65-45aa-abbd-f48736b3c05d"));

            migrationBuilder.RenameColumn(
                name: "base_price",
                table: "products",
                newName: "price");

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
        }
    }
}
