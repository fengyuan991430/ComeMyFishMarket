using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ComeMyFishMarket.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the ComeMyFishMarketUser class
    public class ComeMyFishMarketUser : IdentityUser
    {
        [PersonalData]
        public string Role { get; set; }

        [PersonalData]
        public double UserWallet { get; set; }

        [PersonalData]
        public string Address { get; set; }

    }
}
