using Microsoft.EntityFrameworkCore.Migrations;

namespace ComeMyFishMarket.Migrations.ComeMyFishMarketClass
{
    public partial class UpdateWalletHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HistoryAmount",
                table: "WalletHistory",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HistoryAmount",
                table: "WalletHistory");
        }
    }
}
