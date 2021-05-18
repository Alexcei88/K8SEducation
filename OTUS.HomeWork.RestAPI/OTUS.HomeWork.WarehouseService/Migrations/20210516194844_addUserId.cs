using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OTUS.HomeWork.WarehouseService.Migrations
{
    public partial class addUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "shipments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "shipments");

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
    }
}
