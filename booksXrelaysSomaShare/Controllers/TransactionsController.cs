using booksXrelaysSomaShare.Data;
using booksXrelaysSomaShare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace booksXrelaysSomaShare.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TransactionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Transactions.ToListAsync());
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(m => m.TransactionId == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        // SS3 Improvemnt
        public IActionResult Create()
        {
            // Display Offer ID together with user email
            ViewData["OfferId"] = new SelectList(
                _context.Offers.Select(o => new
                {
                    o.OfferId,
                    DisplayText = $"Offer #{o.OfferId} - {o.UserEmail}"
                }),
                "OfferId",
                "DisplayText");

            return View();
        }
        
        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactionId,OfferId,TransactionDate,IsCompleted")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await
                    _userManager.GetUserAsync(User);

                if (currentUser != null)
                {
                    transaction.SellerId = currentUser.Id;
                }

                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Rebuild dropdown if validation fails
            ViewData["OfferId"] = new SelectList(
                _context.Offers.Select(o => new
                {
                    o.OfferId,
                    DisplayText = $"Offer #{o.OfferId} - {o.UserEmail}"
                }),
                "OfferId",
                "DisplayText",
                transaction.OfferId);

        


            return View(transaction);

        }

        // Complete transctions method
        // ss3 improvement

        // Complete transaction and update seller statistics
        public async Task<IActionResult> Complete(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            // Mark transaction as completed
            transaction.IsCompleted = true;

            // Find the seller who completed the transaction
            var seller = await _userManager.FindByIdAsync(transaction.SellerId);

            if (seller != null)
            {
                // Increase seller's completed transaction count
                seller.SuccessfulTransactions++;

                // Award trusted seller badge after 5 successful transactions
                if (seller.SuccessfulTransactions >= 5)
                {
                    seller.IsTrustedSeller = true;
                }

                await _userManager.UpdateAsync(seller);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransactionId,OfferId,TransactionDate,IsCompleted")] Transaction transaction)
        {
            if (id != transaction.TransactionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.TransactionId))
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
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(m => m.TransactionId == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }



        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.TransactionId == id);
        }
    }
}
