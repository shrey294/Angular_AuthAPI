using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Angular_AuthAPI.Migrations
{
    public partial class addedresetfields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResentPasswordToken",
                table: "users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetPasswordExpiry",
                table: "users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResentPasswordToken",
                table: "users");

            migrationBuilder.DropColumn(
                name: "ResetPasswordExpiry",
                table: "users");
        }
    }
}
