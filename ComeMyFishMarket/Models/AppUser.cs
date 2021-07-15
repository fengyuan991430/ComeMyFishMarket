using ComeMyFishMarket.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComeMyFishMarket.Models
{
    public class AppUser
    {
        public int ID { get; set; }
        public string RegUserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public void AddNewUser(String userid, String username, String email, String role)
        {
            var scope = Program.host.Services.CreateScope();
            var service = scope.ServiceProvider;
            using (var context = new ComeMyFishMarketClassContext(
                service.GetRequiredService<DbContextOptions<ComeMyFishMarketClassContext>>()))
            {
                var appuser = from m in context.AppUser select m;
                if(!userid.Equals(""))
                {
                    appuser = appuser.Where(s => s.RegUserID.Equals(userid));
                    if(!appuser.Any())
                    {
                        context.AppUser.AddRange(
                            new AppUser
                            {
                                RegUserID = userid,
                                Username = username,
                                Email = email,
                                Role = role
                            }
                            );
                    }
                }
                context.SaveChanges();
            }
        }
    }
}
