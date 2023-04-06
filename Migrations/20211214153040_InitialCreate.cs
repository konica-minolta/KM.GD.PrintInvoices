using Microsoft.EntityFrameworkCore.Migrations;

namespace KM.GD.PrintInvoices.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrintedOrders",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    INVOICE_FILE = table.Column<string>(type: "varchar(250)", nullable: true),
                    ORDER_ID = table.Column<string>(type: "varchar(50)", nullable: true),
                    ISSUBMISSIONVALID = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintedOrders", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PrintedOrderItems",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ORDER_ITEM_ID = table.Column<string>(type: "varchar(50)", nullable: true),
                    PrintedOrderID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintedOrderItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PrintedOrderItems_PrintedOrders_PrintedOrderID",
                        column: x => x.PrintedOrderID,
                        principalTable: "PrintedOrders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrintedOrderItems_PrintedOrderID",
                table: "PrintedOrderItems",
                column: "PrintedOrderID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrintedOrderItems");

            migrationBuilder.DropTable(
                name: "PrintedOrders");
        }
    }
}
