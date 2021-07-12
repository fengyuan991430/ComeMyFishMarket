using System.Linq;
using System.Threading.Tasks;
using ComeMyFishMarket.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace ComeMyFishMarket.Models
{
    public class SeedData
    {
        public static async Task Initialize(UserManager<ComeMyFishMarketUser> userManager)
        {
            var Admin = new ComeMyFishMarketUser
            {
                UserName = "Admin",
                Email = "ComeMyFishMarket@gmail.com",
                Role = "Admin",
                UserWallet = 0,
                EmailConfirmed = true
            };
            if(userManager.Users.All(u => u.Id != Admin.Id))
            {
                var user = await userManager.FindByEmailAsync(Admin.Email);
                if(user == null)
                {
                    await userManager.CreateAsync(Admin, "Admin@1234");
                }
            }
        }
    }
}
