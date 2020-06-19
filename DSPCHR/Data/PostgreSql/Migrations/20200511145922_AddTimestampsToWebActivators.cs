using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DSPCHR.Data.PostgreSql.Migrations
{
    public partial class AddTimestampsToWebActivators : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "WebActivators",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "WebActivators",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "WebActivators");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "WebActivators");
        }
    }
}
