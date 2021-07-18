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
    public class WalletHistoriesController : Controller
    {
        private readonly ComeMyFishMarketClassContext _context;

        public WalletHistoriesController(ComeMyFishMarketClassContext context)
        {
            _context = context;
        }

        // GET: WalletHistories
        public async Task<IActionResult> Index()
        {
            return View(await _context.WalletHistory.ToListAsync());
        }

        // GET: WalletHistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var walletHistory = await _context.WalletHistory
                .FirstOrDefaultAsync(m => m.ID == id);
            if (walletHistory == null)
            {
                return NotFound();
            }

            return View(walletHistory);
        }

        // GET: WalletHistories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WalletHistories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,HistoryDesc,HistoryDate")] WalletHistory walletHistory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(walletHistory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(walletHistory);
        }

        // GET: WalletHistories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var walletHistory = await _context.WalletHistory.FindAsync(id);
            if (walletHistory == null)
            {
                return NotFound();
            }
            return View(walletHistory);
        }

        // POST: WalletHistories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,HistoryDesc,HistoryDate")] WalletHistory walletHistory)
        {
            if (id != walletHistory.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(walletHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WalletHistoryExists(walletHistory.ID))
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
            return View(walletHistory);
        }

        // GET: WalletHistories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var walletHistory = await _context.WalletHistory
                .FirstOrDefaultAsync(m => m.ID == id);
            if (walletHistory == null)
            {
                return NotFound();
            }

            return View(walletHistory);
        }

        // POST: WalletHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var walletHistory = await _context.WalletHistory.FindAsync(id);
            _context.WalletHistory.Remove(walletHistory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WalletHistoryExists(int id)
        {
            return _context.WalletHistory.Any(e => e.ID == id);
        }
    }
}
