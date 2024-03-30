using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SlothParlor.MediaJournal.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "MediaJournal");

            migrationBuilder.EnsureSchema(
                name: "Users");

            migrationBuilder.CreateTable(
                name: "User",
                schema: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Provider = table.Column<string>(type: "text", nullable: false),
                    ObjectId = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "WatchGroups",
                schema: "MediaJournal",
                columns: table => new
                {
                    WatchGroupId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DisplayName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchGroups", x => x.WatchGroupId);
                });

            migrationBuilder.CreateTable(
                name: "MediaLogs",
                schema: "MediaJournal",
                columns: table => new
                {
                    MediaLogId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WatchGroupId = table.Column<int>(type: "integer", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaLogs", x => x.MediaLogId);
                    table.ForeignKey(
                        name: "FK_MediaLogs_WatchGroups_WatchGroupId",
                        column: x => x.WatchGroupId,
                        principalSchema: "MediaJournal",
                        principalTable: "WatchGroups",
                        principalColumn: "WatchGroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserWatchGroup",
                schema: "MediaJournal",
                columns: table => new
                {
                    OwnersUserId = table.Column<int>(type: "integer", nullable: false),
                    WatchGroupId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWatchGroup", x => new { x.OwnersUserId, x.WatchGroupId });
                    table.ForeignKey(
                        name: "FK_UserWatchGroup_User_OwnersUserId",
                        column: x => x.OwnersUserId,
                        principalSchema: "Users",
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserWatchGroup_WatchGroups_WatchGroupId",
                        column: x => x.WatchGroupId,
                        principalSchema: "MediaJournal",
                        principalTable: "WatchGroups",
                        principalColumn: "WatchGroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WatchGroupParticipants",
                schema: "MediaJournal",
                columns: table => new
                {
                    ParticipantId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    WatchGroupId = table.Column<int>(type: "integer", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchGroupParticipants", x => x.ParticipantId);
                    table.ForeignKey(
                        name: "FK_WatchGroupParticipants_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "Users",
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WatchGroupParticipants_WatchGroups_WatchGroupId",
                        column: x => x.WatchGroupId,
                        principalSchema: "MediaJournal",
                        principalTable: "WatchGroups",
                        principalColumn: "WatchGroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Entries",
                schema: "MediaJournal",
                columns: table => new
                {
                    MediaLogEntryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MediaLogId = table.Column<int>(type: "integer", nullable: false),
                    CandidateName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entries", x => x.MediaLogEntryId);
                    table.ForeignKey(
                        name: "FK_Entries_MediaLogs_MediaLogId",
                        column: x => x.MediaLogId,
                        principalSchema: "MediaJournal",
                        principalTable: "MediaLogs",
                        principalColumn: "MediaLogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntryAttendees",
                schema: "MediaJournal",
                columns: table => new
                {
                    EntryAttendeeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntryId = table.Column<int>(type: "integer", nullable: false),
                    ParticipantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryAttendees", x => x.EntryAttendeeId);
                    table.ForeignKey(
                        name: "FK_EntryAttendees_Entries_EntryId",
                        column: x => x.EntryId,
                        principalSchema: "MediaJournal",
                        principalTable: "Entries",
                        principalColumn: "MediaLogEntryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntryAttendees_WatchGroupParticipants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalSchema: "MediaJournal",
                        principalTable: "WatchGroupParticipants",
                        principalColumn: "ParticipantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Entries_MediaLogId",
                schema: "MediaJournal",
                table: "Entries",
                column: "MediaLogId");

            migrationBuilder.CreateIndex(
                name: "IX_EntryAttendees_EntryId",
                schema: "MediaJournal",
                table: "EntryAttendees",
                column: "EntryId");

            migrationBuilder.CreateIndex(
                name: "IX_EntryAttendees_ParticipantId",
                schema: "MediaJournal",
                table: "EntryAttendees",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaLogs_WatchGroupId",
                schema: "MediaJournal",
                table: "MediaLogs",
                column: "WatchGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWatchGroup_WatchGroupId",
                schema: "MediaJournal",
                table: "UserWatchGroup",
                column: "WatchGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_WatchGroupParticipants_UserId",
                schema: "MediaJournal",
                table: "WatchGroupParticipants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WatchGroupParticipants_WatchGroupId",
                schema: "MediaJournal",
                table: "WatchGroupParticipants",
                column: "WatchGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntryAttendees",
                schema: "MediaJournal");

            migrationBuilder.DropTable(
                name: "UserWatchGroup",
                schema: "MediaJournal");

            migrationBuilder.DropTable(
                name: "Entries",
                schema: "MediaJournal");

            migrationBuilder.DropTable(
                name: "WatchGroupParticipants",
                schema: "MediaJournal");

            migrationBuilder.DropTable(
                name: "MediaLogs",
                schema: "MediaJournal");

            migrationBuilder.DropTable(
                name: "User",
                schema: "Users");

            migrationBuilder.DropTable(
                name: "WatchGroups",
                schema: "MediaJournal");
        }
    }
}
