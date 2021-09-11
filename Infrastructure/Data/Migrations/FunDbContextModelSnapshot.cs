﻿// <auto-generated />
using System;
using Infrastructure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Infrastructure.Data.Migrations
{
    [DbContext(typeof(FunDbContext))]
    partial class FunDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Models.Db.Account.FunAccount", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Fio")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("HasSubscription")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsSoftDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Login")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<string>("Password")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.HasKey("Id");

                    b.HasIndex("IsSoftDeleted");

                    b.ToTable("FunAccounts");
                });

            modelBuilder.Entity("Models.Db.Relations.DeskShare", b =>
                {
                    b.Property<long>("FunAccountId")
                        .HasColumnType("bigint");

                    b.Property<long>("DeskId")
                        .HasColumnType("bigint");

                    b.HasKey("FunAccountId", "DeskId");

                    b.HasIndex("DeskId");

                    b.ToTable("DeskShare");
                });

            modelBuilder.Entity("Models.Db.Relations.FolderShare", b =>
                {
                    b.Property<long>("FunAccountId")
                        .HasColumnType("bigint");

                    b.Property<long>("FolderId")
                        .HasColumnType("bigint");

                    b.HasKey("FunAccountId", "FolderId");

                    b.HasIndex("FolderId");

                    b.ToTable("FolderShare");
                });

            modelBuilder.Entity("Models.Db.Sessions.TokenSession", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("FunAccountId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsSoftDeleted")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Token")
                        .HasMaxLength(36)
                        .HasColumnType("character varying(36)");

                    b.HasKey("Id");

                    b.HasIndex("FunAccountId");

                    b.HasIndex("IsSoftDeleted");

                    b.HasIndex("Token");

                    b.ToTable("TokenSessions");
                });

            modelBuilder.Entity("Models.Db.Tree.Card", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ColorHex")
                        .HasMaxLength(9)
                        .HasColumnType("character varying(9)");

                    b.Property<string>("Description")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<long>("DeskId")
                        .HasColumnType("bigint");

                    b.Property<string>("ExternalUrl")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<string>("Image")
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)");

                    b.Property<bool>("IsSoftDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Title")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<long>("X")
                        .HasColumnType("bigint");

                    b.Property<long>("Y")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("DeskId");

                    b.HasIndex("IsSoftDeleted");

                    b.ToTable("Cards");
                });

            modelBuilder.Entity("Models.Db.Tree.CardConnection", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long>("CardLeftId")
                        .HasColumnType("bigint");

                    b.Property<long>("CardRightId")
                        .HasColumnType("bigint");

                    b.Property<long?>("DeskId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsSoftDeleted")
                        .HasColumnType("boolean");

                    b.HasKey("Id", "CardLeftId", "CardRightId");

                    b.HasIndex("CardLeftId");

                    b.HasIndex("CardRightId");

                    b.HasIndex("DeskId");

                    b.HasIndex("IsSoftDeleted");

                    b.ToTable("CardConnections");
                });

            modelBuilder.Entity("Models.Db.Tree.Desk", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long>("AuthorAccountId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<bool>("IsInTrashBin")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsSoftDeleted")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastUpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("ParentId")
                        .HasColumnType("bigint");

                    b.Property<string>("Title")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorAccountId");

                    b.HasIndex("IsSoftDeleted");

                    b.HasIndex("ParentId");

                    b.ToTable("Desks");
                });

            modelBuilder.Entity("Models.Db.Tree.DeskActionHistoryItem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("Action")
                        .HasColumnType("integer");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("DeskId")
                        .HasColumnType("bigint");

                    b.Property<long>("FunAccountId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsSoftDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("NewData")
                        .HasColumnType("text");

                    b.Property<string>("OldData")
                        .HasColumnType("text");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("DeskId");

                    b.HasIndex("FunAccountId");

                    b.HasIndex("IsSoftDeleted");

                    b.ToTable("DeskActionHistoryItem");
                });

            modelBuilder.Entity("Models.Db.Tree.Folder", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long>("AuthorAccountId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsInTrashBin")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsSoftDeleted")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastUpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long?>("ParentId")
                        .HasColumnType("bigint");

                    b.Property<string>("Title")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorAccountId");

                    b.HasIndex("IsSoftDeleted");

                    b.HasIndex("ParentId");

                    b.ToTable("Folders");
                });

            modelBuilder.Entity("Models.Db.Relations.DeskShare", b =>
                {
                    b.HasOne("Models.Db.Tree.Desk", "Desk")
                        .WithMany("SharedToRelation")
                        .HasForeignKey("DeskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.Db.Account.FunAccount", "FunAccount")
                        .WithMany("SharedDesksRelation")
                        .HasForeignKey("FunAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Desk");

                    b.Navigation("FunAccount");
                });

            modelBuilder.Entity("Models.Db.Relations.FolderShare", b =>
                {
                    b.HasOne("Models.Db.Tree.Folder", "Folder")
                        .WithMany("SharedToRelation")
                        .HasForeignKey("FolderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.Db.Account.FunAccount", "FunAccount")
                        .WithMany("SharedFoldersRelation")
                        .HasForeignKey("FunAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Folder");

                    b.Navigation("FunAccount");
                });

            modelBuilder.Entity("Models.Db.Sessions.TokenSession", b =>
                {
                    b.HasOne("Models.Db.Account.FunAccount", "FunAccount")
                        .WithMany("TokenSessions")
                        .HasForeignKey("FunAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FunAccount");
                });

            modelBuilder.Entity("Models.Db.Tree.Card", b =>
                {
                    b.HasOne("Models.Db.Tree.Desk", "Desk")
                        .WithMany("Cards")
                        .HasForeignKey("DeskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Desk");
                });

            modelBuilder.Entity("Models.Db.Tree.CardConnection", b =>
                {
                    b.HasOne("Models.Db.Tree.Card", "CardLeft")
                        .WithMany("AsLeftCardConnections")
                        .HasForeignKey("CardLeftId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.Db.Tree.Card", "CardRight")
                        .WithMany("AsRightCardConnections")
                        .HasForeignKey("CardRightId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.Db.Tree.Desk", null)
                        .WithMany("CardConnections")
                        .HasForeignKey("DeskId");

                    b.Navigation("CardLeft");

                    b.Navigation("CardRight");
                });

            modelBuilder.Entity("Models.Db.Tree.Desk", b =>
                {
                    b.HasOne("Models.Db.Account.FunAccount", "AuthorAccount")
                        .WithMany("AuthoredDesks")
                        .HasForeignKey("AuthorAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.Db.Tree.Folder", "Parent")
                        .WithMany("Desks")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AuthorAccount");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("Models.Db.Tree.DeskActionHistoryItem", b =>
                {
                    b.HasOne("Models.Db.Tree.Desk", "Desk")
                        .WithMany("HistoryItems")
                        .HasForeignKey("DeskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.Db.Account.FunAccount", "FunAccount")
                        .WithMany("FiredActions")
                        .HasForeignKey("FunAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Desk");

                    b.Navigation("FunAccount");
                });

            modelBuilder.Entity("Models.Db.Tree.Folder", b =>
                {
                    b.HasOne("Models.Db.Account.FunAccount", "AuthorAccount")
                        .WithMany("AuthoredFolders")
                        .HasForeignKey("AuthorAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.Db.Tree.Folder", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.Navigation("AuthorAccount");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("Models.Db.Account.FunAccount", b =>
                {
                    b.Navigation("AuthoredDesks");

                    b.Navigation("AuthoredFolders");

                    b.Navigation("FiredActions");

                    b.Navigation("SharedDesksRelation");

                    b.Navigation("SharedFoldersRelation");

                    b.Navigation("TokenSessions");
                });

            modelBuilder.Entity("Models.Db.Tree.Card", b =>
                {
                    b.Navigation("AsLeftCardConnections");

                    b.Navigation("AsRightCardConnections");
                });

            modelBuilder.Entity("Models.Db.Tree.Desk", b =>
                {
                    b.Navigation("CardConnections");

                    b.Navigation("Cards");

                    b.Navigation("HistoryItems");

                    b.Navigation("SharedToRelation");
                });

            modelBuilder.Entity("Models.Db.Tree.Folder", b =>
                {
                    b.Navigation("Children");

                    b.Navigation("Desks");

                    b.Navigation("SharedToRelation");
                });
#pragma warning restore 612, 618
        }
    }
}
