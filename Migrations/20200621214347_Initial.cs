using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VoteMyst.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DisplayID = table.Column<string>(type: "VARCHAR(16)", nullable: false),
                    URL = table.Column<string>(maxLength: 32, nullable: true),
                    Title = table.Column<string>(maxLength: 64, nullable: false),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    EventType = table.Column<int>(nullable: false),
                    Settings = table.Column<ulong>(nullable: false),
                    RevealDate = table.Column<DateTime>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    SubmissionEndDate = table.Column<DateTime>(nullable: false),
                    VoteEndDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserAccounts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DisplayID = table.Column<string>(type: "VARCHAR(28)", nullable: false),
                    Username = table.Column<string>(maxLength: 32, nullable: false),
                    JoinDate = table.Column<DateTime>(nullable: false),
                    Permissions = table.Column<ulong>(nullable: false),
                    AccountBadge = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccounts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Authorizations",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<int>(nullable: false),
                    ServiceUserID = table.Column<string>(type: "VARCHAR(64)", nullable: false),
                    Service = table.Column<int>(nullable: false),
                    Valid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authorizations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Authorizations_UserAccounts_UserID",
                        column: x => x.UserID,
                        principalTable: "UserAccounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Entries",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DisplayID = table.Column<string>(type: "VARCHAR(16)", nullable: false),
                    EventID = table.Column<int>(nullable: false),
                    AuthorID = table.Column<int>(nullable: false),
                    SubmitDate = table.Column<DateTime>(nullable: false),
                    EntryType = table.Column<int>(nullable: false),
                    Content = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entries", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Entries_UserAccounts_AuthorID",
                        column: x => x.AuthorID,
                        principalTable: "UserAccounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Entries_Events_EventID",
                        column: x => x.EventID,
                        principalTable: "Events",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventPermissionModifiers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<int>(nullable: false),
                    EventID = table.Column<int>(nullable: false),
                    Permissions = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventPermissionModifiers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EventPermissionModifiers_Events_EventID",
                        column: x => x.EventID,
                        principalTable: "Events",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventPermissionModifiers_UserAccounts_UserID",
                        column: x => x.UserID,
                        principalTable: "UserAccounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ReportAuthorID = table.Column<int>(nullable: true),
                    EntryAuthorID = table.Column<int>(nullable: true),
                    EventID = table.Column<int>(nullable: true),
                    EntryID = table.Column<int>(nullable: true),
                    Reason = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Reports_UserAccounts_EntryAuthorID",
                        column: x => x.EntryAuthorID,
                        principalTable: "UserAccounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reports_Entries_EntryID",
                        column: x => x.EntryID,
                        principalTable: "Entries",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reports_Events_EventID",
                        column: x => x.EventID,
                        principalTable: "Events",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reports_UserAccounts_ReportAuthorID",
                        column: x => x.ReportAuthorID,
                        principalTable: "UserAccounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Votes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<int>(nullable: false),
                    EntryID = table.Column<int>(nullable: false),
                    VoteDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Votes_Entries_EntryID",
                        column: x => x.EntryID,
                        principalTable: "Entries",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Votes_UserAccounts_UserID",
                        column: x => x.UserID,
                        principalTable: "UserAccounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Authorizations_UserID",
                table: "Authorizations",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_AuthorID",
                table: "Entries",
                column: "AuthorID");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_DisplayID",
                table: "Entries",
                column: "DisplayID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Entries_EventID",
                table: "Entries",
                column: "EventID");

            migrationBuilder.CreateIndex(
                name: "IX_EventPermissionModifiers_EventID",
                table: "EventPermissionModifiers",
                column: "EventID");

            migrationBuilder.CreateIndex(
                name: "IX_EventPermissionModifiers_UserID",
                table: "EventPermissionModifiers",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Events_DisplayID",
                table: "Events",
                column: "DisplayID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_EntryAuthorID",
                table: "Reports",
                column: "EntryAuthorID");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_EntryID",
                table: "Reports",
                column: "EntryID");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_EventID",
                table: "Reports",
                column: "EventID");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportAuthorID",
                table: "Reports",
                column: "ReportAuthorID");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_DisplayID",
                table: "UserAccounts",
                column: "DisplayID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_EntryID",
                table: "Votes",
                column: "EntryID");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_UserID",
                table: "Votes",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Authorizations");

            migrationBuilder.DropTable(
                name: "EventPermissionModifiers");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "Votes");

            migrationBuilder.DropTable(
                name: "Entries");

            migrationBuilder.DropTable(
                name: "UserAccounts");

            migrationBuilder.DropTable(
                name: "Events");
        }
    }
}
