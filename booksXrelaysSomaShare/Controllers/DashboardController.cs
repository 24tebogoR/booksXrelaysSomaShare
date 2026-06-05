using booksXrelaysSomaShare.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Mvc;

namespace booksXrelaysSomaShare.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        
       
        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Displays dashboard summary information
        public async Task<IActionResult> Index()
        {
            // Get total number of textbooks in the system
            ViewBag.Textbooks = await _context.Textbooks.CountAsync();

            // Get total number of wanted ads
            ViewBag.WantedAds = await _context.WantedAds.CountAsync();

            // Get total number of offers
            ViewBag.Offers = await _context.Offers.CountAsync();

            // Get total number of transactions
            ViewBag.Transactions = await _context.Transactions.CountAsync();

            // ss3 improvement 
            // Get completed transactions
            ViewBag.CompletedTransactions = await _context.Transactions.CountAsync(t => t.IsCompleted);

            // Calculate average review rating
            ViewBag.AverageRating =
                await _context.Reviews.AnyAsync()
                ? await _context.Reviews.AverageAsync(r => r.Rating)
                : 0;

            // Get total number of reviews
            ViewBag.Reviews = await _context.Reviews.CountAsync();

            // Return dashboard view with all data
            return View();
        }
    }
}
