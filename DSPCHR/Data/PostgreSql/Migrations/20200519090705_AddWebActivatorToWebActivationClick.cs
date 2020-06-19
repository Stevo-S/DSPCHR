using Microsoft.EntityFrameworkCore.Migrations;

namespace DSPCHR.Data.PostgreSql.Migrations
{
    public partial class AddWebActivatorToWebActivationClick : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "WebActivatorId",
                table: "WebActivationClicks",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_WebActivationClicks_WebActivatorId",
                table: "WebActivationClicks",
                column: "WebActivatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_WebActivationClicks_WebActivators_WebActivatorId",
                table: "WebActivationClicks",
                column: "WebActivatorId",
                principalTable: "WebActivators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WebActivationClicks_WebActivators_WebActivatorId",
                table: "WebActivationClicks");

            migrationBuilder.DropIndex(
                name: "IX_WebActivationClicks_WebActivatorId",
                table: "WebActivationClicks");

            migrationBuilder.DropColumn(
                name: "WebActivatorId",
                table: "WebActivationClicks");
        }
    }
}
