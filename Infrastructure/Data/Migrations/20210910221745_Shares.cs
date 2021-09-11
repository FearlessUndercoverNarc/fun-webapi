using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Infrastructure.Data.Migrations
{
    public partial class Shares : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Y",
                table: "Cards",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<long>(
                name: "X",
                table: "Cards",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Cards",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DeskActionHistoryItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeskId = table.Column<long>(type: "bigint", nullable: false),
                    FunAccountId = table.Column<long>(type: "bigint", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: true),
                    DateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsSoftDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeskActionHistoryItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeskActionHistoryItem_Desks_DeskId",
                        column: x => x.DeskId,
                        principalTable: "Desks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeskActionHistoryItem_FunAccounts_FunAccountId",
                        column: x => x.FunAccountId,
                        principalTable: "FunAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FolderShare",
                columns: table => new
                {
                    FunAccountId = table.Column<long>(type: "bigint", nullable: false),
                    FolderId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderShare", x => new { x.FunAccountId, x.FolderId });
                    table.ForeignKey(
                        name: "FK_FolderShare_Folders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "Folders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FolderShare_FunAccounts_FunAccountId",
                        column: x => x.FunAccountId,
                        principalTable: "FunAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeskActionHistoryItem_DeskId",
                table: "DeskActionHistoryItem",
                column: "DeskId");

            migrationBuilder.CreateIndex(
                name: "IX_DeskActionHistoryItem_FunAccountId",
                table: "DeskActionHistoryItem",
                column: "FunAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_DeskActionHistoryItem_IsSoftDeleted",
                table: "DeskActionHistoryItem",
                column: "IsSoftDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_FolderShare_FolderId",
                table: "FolderShare",
                column: "FolderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeskActionHistoryItem");

            migrationBuilder.DropTable(
                name: "FolderShare");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Cards");

            migrationBuilder.AlterColumn<float>(
                name: "Y",
                table: "Cards",
                type: "real",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<float>(
                name: "X",
                table: "Cards",
                type: "real",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
