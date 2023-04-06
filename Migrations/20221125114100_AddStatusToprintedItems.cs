using Microsoft.EntityFrameworkCore.Migrations;

namespace KM.GD.PrintInvoices.Migrations
{
    public partial class AddStatusToprintedItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "STATE",
                table: "PrintedOrderItems",
                type: "varchar(50)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "STATE",
                table: "PrintedOrderItems");
        }
    }
}
