using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class DeskShares : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeskShare",
                columns: table => new
                {
                    FunAccountId = table.Column<long>(type: "bigint", nullable: false),
                    DeskId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeskShare", x => new { x.FunAccountId, x.DeskId });
                    table.ForeignKey(
                        name: "FK_DeskShare_Desks_DeskId",
                        column: x => x.DeskId,
                        principalTable: "Desks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeskShare_FunAccounts_FunAccountId",
                        column: x => x.FunAccountId,
                        principalTable: "FunAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeskShare_DeskId",
                table: "DeskShare",
                column: "DeskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeskShare");
        }
    }
}
