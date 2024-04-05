using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SlothParlor.MediaJournal.Data.Migrations
{
    /// <inheritdoc />
    public partial class Fix_ModelQuirks1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MediaLogEntryId",
                schema: "MediaJournal",
                table: "Entries",
                newName: "EntryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EntryId",
                schema: "MediaJournal",
                table: "Entries",
                newName: "MediaLogEntryId");
        }
    }
}
