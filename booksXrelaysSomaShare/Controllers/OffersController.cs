using booksXrelaysSomaShare.Data;
using booksXrelaysSomaShare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace booksXrelaysSomaShare.Controllers
{
    [Authorize]
    public class OffersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OffersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Offers
        public async Task<IActionResult> Index()
            // ss3 improvement
        {
            var offers = await _context.Offers
                .Include(o => o.Textbook)
                .ToListAsync();

            return View(offers);
        }

        // GET: Offers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //ss3 improvement
            var offer = await _context.Offers
                .Include(o => o.Textbook)
                .FirstOrDefaultAsync(m => m.OfferId == id);
            if (offer == null)
            {
                return NotFound();
            }

            return View(offer);
        }

        // GET: Offers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Offers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OfferId,OfferPrice,TextbookId,IsAccepted")] Offer offer)
        {
            // Store logged-in user's email
            offer.UserEmail = User.Identity.Name;

            ModelState.Remove("UserEmail");
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                _context.Add(offer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["TextbookId"] = new SelectList(_context.Textbooks, "TextbookId", "Title", offer.TextbookId);

            return View(offer);
        }

        // accept method
        public async Task<IActionResult> Accept(int id)
        {
            var offer = await _context.Offers.FindAsync(id);

            if (offer == null)
            {
                return NotFound();
            }

            offer.IsAccepted = true;
            // ss3 improvement 
            var textbook = await _context.Textbooks
                .FirstOrDefaultAsync(t => t.TextBookID == offer.TextbookId);

            // Create transaction from accepted offer
            var transaction = new Transaction
            {
                OfferId = offer.OfferId,

                // Store accepted offer price
                OfferPrice = offer.OfferPrice,

                // Store textbook title
                BookTitle = textbook?.Title,

                TransactionDate = DateTime.Now,

                // Accepted offers become completed transactions
                IsCompleted = true
            };

            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // reject method
        public async Task<IActionResult> Reject(int id)
        {
            var offer = await _context.Offers.FindAsync(id);

            if (offer == null)
            {
                return NotFound();
            }

            offer.IsAccepted = false;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: Offers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offer = await _context.Offers.FindAsync(id);
            if (offer == null)
            {
                return NotFound();
            }
            return View(offer);
        }

        // POST: Offers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OfferId,OfferPrice,TextbookId,UserId,IsAccepted")] Offer offer)
        {
            if (id != offer.OfferId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(offer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OfferExists(offer.OfferId))
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
            return View(offer);
        }

        // GET: Offers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offer = await _context.Offers
                .FirstOrDefaultAsync(m => m.OfferId == id);
            if (offer == null)
            {
                return NotFound();
            }

            return View(offer);
        }

        // POST: Offers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var offer = await _context.Offers.FindAsync(id);
            if (offer != null)
            {
                _context.Offers.Remove(offer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OfferExists(int id)
        {
            return _context.Offers.Any(e => e.OfferId == id);
        }
    }
}
