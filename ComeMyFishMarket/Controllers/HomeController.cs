using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ComeMyFishMarket.Models;
using ComeMyFishMarket.Data;
using Microsoft.AspNetCore.Identity;
using ComeMyFishMarket.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;

namespace ComeMyFishMarket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ComeMyFishMarketClassContext _context;

        private readonly UserManager<ComeMyFishMarketUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ComeMyFishMarketClassContext context, UserManager<ComeMyFishMarketUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult Index(string keyword)
        {
            var product = from m in _context.Product select m;

            if (!string.IsNullOrEmpty(keyword))
            {
                product = product.Where(s => s.ProductName.Contains(keyword) || s.Category.Contains(keyword));
            }
            var result = _context.ShoppingCart.Where(x => x.CustomerId == _userManager.GetUserId(User)).ToList();
            int count = result.Count;
            ViewBag.CartItem = result;
            ViewBag.Cartcount = count;
            return View(product.Where(s => s.Quantity > 0 || s.ProductStatus == "Active").ToList());
        }

        [Authorize]
        public IActionResult AddCart(int id)
        {
            Product p = _context.Product.FirstOrDefault(x => x.ProductID == id);
            if (p != null && p.Quantity > 0)
            {
                ShoppingCart cart = _context.ShoppingCart.FirstOrDefault(x => x.ProductId == id);
                if (cart != null)
                {
                    cart.Quantity++;
                }
                else
                {
                    ShoppingCart cart1 = new ShoppingCart();
                    cart1.ProductId = p.ProductID;
                    cart1.ProductName = p.ProductName;
                    cart1.Quantity = 1;
                    cart1.Price = p.Price;
                    cart1.ProductImage = p.ProductImage;
                    cart1.CustomerId = _userManager.GetUserId(User);
                    cart1.SellerId = p.UserID;
                    _context.ShoppingCart.Add(cart1);
                }
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
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
