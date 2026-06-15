using booksXrelaysSomaShare.Data;
using booksXrelaysSomaShare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace booksXrelaysSomaShare.Controllers
{
    // SS3 updated feature where the buyer and admin can access wanted ads//
    [Authorize(Roles = "Buyer,Admin")]

    public class WantedAdsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WantedAdsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WantedAds
        public async Task<IActionResult> Index()
        {
            return View(await _context.WantedAds.ToListAsync());
        }

        // GET: WantedAds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wantedAd = await _context.WantedAds
                .FirstOrDefaultAsync(m => m.WantedAdId == id);
            if (wantedAd == null)
            {
                return NotFound();
            }

            return View(wantedAd);
        }

        // GET: WantedAds/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WantedAds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WantedAdId,BookTitle,Description,UserId")] WantedAd wantedAd)
        {
            if (ModelState.IsValid)
            {
                _context.Add(wantedAd);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(wantedAd);
        }

        // GET: WantedAds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wantedAd = await _context.WantedAds.FindAsync(id);
            if (wantedAd == null)
            {
                return NotFound();
            }
            return View(wantedAd);
        }

        // POST: WantedAds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WantedAdId,BookTitle,Description,UserId")] WantedAd wantedAd)
        {
            if (id != wantedAd.WantedAdId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wantedAd);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WantedAdExists(wantedAd.WantedAdId))
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
            return View(wantedAd);
        }

        // GET: WantedAds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wantedAd = await _context.WantedAds
                .FirstOrDefaultAsync(m => m.WantedAdId == id);
            if (wantedAd == null)
            {
                return NotFound();
            }

            return View(wantedAd);
        }

        // POST: WantedAds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wantedAd = await _context.WantedAds.FindAsync(id);
            if (wantedAd != null)
            {
                _context.WantedAds.Remove(wantedAd);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WantedAdExists(int id)
        {
            return _context.WantedAds.Any(e => e.WantedAdId == id);
        }
    }
}
