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
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;

namespace ComeMyFishMarket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ComeMyFishMarketClassContext _context;

        private readonly UserManager<ComeMyFishMarketUser> _userManager;

        private readonly ComeMyFishMarketContext _context1;

        public HomeController(ILogger<HomeController> logger, ComeMyFishMarketClassContext context, UserManager<ComeMyFishMarketUser> userManager, ComeMyFishMarketContext context1)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _context1 = context1;
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
            return View(product.Where(s => s.Quantity > 0 && s.ProductStatus == "Active").ToList());
        }

        [Authorize]
        public IActionResult AddCart(int id)
        {
            var user = _context1.Users.FirstOrDefault(x => x.Id == _userManager.GetUserId(User));

            if (user.Role != "Customer")
            {
                TempData["Validate"] = "Only Customer Can Add To Cart!";
                return RedirectToAction("Index", "Home");
            }

            Product p = _context.Product.FirstOrDefault(x => x.ProductID == id);
            if (p != null && p.Quantity > 0)
            {
                ShoppingCart cart = _context.ShoppingCart.FirstOrDefault(x => x.ProductId == id && x.CustomerId == user.Id);
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

            TempData["Add"] = "1 Unit " + p.ProductName +" Product Successfully Added To Cart!";
            return RedirectToAction("Index");
        }

        public IActionResult RemoveUnit(int id)
        {
            var cartitem = _context.ShoppingCart.FirstOrDefault(x => x.ShoppingCartID == id);
            if(cartitem!= null)
            {
                if(cartitem.Quantity > 1)
                {
                    cartitem.Quantity--;
                }
                else
                {
                    _context.ShoppingCart.Remove(cartitem);
                }
                _context.SaveChanges();
            }

            TempData["Remove"] = "1 Unit " + cartitem.ProductName + " Product Successfully Removed From Cart!";
            return RedirectToAction("Index");
        }

        public IActionResult TopUp()
        {
            var user = _context1.Users.FirstOrDefault(x=>x.Id == _userManager.GetUserId(User));
            return View(user);
        }

        [HttpPost]
        public IActionResult TopUp(double topup)
        {            
            var user = _context1.Users.FirstOrDefault(x => x.Id == _userManager.GetUserId(User));
            user.UserWallet += topup;
            _context1.SaveChanges();
            WalletHistory topuphis = new WalletHistory
            {
                HistoryDesc = "Top Up",
                HistoryAmount = "+" + topup.ToString(),
                HistoryDate = DateTime.Now,
                UserID = user.Id
            };
            _context.WalletHistory.Add(topuphis);
            _context.SaveChanges();
            return RedirectToAction("TopUp");
        }

        public IActionResult DownloadImage(string id)
        {
            BlobManager b = new BlobManager();
            CloudBlobContainer container = b.Container();
            CloudBlockBlob blob = container.GetBlockBlobReference(id);
            Stream blobStream = blob.OpenReadAsync().Result;
            return File(blobStream, blob.Properties.ContentType, blob.Name);
        }

        public IActionResult SellerList(string SellerName)
        {
            var result = from m in _context1.Users select m;

            if (!string.IsNullOrEmpty(SellerName))
            {
                result = result.Where(s => s.UserName.Contains(SellerName));
            }
          
            return View( result.Where(x => x.Role == "Admin" || x.Role == "Seller").ToList());
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
