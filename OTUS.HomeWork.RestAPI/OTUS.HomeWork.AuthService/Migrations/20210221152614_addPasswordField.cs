using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OTUS.HomeWork.AuthService.Migrations
{
    public partial class addPasswordField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("323baa9f-94c8-4b9f-a1a8-4524553cf81a"));

            migrationBuilder.AddColumn<string>(
                name: "password",
                table: "users",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "password",
                table: "users");

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "email", "first_name", "last_name", "phone", "user_name" },
                values: new object[] { new Guid("323baa9f-94c8-4b9f-a1a8-4524553cf81a"), "kuber@otus.ru", "OTUS", "Kubernetovich", "+9876543210", "User1" });
        }
    }
}
