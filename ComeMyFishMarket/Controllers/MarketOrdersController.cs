using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ComeMyFishMarket.Data;
using ComeMyFishMarket.Models;

namespace ComeMyFishMarket.Controllers
{
    public class MarketOrdersController : Controller
    {
        private readonly ComeMyFishMarketClassContext _context;

        public MarketOrdersController(ComeMyFishMarketClassContext context)
        {
            _context = context;
        }

        // GET: MarketOrders
        public async Task<IActionResult> Index()
        {
            return View(await _context.MarketOrder.ToListAsync());
        }

        // GET: MarketOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marketOrder = await _context.MarketOrder
                .FirstOrDefaultAsync(m => m.MarketOrderID == id);
            if (marketOrder == null)
            {
                return NotFound();
            }

            return View(marketOrder);
        }

        // GET: MarketOrders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MarketOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MarketOrderID,OrderDescription,TotalAmount,OrderStatus,OrderDate,UserID,HandledBy")] MarketOrder marketOrder)
        {
            if (ModelState.IsValid)
            {
                marketOrder.Get = "No";
                _context.Add(marketOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(marketOrder);
        }

        // GET: MarketOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marketOrder = await _context.MarketOrder.FindAsync(id);
            if (marketOrder == null)
            {
                return NotFound();
            }
            return View(marketOrder);
        }

        // POST: MarketOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MarketOrderID,OrderDescription,TotalAmount,OrderStatus,OrderDate,UserID,HandledBy")] MarketOrder marketOrder)
        {
            if (id != marketOrder.MarketOrderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(marketOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarketOrderExists(marketOrder.MarketOrderID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(marketOrder);
        }

        // GET: MarketOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marketOrder = await _context.MarketOrder
                .FirstOrDefaultAsync(m => m.MarketOrderID == id);
            if (marketOrder == null)
            {
                return NotFound();
            }

            return View(marketOrder);
        }

        // POST: MarketOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var marketOrder = await _context.MarketOrder.FindAsync(id);
            _context.MarketOrder.Remove(marketOrder);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MarketOrderExists(int id)
        {
            return _context.MarketOrder.Any(e => e.MarketOrderID == id);
        }
    }
}
