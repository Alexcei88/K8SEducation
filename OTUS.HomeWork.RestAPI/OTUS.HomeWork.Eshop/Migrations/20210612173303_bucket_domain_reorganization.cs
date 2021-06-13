using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OTUS.HomeWork.EShop.Migrations
{
    public partial class bucket_domain_reorganization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE From buckets");

            migrationBuilder.DropPrimaryKey(
                name: "pk_buckets",
                table: "buckets");

            migrationBuilder.DropColumn(
                name: "quantity",
                table: "buckets");

            migrationBuilder.RenameColumn(
                name: "product_id",
                table: "buckets",
                newName: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_buckets",
                table: "buckets",
                column: "user_id");

            migrationBuilder.CreateTable(
                name: "bucket_item",
                columns: table => new
                {
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bucket_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    bucket_user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bucket_item", x => new { x.bucket_id, x.product_id });
                    table.ForeignKey(
                        name: "fk_bucket_item_buckets_bucket_id",
                        column: x => x.bucket_id,
                        principalTable: "buckets",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_bucket_item_buckets_bucket_user_id",
                        column: x => x.bucket_user_id,
                        principalTable: "buckets",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_bucket_item_bucket_user_id",
                table: "bucket_item",
                column: "bucket_user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bucket_item");

            migrationBuilder.DropPrimaryKey(
                name: "pk_buckets",
                table: "buckets");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "buckets",
                newName: "product_id");

            migrationBuilder.AddColumn<int>(
                name: "quantity",
                table: "buckets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "pk_buckets",
                table: "buckets",
                columns: new[] { "user_id", "product_id" });
        }
    }
}
