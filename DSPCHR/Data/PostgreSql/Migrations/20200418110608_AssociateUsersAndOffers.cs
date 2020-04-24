using Microsoft.EntityFrameworkCore.Migrations;

namespace DSPCHR.Data.PostgreSql.Migrations
{
    public partial class AssociateUsersAndOffers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationUserOffer",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(nullable: false),
                    OfferId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserOffer", x => new { x.ApplicationUserId, x.OfferId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserOffer_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserOffer_Offers_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserOffer_OfferId",
                table: "ApplicationUserOffer",
                column: "OfferId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserOffer");
        }
    }
}
