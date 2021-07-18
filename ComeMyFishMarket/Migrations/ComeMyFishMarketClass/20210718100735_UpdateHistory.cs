using Microsoft.EntityFrameworkCore.Migrations;

namespace ComeMyFishMarket.Migrations.ComeMyFishMarketClass
{
    public partial class UpdateHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "WalletHistory",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserID",
                table: "WalletHistory");
        }
    }
}
