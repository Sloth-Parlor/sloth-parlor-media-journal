using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SlothParlor.MediaJournal.Data.Migrations
{
    /// <inheritdoc />
    public partial class DbTroubleshootingTweaks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserWatchGroup",
                schema: "MediaJournal");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserWatchGroup",
                schema: "MediaJournal",
                columns: table => new
                {
                    OwnersUserId = table.Column<string>(type: "text", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_UserWatchGroup_WatchGroupId",
                schema: "MediaJournal",
                table: "UserWatchGroup",
                column: "WatchGroupId");
        }
    }
}
