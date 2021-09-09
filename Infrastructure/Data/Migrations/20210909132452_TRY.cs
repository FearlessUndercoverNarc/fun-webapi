using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Infrastructure.Data.Migrations
{
    public partial class TRY : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FunAccounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Login = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Fio = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Password = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    HasSubscription = table.Column<bool>(type: "boolean", nullable: false),
                    IsSoftDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FunAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Folders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    AuthorAccountId = table.Column<long>(type: "bigint", nullable: false),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    IsInTrashBin = table.Column<bool>(type: "boolean", nullable: false),
                    IsSoftDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Folders_Folders_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Folders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Folders_FunAccounts_AuthorAccountId",
                        column: x => x.AuthorAccountId,
                        principalTable: "FunAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TokenSessions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
                    FunAccountId = table.Column<long>(type: "bigint", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsSoftDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TokenSessions_FunAccounts_FunAccountId",
                        column: x => x.FunAccountId,
                        principalTable: "FunAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Desks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FolderId = table.Column<long>(type: "bigint", nullable: false),
                    IsSoftDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Desks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Desks_Folders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "Folders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    X = table.Column<float>(type: "real", nullable: false),
                    Y = table.Column<float>(type: "real", nullable: false),
                    Title = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    ExternalUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    ColorHex = table.Column<string>(type: "text", nullable: true),
                    DeskId = table.Column<long>(type: "bigint", nullable: false),
                    IsSoftDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cards_Desks_DeskId",
                        column: x => x.DeskId,
                        principalTable: "Desks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardConnections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CardLeftId = table.Column<long>(type: "bigint", nullable: false),
                    CardRightId = table.Column<long>(type: "bigint", nullable: false),
                    DeskId = table.Column<long>(type: "bigint", nullable: true),
                    IsSoftDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardConnections", x => new { x.Id, x.CardLeftId, x.CardRightId });
                    table.ForeignKey(
                        name: "FK_CardConnections_Cards_CardLeftId",
                        column: x => x.CardLeftId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardConnections_Cards_CardRightId",
                        column: x => x.CardRightId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardConnections_Desks_DeskId",
                        column: x => x.DeskId,
                        principalTable: "Desks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardConnections_CardLeftId",
                table: "CardConnections",
                column: "CardLeftId");

            migrationBuilder.CreateIndex(
                name: "IX_CardConnections_CardRightId",
                table: "CardConnections",
                column: "CardRightId");

            migrationBuilder.CreateIndex(
                name: "IX_CardConnections_DeskId",
                table: "CardConnections",
                column: "DeskId");

            migrationBuilder.CreateIndex(
                name: "IX_CardConnections_IsSoftDeleted",
                table: "CardConnections",
                column: "IsSoftDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_DeskId",
                table: "Cards",
                column: "DeskId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_IsSoftDeleted",
                table: "Cards",
                column: "IsSoftDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Desks_FolderId",
                table: "Desks",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Desks_IsSoftDeleted",
                table: "Desks",
                column: "IsSoftDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_AuthorAccountId",
                table: "Folders",
                column: "AuthorAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_IsSoftDeleted",
                table: "Folders",
                column: "IsSoftDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_ParentId",
                table: "Folders",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_FunAccounts_IsSoftDeleted",
                table: "FunAccounts",
                column: "IsSoftDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TokenSessions_FunAccountId",
                table: "TokenSessions",
                column: "FunAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TokenSessions_IsSoftDeleted",
                table: "TokenSessions",
                column: "IsSoftDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TokenSessions_Token",
                table: "TokenSessions",
                column: "Token");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardConnections");

            migrationBuilder.DropTable(
                name: "TokenSessions");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Desks");

            migrationBuilder.DropTable(
                name: "Folders");

            migrationBuilder.DropTable(
                name: "FunAccounts");
        }
    }
}
