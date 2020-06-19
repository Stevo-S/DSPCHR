using Microsoft.EntityFrameworkCore.Migrations;

namespace DSPCHR.Data.PostgreSql.Migrations
{
    public partial class AddPostBackUrlToWebActivators : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PostBackUrl",
                table: "WebActivators",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostBackUrl",
                table: "WebActivators");
        }
    }
}
