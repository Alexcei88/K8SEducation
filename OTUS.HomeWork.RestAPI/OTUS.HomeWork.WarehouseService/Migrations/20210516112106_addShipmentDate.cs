using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OTUS.HomeWork.WarehouseService.Migrations
{
    public partial class addShipmentDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "error_description",
                table: "shipments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ready_to_shipment_date",
                table: "shipments",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "shipment_date",
                table: "shipments",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "was_cancelled",
                table: "shipments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "id", "base_price", "description", "name", "space", "weight" },
                values: new object[,]
                {
                    { new Guid("6636cd9f-b74d-4f9c-8d97-082f9914d42a"), 100m, "", "Футбольный Мяч", 1.0, 2.0 },
                    { new Guid("7f00e30e-21c1-4a64-8011-3ccd84182c3c"), 400m, "", "Футбольная сетка", 0.5, 1.5 },
                    { new Guid("0157153a-7dda-4328-adc6-1111cb713596"), 20m, "", "Футболка", 0.10000000000000001, 0.69999999999999996 },
                    { new Guid("246aac6d-a707-479f-ba09-0e18a9ac86fd"), 20m, "", "Шорты", 0.10000000000000001, 0.40000000000000002 }
                });

            migrationBuilder.InsertData(
                table: "counters",
                columns: new[] { "id", "product_id", "remain_count", "reserve_count", "sold_count" },
                values: new object[,]
                {
                    { new Guid("ec1794e6-113f-42be-95c6-378de6f2eb16"), new Guid("6636cd9f-b74d-4f9c-8d97-082f9914d42a"), 100L, 0L, 0L },
                    { new Guid("8bf583df-5c80-42e2-9e0b-6864b0618926"), new Guid("7f00e30e-21c1-4a64-8011-3ccd84182c3c"), 50L, 0L, 0L },
                    { new Guid("87d1328c-90fc-454b-876e-9ef4850b141b"), new Guid("0157153a-7dda-4328-adc6-1111cb713596"), 20L, 4L, 0L },
                    { new Guid("28ef9d0b-03f2-42da-9e72-19ca81ea725c"), new Guid("246aac6d-a707-479f-ba09-0e18a9ac86fd"), 25L, 1L, 3L }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "counters",
                keyColumn: "id",
                keyValue: new Guid("28ef9d0b-03f2-42da-9e72-19ca81ea725c"));

            migrationBuilder.DeleteData(
                table: "counters",
                keyColumn: "id",
                keyValue: new Guid("87d1328c-90fc-454b-876e-9ef4850b141b"));

            migrationBuilder.DeleteData(
                table: "counters",
                keyColumn: "id",
                keyValue: new Guid("8bf583df-5c80-42e2-9e0b-6864b0618926"));

            migrationBuilder.DeleteData(
                table: "counters",
                keyColumn: "id",
                keyValue: new Guid("ec1794e6-113f-42be-95c6-378de6f2eb16"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("0157153a-7dda-4328-adc6-1111cb713596"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("246aac6d-a707-479f-ba09-0e18a9ac86fd"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("6636cd9f-b74d-4f9c-8d97-082f9914d42a"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("7f00e30e-21c1-4a64-8011-3ccd84182c3c"));

            migrationBuilder.DropColumn(
                name: "error_description",
                table: "shipments");

            migrationBuilder.DropColumn(
                name: "ready_to_shipment_date",
                table: "shipments");

            migrationBuilder.DropColumn(
                name: "shipment_date",
                table: "shipments");

            migrationBuilder.DropColumn(
                name: "was_cancelled",
                table: "shipments");

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
    }
}
