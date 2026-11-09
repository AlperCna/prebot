using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PreBot.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class LobbyPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LobbyPassword",
                table: "Listings",
                type: "TEXT",
                nullable: true);

            // Locked lobbies opened before this change used the code as the password: keep them working.
            migrationBuilder.Sql("UPDATE Listings SET LobbyPassword = LobbyCode WHERE LobbyLocked = 1 AND LobbyCode IS NOT NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LobbyPassword",
                table: "Listings");
        }
    }
}
