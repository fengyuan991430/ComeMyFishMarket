using ComeMyFishMarket.Areas.Identity.Data;
using ComeMyFishMarket.Data;
using ComeMyFishMarket.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComeMyFishMarket.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ComeMyFishMarketClassContext _context;
        private readonly UserManager<ComeMyFishMarketUser> _userManager;
        private readonly ComeMyFishMarketContext _context1;
        public ShoppingCartController(ComeMyFishMarketClassContext context, UserManager<ComeMyFishMarketUser> UserManager, ComeMyFishMarketContext context1)
        {
            _context = context;
            _userManager = UserManager;
            _context1 = context1;
        }
        public IActionResult Index(string message = null)
        {
            var result = _context.ShoppingCart.Where(x => x.CustomerId == _userManager.GetUserId(User)).ToList();
            ViewBag.message = message;
            return View(result);
        }

        public IActionResult Checkout(string radio)
        {
            string message = null;
            var result = _context.ShoppingCart.Where(x => x.SellerId == radio && x.CustomerId == _userManager.GetUserId(User)).ToList();
            foreach(var cartitem in result)
            {
                var product = _context.Product.FirstOrDefault(x => x.ProductID == cartitem.ProductId);
                if(product != null && product.ProductStatus == "Active" && product.Quantity > 0)
                {
                    if (cartitem.Quantity > product.Quantity)
                    {
                        message = "Not enough quantity this added cart product-> " + cartitem.ProductName + ". Remaining quantity for this product -> " + product.Quantity;
                        return RedirectToAction("Index", new { message = message });
                    }
                }
                else
                {
                    message = "Sorry! " + cartitem.ProductName + " product is not available anymore.";
                    return RedirectToAction("Index", new { message = message });
                }
                
            }
            ViewBag.Seller = radio;
            return View(result);
        }

        public IActionResult Delete(int cartid)
        {
            string message = null;
            var item = _context.ShoppingCart.FirstOrDefault(x => x.ShoppingCartID == cartid);
            if(item != null)
            {
                _context.ShoppingCart.Remove(item);
                _context.SaveChanges();
            }
            else
            {
                message = "Product not found";
            }
            return RedirectToAction("Index", new { message = message});
        }

        [HttpPost]
        public IActionResult PlaceOrder(string id)
        {
            var cart = _context.ShoppingCart.Where(x => x.SellerId == id && x.CustomerId == _userManager.GetUserId(User)).ToList();
            MarketOrder order = new MarketOrder
            {
                OrderDate = DateTime.Now,
                OrderDescription = "Total Order Items: " + cart.Count,
                OrderStatus = "Completed",
                TotalAmount = cart.Sum(x => x.Quantity * x.Price),
                UserID = _userManager.GetUserId(User),
                HandledBy = id,
                GetFeedback = "No"
            };
            _context.MarketOrder.Add(order);
            _context.SaveChanges();

            foreach(var item in cart)
            {
                OrderItem orderitem = new OrderItem
                {
                    ItemName = item.ProductName,
                    ItemQuantity = item.Quantity,
                    TotalPrice = item.Quantity * item.Price,
                    ItemID = item.ProductId,
                    UserID = item.CustomerId,
                    OrderID = order.MarketOrderID
                };
                _context.OrderItem.Add(orderitem);
                var product = _context.Product.FirstOrDefault(x => x.ProductID == item.ProductId);
                product.Quantity = product.Quantity - item.Quantity;
                _context.ShoppingCart.Remove(item);
                _context.SaveChanges();
            }
            
            return RedirectToAction("OrderConfirmation", new { orderid = order.MarketOrderID });
        }

        public IActionResult OrderConfirmation(int orderid)
        {
            ViewBag.orderid = orderid;
            return View();
        }
    }
}
