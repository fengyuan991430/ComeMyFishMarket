﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComeMyFishMarket.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ComeMyFishMarket.Data
{
    public class ComeMyFishMarketContext : IdentityDbContext<ComeMyFishMarketUser>
    {
        public ComeMyFishMarketContext(DbContextOptions<ComeMyFishMarketContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
