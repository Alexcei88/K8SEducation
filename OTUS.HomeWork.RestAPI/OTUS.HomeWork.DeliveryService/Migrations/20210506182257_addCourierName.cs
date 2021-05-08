using Microsoft.EntityFrameworkCore.Migrations;

namespace OTUS.HomeWork.DeliveryService.Migrations
{
    public partial class addCourierName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "courier_name",
                table: "delivery",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "courier_name",
                table: "delivery");
        }
    }
}
