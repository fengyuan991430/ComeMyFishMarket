using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ComeMyFishMarket.Data; //Can find the context class to check the table connection
using ComeMyFishMarket.Models;
using ComeMyFishMarket.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace ComeMyFishMarket
{
    public class Program
    {
        public static IHost host;
        public async static Task Main(string[] args)
        {
            host = CreateHostBuilder(args).Build();
            //onhost = host;
            using (var scope = host.Services.CreateScope())
            {
                var service = scope.ServiceProvider;
                try
                {
                    var context = service.GetRequiredService<ComeMyFishMarketContext>();
                    var userManager = service.GetRequiredService<UserManager<ComeMyFishMarketUser>>();
                    context.Database.Migrate();
                    await SeedData.Initialize(userManager);
                }
                catch (Exception ex)
                {
                    var logger = service.GetRequiredService<Logger<Program>>();
                    logger.LogError(ex, "Error Found");
                }
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
