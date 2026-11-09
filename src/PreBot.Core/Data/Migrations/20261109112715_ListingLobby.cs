using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PreBot.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class ListingLobby : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LobbyCode",
                table: "Listings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LobbyLocked",
                table: "Listings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LobbyCode",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "LobbyLocked",
                table: "Listings");
        }
    }
}
