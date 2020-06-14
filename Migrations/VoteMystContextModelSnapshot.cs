﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VoteMyst.Database;

namespace VoteMyst.Migrations
{
    [DbContext(typeof(VoteMystContext))]
    partial class VoteMystContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("VoteMyst.Database.Authorization", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Service")
                        .HasColumnType("int");

                    b.Property<string>("ServiceUserID")
                        .IsRequired()
                        .HasColumnType("VARCHAR(64)");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.Property<bool>("Valid")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("Authorizations");
                });

            modelBuilder.Entity("VoteMyst.Database.Entry", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("AuthorID")
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("DisplayID")
                        .IsRequired()
                        .HasColumnType("VARCHAR(16)");

                    b.Property<int>("EntryType")
                        .HasColumnType("int");

                    b.Property<int>("EventID")
                        .HasColumnType("int");

                    b.Property<DateTime>("SubmitDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("ID");

                    b.HasIndex("AuthorID");

                    b.HasIndex("DisplayID")
                        .IsUnique();

                    b.HasIndex("EventID");

                    b.ToTable("Entries");
                });

            modelBuilder.Entity("VoteMyst.Database.Event", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("varchar(512) CHARACTER SET utf8mb4")
                        .HasMaxLength(512);

                    b.Property<string>("DisplayID")
                        .IsRequired()
                        .HasColumnType("VARCHAR(16)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("EventType")
                        .HasColumnType("int");

                    b.Property<DateTime>("RevealDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("varchar(64) CHARACTER SET utf8mb4")
                        .HasMaxLength(64);

                    b.Property<string>("URL")
                        .HasColumnType("varchar(32) CHARACTER SET utf8mb4")
                        .HasMaxLength(32);

                    b.Property<DateTime>("VoteEndDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("ID");

                    b.HasIndex("DisplayID")
                        .IsUnique();

                    b.ToTable("Events");
                });

            modelBuilder.Entity("VoteMyst.Database.EventPermissionModifier", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("EventID")
                        .HasColumnType("int");

                    b.Property<ulong>("Permissions")
                        .HasColumnType("bigint unsigned");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("EventID");

                    b.HasIndex("UserID");

                    b.ToTable("EventPermissionModifiers");
                });

            modelBuilder.Entity("VoteMyst.Database.Report", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("EntryID")
                        .HasColumnType("int");

                    b.Property<string>("Reason")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("EntryID");

                    b.HasIndex("UserID");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("VoteMyst.Database.UserAccount", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("AccountBadge")
                        .HasColumnType("int");

                    b.Property<string>("DisplayID")
                        .IsRequired()
                        .HasColumnType("VARCHAR(28)");

                    b.Property<DateTime>("JoinDate")
                        .HasColumnType("datetime(6)");

                    b.Property<ulong>("Permissions")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("ID");

                    b.HasIndex("DisplayID")
                        .IsUnique();

                    b.ToTable("UserAccounts");
                });

            modelBuilder.Entity("VoteMyst.Database.Vote", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("EntryID")
                        .HasColumnType("int");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.Property<DateTime>("VoteDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("ID");

                    b.HasIndex("EntryID");

                    b.HasIndex("UserID");

                    b.ToTable("Votes");
                });

            modelBuilder.Entity("VoteMyst.Database.Authorization", b =>
                {
                    b.HasOne("VoteMyst.Database.UserAccount", "User")
                        .WithMany("Authorizations")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VoteMyst.Database.Entry", b =>
                {
                    b.HasOne("VoteMyst.Database.UserAccount", "Author")
                        .WithMany("Entries")
                        .HasForeignKey("AuthorID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VoteMyst.Database.Event", "Event")
                        .WithMany("Entries")
                        .HasForeignKey("EventID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VoteMyst.Database.EventPermissionModifier", b =>
                {
                    b.HasOne("VoteMyst.Database.Event", "Event")
                        .WithMany()
                        .HasForeignKey("EventID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VoteMyst.Database.UserAccount", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VoteMyst.Database.Report", b =>
                {
                    b.HasOne("VoteMyst.Database.Entry", "Entry")
                        .WithMany("Reports")
                        .HasForeignKey("EntryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VoteMyst.Database.UserAccount", "User")
                        .WithMany("AuthoredReports")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VoteMyst.Database.Vote", b =>
                {
                    b.HasOne("VoteMyst.Database.Entry", "Entry")
                        .WithMany("Votes")
                        .HasForeignKey("EntryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VoteMyst.Database.UserAccount", "User")
                        .WithMany("AuthoredVotes")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
