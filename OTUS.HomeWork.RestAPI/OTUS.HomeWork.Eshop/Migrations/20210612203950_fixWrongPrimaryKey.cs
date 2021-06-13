using Microsoft.EntityFrameworkCore.Migrations;

namespace OTUS.HomeWork.EShop.Migrations
{
    public partial class fixWrongPrimaryKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bucket_item_buckets_bucket_id",
                table: "bucket_item");

            migrationBuilder.DropForeignKey(
                name: "fk_bucket_item_buckets_bucket_user_id",
                table: "bucket_item");

            migrationBuilder.DropPrimaryKey(
                name: "pk_buckets",
                table: "buckets");

            migrationBuilder.RenameColumn(
                name: "bucket_user_id",
                table: "bucket_item",
                newName: "bucket_id1");

            migrationBuilder.RenameIndex(
                name: "ix_bucket_item_bucket_user_id",
                table: "bucket_item",
                newName: "ix_bucket_item_bucket_id1");

            migrationBuilder.AddPrimaryKey(
                name: "pk_buckets",
                table: "buckets",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_bucket_item_buckets_bucket_id",
                table: "bucket_item",
                column: "bucket_id",
                principalTable: "buckets",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_bucket_item_buckets_bucket_id1",
                table: "bucket_item",
                column: "bucket_id1",
                principalTable: "buckets",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bucket_item_buckets_bucket_id",
                table: "bucket_item");

            migrationBuilder.DropForeignKey(
                name: "fk_bucket_item_buckets_bucket_id1",
                table: "bucket_item");

            migrationBuilder.DropPrimaryKey(
                name: "pk_buckets",
                table: "buckets");

            migrationBuilder.RenameColumn(
                name: "bucket_id1",
                table: "bucket_item",
                newName: "bucket_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_bucket_item_bucket_id1",
                table: "bucket_item",
                newName: "ix_bucket_item_bucket_user_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_buckets",
                table: "buckets",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_bucket_item_buckets_bucket_id",
                table: "bucket_item",
                column: "bucket_id",
                principalTable: "buckets",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_bucket_item_buckets_bucket_user_id",
                table: "bucket_item",
                column: "bucket_user_id",
                principalTable: "buckets",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
