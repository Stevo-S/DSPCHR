using Microsoft.EntityFrameworkCore.Migrations;

namespace DSPCHR.Data.PostgreSql.Migrations
{
    public partial class AddUssdToWebActivators : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ForwardClickId",
                table: "WebActivators",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UssdFallBack",
                table: "WebActivators",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForwardClickId",
                table: "WebActivators");

            migrationBuilder.DropColumn(
                name: "UssdFallBack",
                table: "WebActivators");
        }
    }
}
