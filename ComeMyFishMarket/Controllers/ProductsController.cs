using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ComeMyFishMarket.Data;
using ComeMyFishMarket.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ComeMyFishMarket.Areas.Identity.Data;


namespace ComeMyFishMarket.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ComeMyFishMarketClassContext _context;
        private readonly UserManager<ComeMyFishMarketUser> _userManager;
        public ProductsController(ComeMyFishMarketClassContext context, UserManager<ComeMyFishMarketUser> UserManager)
        {
            _context = context;
            _userManager = UserManager;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Product.ToListAsync());
        }

        public async Task<IActionResult> AdminProduct(String ProductName, String ProductCategory)
        {
            var product = from m in _context.Product select m;
            if(!String.IsNullOrEmpty(ProductName))
            {
                product = product.Where(s => s.ProductName.Contains(ProductName));
            }
            //Add Item into drop down list
            IQueryable<string> TypeQuery = from m in _context.Product orderby m.Category select m.Category;
            IEnumerable<SelectListItem> items = new SelectList(await TypeQuery.Distinct().ToListAsync());
            ViewBag.Category = items;

            if(!String.IsNullOrEmpty(ProductCategory))
            {
                product = product.Where(s => s.Category.Contains(ProductCategory));
            }
            return View(await _context.Product.ToListAsync());
        }

        public async Task<IActionResult> SellerProduct(String ProductName, String ProductCategory)
        {
            var product = from m in _context.Product select m;
            if (!String.IsNullOrEmpty(ProductName))
            {
                product = product.Where(s => s.ProductName.Contains(ProductName));
            }
            //Add Item into drop down list
            IQueryable<string> TypeQuery = from m in _context.Product orderby m.Category select m.Category;
            IEnumerable<SelectListItem> items = new SelectList(await TypeQuery.Distinct().ToListAsync());
            ViewBag.Category = items;

            if (!String.IsNullOrEmpty(ProductCategory))
            {
                product = product.Where(s => s.Category.Contains(ProductCategory));
            }
            return View(await _context.Product.ToListAsync());
        }

        public async Task<IActionResult> CustomerProduct(String ProductName, String ProductCategory)
        {
            var product = from m in _context.Product select m;
            if (!String.IsNullOrEmpty(ProductName))
            {
                product = product.Where(s => s.ProductName.Contains(ProductName));
            }
            //Add Item into drop down list
            IQueryable<string> TypeQuery = from m in _context.Product orderby m.Category select m.Category;
            IEnumerable<SelectListItem> items = new SelectList(await TypeQuery.Distinct().ToListAsync());
            ViewBag.Category = items;

            if (!String.IsNullOrEmpty(ProductCategory))
            {
                product = product.Where(s => s.Category.Contains(ProductCategory));
            }
            return View(await _context.Product.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,ProductName,Category,Quantity,Price,ProductImage,ProductStatus,UserID")] Product product, IFormFile files, string userid)
        {
            var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
            BlobManager bm = new BlobManager();
            product.ProductStatus = "Active";
            if (bm.UploadBlobImage(files,product.ProductName))
            {
                product.ProductImage = product.ProductName+files.FileName;
                product.UserID = userid;
                if (ModelState.IsValid)
                {
                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    if(user.Role.Equals("Admin"))
                    {
                        return RedirectToAction(nameof(this.AdminProduct));
                    }
                    if (user.Role.Equals("Seller"))
                    {
                        return RedirectToAction(nameof(this.SellerProduct));
                    }
                }
            }
            else
            {
                ViewBag.Message = "Please makesure that the uploaded image is jpg, jpeg and png file type!";
            }

            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,ProductName,Category,Quantity,Price,ProductImage,ProductStatus,UserID")] Product product, IFormFile files)
        {
            BlobManager bm = new BlobManager();
            var curuser = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
            if (id != product.ProductID)
            {
                return NotFound();
            }            
            if(files != null)
            {
                string orifile = product.ProductName + files.FileName;
                if(!product.ProductImage.Equals(orifile))
                {
                    if(bm.UploadBlobImage(files,product.ProductName))
                    {
                        product.ProductImage = product.ProductName + files.FileName;
                    }
                    else
                    {
                        ViewBag.Message = "Please makesure that the uploaded image is jpg, jpeg and png file type!";
                    }
                }
            }
            if (ModelState.IsValid)
            {                
                try
                {
                    _context.Update(product);
                    var cartitem = _context.ShoppingCart.Where(x => x.ProductId == product.ProductID).ToList();
                    if(cartitem.Count > 0)
                    {
                        foreach (var item in cartitem)
                        {
                            item.ProductImage = product.ProductImage;
                            item.ProductName = product.ProductName;
                            item.Price = product.Price;
                        }
                    }                   
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (curuser.Role.Equals("Admin"))
                {
                    return RedirectToAction(nameof(this.AdminProduct));
                }
                if (curuser.Role.Equals("Seller"))
                {
                    return RedirectToAction(nameof(this.SellerProduct));
                }
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var curuser = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            if (curuser.Role.Equals("Admin"))
            {
                return RedirectToAction(nameof(this.AdminProduct));
            }
            if (curuser.Role.Equals("Seller"))
            {
                return RedirectToAction(nameof(this.SellerProduct));
            }
            return View(product);
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductID == id);
        }
    }
}
