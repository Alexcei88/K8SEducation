using Microsoft.EntityFrameworkCore.Migrations;

namespace OTUS.HomeWork.BillingService.Migrations
{
    public partial class IdempotencyKeyToPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "idempotance_key",
                table: "payments",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_payments_idempotance_key",
                table: "payments",
                column: "idempotance_key",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_payments_idempotance_key",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "idempotance_key",
                table: "payments");
        }
    }
}
