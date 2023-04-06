using Microsoft.EntityFrameworkCore.Migrations;

namespace KM.GD.PrintInvoices.Migrations
{
    public partial class CreateIndexedFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IndexedFiles",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FILE_NAME = table.Column<string>(type: "varchar(255)", nullable: true),
                    FULL_PATH = table.Column<string>(type: "varchar(250)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndexedFiles", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IndexedFiles");
        }
    }
}
