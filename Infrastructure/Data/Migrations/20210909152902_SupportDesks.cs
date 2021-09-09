using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class SupportDesks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Desks_Folders_FolderId",
                table: "Desks");

            migrationBuilder.RenameColumn(
                name: "FolderId",
                table: "Desks",
                newName: "AuthorAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Desks_FolderId",
                table: "Desks",
                newName: "IX_Desks_AuthorAccountId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Desks",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Desks",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInTrashBin",
                table: "Desks",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "Desks",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "ParentId",
                table: "Desks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Desks",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ColorHex",
                table: "Cards",
                type: "character varying(9)",
                maxLength: 9,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Desks_ParentId",
                table: "Desks",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Desks_Folders_ParentId",
                table: "Desks",
                column: "ParentId",
                principalTable: "Folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Desks_FunAccounts_AuthorAccountId",
                table: "Desks",
                column: "AuthorAccountId",
                principalTable: "FunAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Desks_Folders_ParentId",
                table: "Desks");

            migrationBuilder.DropForeignKey(
                name: "FK_Desks_FunAccounts_AuthorAccountId",
                table: "Desks");

            migrationBuilder.DropIndex(
                name: "IX_Desks_ParentId",
                table: "Desks");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Desks");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Desks");

            migrationBuilder.DropColumn(
                name: "IsInTrashBin",
                table: "Desks");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "Desks");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Desks");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Desks");

            migrationBuilder.RenameColumn(
                name: "AuthorAccountId",
                table: "Desks",
                newName: "FolderId");

            migrationBuilder.RenameIndex(
                name: "IX_Desks_AuthorAccountId",
                table: "Desks",
                newName: "IX_Desks_FolderId");

            migrationBuilder.AlterColumn<string>(
                name: "ColorHex",
                table: "Cards",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(9)",
                oldMaxLength: 9,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Desks_Folders_FolderId",
                table: "Desks",
                column: "FolderId",
                principalTable: "Folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
