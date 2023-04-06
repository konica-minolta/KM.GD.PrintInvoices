using Microsoft.EntityFrameworkCore.Migrations;

namespace KM.GD.PrintInvoices.Migrations
{
    public partial class PritedFilesDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DATE",
                table: "PrintedOrders",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "N_DOCS",
                table: "PrintedOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DATE",
                table: "PrintedOrders");

            migrationBuilder.DropColumn(
                name: "N_DOCS",
                table: "PrintedOrders");
        }
    }
}
