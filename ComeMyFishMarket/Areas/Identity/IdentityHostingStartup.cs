using System;
using ComeMyFishMarket.Areas.Identity.Data;
using ComeMyFishMarket.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(ComeMyFishMarket.Areas.Identity.IdentityHostingStartup))]
namespace ComeMyFishMarket.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<ComeMyFishMarketContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("ComeMyFishMarketContextConnection")));

                services.AddDefaultIdentity<ComeMyFishMarketUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<ComeMyFishMarketContext>();
            });
        }
    }
}