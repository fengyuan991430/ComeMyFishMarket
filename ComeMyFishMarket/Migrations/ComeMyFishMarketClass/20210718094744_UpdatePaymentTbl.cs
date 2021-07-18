using Microsoft.EntityFrameworkCore.Migrations;

namespace ComeMyFishMarket.Migrations.ComeMyFishMarketClass
{
    public partial class UpdatePaymentTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MarketOrderID",
                table: "Payment",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarketOrderID",
                table: "Payment");
        }
    }
}
