using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UrlShortener.Migrations
{
    /// <inheritdoc />
    public partial class AddLoggingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserUrlMappings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 10, 1, 7, 11, 1, 122, DateTimeKind.Utc).AddTicks(7694),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 10, 1, 6, 54, 42, 405, DateTimeKind.Utc).AddTicks(3597));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 10, 1, 7, 11, 1, 122, DateTimeKind.Utc).AddTicks(6251),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 10, 1, 6, 54, 42, 405, DateTimeKind.Utc).AddTicks(2102));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Urls",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 10, 1, 7, 11, 1, 118, DateTimeKind.Utc).AddTicks(4997),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 10, 1, 6, 54, 42, 401, DateTimeKind.Utc).AddTicks(5740));

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2026, 10, 1, 7, 11, 1, 128, DateTimeKind.Utc).AddTicks(1980)),
                    Level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MessageTemplate = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: true),
                    Exception = table.Column<string>(type: "text", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    SourceContext = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RequestId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Logs_Level",
                table: "Logs",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_RequestId",
                table: "Logs",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_SourceContext",
                table: "Logs",
                column: "SourceContext");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_TimeStamp",
                table: "Logs",
                column: "TimeStamp",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UserId",
                table: "Logs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserUrlMappings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 10, 1, 6, 54, 42, 405, DateTimeKind.Utc).AddTicks(3597),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 10, 1, 7, 11, 1, 122, DateTimeKind.Utc).AddTicks(7694));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 10, 1, 6, 54, 42, 405, DateTimeKind.Utc).AddTicks(2102),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 10, 1, 7, 11, 1, 122, DateTimeKind.Utc).AddTicks(6251));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Urls",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 10, 1, 6, 54, 42, 401, DateTimeKind.Utc).AddTicks(5740),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 10, 1, 7, 11, 1, 118, DateTimeKind.Utc).AddTicks(4997));
        }
    }
}
