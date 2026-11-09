using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PreBot.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class SlotDeclaredRank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeclaredRank",
                table: "ListingSlots",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeclaredRole",
                table: "ListingSlots",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeclaredRank",
                table: "ListingSlots");

            migrationBuilder.DropColumn(
                name: "DeclaredRole",
                table: "ListingSlots");
        }
    }
}
