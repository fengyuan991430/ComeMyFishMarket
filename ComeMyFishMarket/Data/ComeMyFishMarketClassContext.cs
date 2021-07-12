using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ComeMyFishMarket.Models;

namespace ComeMyFishMarket.Data
{
    public class ComeMyFishMarketClassContext : DbContext
    {
        public ComeMyFishMarketClassContext (DbContextOptions<ComeMyFishMarketClassContext> options)
            : base(options)
        {
        }

        public DbSet<ComeMyFishMarket.Models.MarketOrder> MarketOrder { get; set; }

        public DbSet<ComeMyFishMarket.Models.OrderItem> OrderItem { get; set; }

        public DbSet<ComeMyFishMarket.Models.Payment> Payment { get; set; }

        public DbSet<ComeMyFishMarket.Models.Product> Product { get; set; }
    }
}
