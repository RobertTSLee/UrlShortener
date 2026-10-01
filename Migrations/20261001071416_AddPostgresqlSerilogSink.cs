using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortener.Migrations
{
    /// <inheritdoc />
    public partial class AddPostgresqlSerilogSink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserUrlMappings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 10, 1, 7, 14, 15, 524, DateTimeKind.Utc).AddTicks(3798),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 10, 1, 7, 11, 1, 122, DateTimeKind.Utc).AddTicks(7694));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 10, 1, 7, 14, 15, 524, DateTimeKind.Utc).AddTicks(2524),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 10, 1, 7, 11, 1, 122, DateTimeKind.Utc).AddTicks(6251));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Urls",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 10, 1, 7, 14, 15, 520, DateTimeKind.Utc).AddTicks(2821),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 10, 1, 7, 11, 1, 118, DateTimeKind.Utc).AddTicks(4997));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeStamp",
                table: "Logs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 10, 1, 7, 14, 15, 529, DateTimeKind.Utc).AddTicks(1870),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 10, 1, 7, 11, 1, 128, DateTimeKind.Utc).AddTicks(1980));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserUrlMappings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 10, 1, 7, 11, 1, 122, DateTimeKind.Utc).AddTicks(7694),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 10, 1, 7, 14, 15, 524, DateTimeKind.Utc).AddTicks(3798));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 10, 1, 7, 11, 1, 122, DateTimeKind.Utc).AddTicks(6251),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 10, 1, 7, 14, 15, 524, DateTimeKind.Utc).AddTicks(2524));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Urls",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 10, 1, 7, 11, 1, 118, DateTimeKind.Utc).AddTicks(4997),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 10, 1, 7, 14, 15, 520, DateTimeKind.Utc).AddTicks(2821));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeStamp",
                table: "Logs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 10, 1, 7, 11, 1, 128, DateTimeKind.Utc).AddTicks(1980),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 10, 1, 7, 14, 15, 529, DateTimeKind.Utc).AddTicks(1870));
        }
    }
}
