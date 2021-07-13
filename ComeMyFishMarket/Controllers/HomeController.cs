using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ComeMyFishMarket.Models;
using ComeMyFishMarket.Data;

namespace ComeMyFishMarket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ComeMyFishMarketClassContext _context;

        public HomeController(ILogger<HomeController> logger, ComeMyFishMarketClassContext context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index(string keyword)
        {
            var product = from m in _context.Product select m;

            if (!string.IsNullOrEmpty(keyword))
            {
                product = product.Where(s => s.ProductName.Contains(keyword) || s.Category.Contains(keyword));
            }

            return View(product.Where(s => s.Quantity > 0 && s.ProductStatus == "Active").ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
