using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KM.GD.PrintInvoices.Migrations
{
    public partial class AddStatusToPrintedInvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DATE",
                table: "PrintedOrders",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "STATE",
                table: "PrintedOrders",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TOTAL_PAGES",
                table: "PrintedOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "STATE",
                table: "PrintedOrders");

            migrationBuilder.DropColumn(
                name: "TOTAL_PAGES",
                table: "PrintedOrders");

            migrationBuilder.AlterColumn<string>(
                name: "DATE",
                table: "PrintedOrders",
                type: "varchar(30)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");
        }
    }
}
