using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Infrastructure.Data.Migrations
{
    public partial class FixCardConnectionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Action",
                table: "DeskActionHistoryItem",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewData",
                table: "DeskActionHistoryItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OldData",
                table: "DeskActionHistoryItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "DeskActionHistoryItem",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CardConnections",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewData",
                table: "DeskActionHistoryItem");

            migrationBuilder.DropColumn(
                name: "OldData",
                table: "DeskActionHistoryItem");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "DeskActionHistoryItem");

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "DeskActionHistoryItem",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CardConnections",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
