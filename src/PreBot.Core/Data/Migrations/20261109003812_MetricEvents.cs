using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PreBot.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class MetricEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReleaseReason",
                table: "ListingSlots",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MetricEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Kind = table.Column<int>(type: "INTEGER", nullable: false),
                    CommunityId = table.Column<long>(type: "INTEGER", nullable: false),
                    ListingId = table.Column<int>(type: "INTEGER", nullable: false),
                    Wave = table.Column<int>(type: "INTEGER", nullable: true),
                    Value = table.Column<int>(type: "INTEGER", nullable: true),
                    Reason = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetricEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MetricEvents_CommunityId_Kind_AtUtc",
                table: "MetricEvents",
                columns: new[] { "CommunityId", "Kind", "AtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_MetricEvents_ListingId",
                table: "MetricEvents",
                column: "ListingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MetricEvents");

            migrationBuilder.DropColumn(
                name: "ReleaseReason",
                table: "ListingSlots");
        }
    }
}
