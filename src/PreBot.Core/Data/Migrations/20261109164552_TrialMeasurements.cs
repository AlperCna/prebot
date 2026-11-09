using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PreBot.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class TrialMeasurements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InGameListingId",
                table: "Players",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InGameUntilUtc",
                table: "Players",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SinceNotifiedSeconds",
                table: "MetricEvents",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "MetricEvents",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OpenedAtUtc",
                table: "ListingNotifications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "ListingNotifications",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InGameListingId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "InGameUntilUtc",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "SinceNotifiedSeconds",
                table: "MetricEvents");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "MetricEvents");

            migrationBuilder.DropColumn(
                name: "OpenedAtUtc",
                table: "ListingNotifications");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "ListingNotifications");
        }
    }
}
