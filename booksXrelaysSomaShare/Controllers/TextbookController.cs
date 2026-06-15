using booksXrelaysSomaShare.Data;
using booksXrelaysSomaShare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace booksXrelaysSomaShare.Controllers
{
    public class TextbookController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TextbookController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index( string search, decimal? minPrice, decimal? maxPrice, int page = 1)
        {
            var textbooks = _context.Textbooks.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                textbooks = textbooks.Where(t =>
                    t.Title.Contains(search) ||
                    t.Author.Contains(search) ||
                    t.ISBN.Contains(search) ||
                    t.Module.Contains(search));
            }

            // SS3 SEARCH FILTERING IMPROVEMENTS

            if (minPrice.HasValue)
            {
                textbooks = textbooks.Where(t => t.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                textbooks = textbooks.Where(t => t.Price <= maxPrice.Value);
            }

            // SS3 Pagination Improvement
            int pageSize = 3;

            var pagedTextbooks = await textbooks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;

            ViewBag.HasPreviousPage = page > 1;

            ViewBag.HasNextPage =
                await textbooks.CountAsync() >
                page * pageSize;

            return View(pagedTextbooks);
        }

        //Create

        // SS3 updated feature where the seller and admin can create textbooks//
        [Authorize(Roles = "Seller,Admin")]

        public IActionResult Create()
        {
            return View();
        }

        // Post for Textbook

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Textbook textbook, IFormFile photo)

        {
            if (!ModelState.IsValid)
            {
                return View(textbook);
            }
            
            if (photo != null)
            {
                string fileName = photo.FileName;

                string path = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }
                
                textbook.ImageName = fileName;
            }
            
            _context.Add(textbook);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            
        }



        // Employee Edit

        // SS3 updated feature only the seller and admin can edit textbooks//
        [Authorize(Roles = "Seller,Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var textbook = await _context.Textbooks.FindAsync(id);
            if (textbook == null)
            {
                return NotFound();
            }

            return View(textbook);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, [Bind("TextBookID, Title, Author, Description, ISBN, Module, Price, ListingDate")] Textbook textbook)
        {
            if (id != textbook.TextBookID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(textbook);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TextbookExist(textbook.TextBookID))
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
            return View(textbook);
        }


        // Textbook delete


        // SS3 updated feature only the seller and admin can delete textbooks//
        [Authorize(Roles = "Seller,Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var textbook = await _context.Textbooks
                .FirstOrDefaultAsync(m => m.TextBookID == id);
            if (textbook == null)
            {
                return NotFound();
            }

            return View(textbook);
        }

        // Post Textbook delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult>DeleteConfirmed(int id)

        {
            var textbook = await _context.Textbooks.FindAsync(id);
            if (textbook == null)
            {
                return NotFound();
            }

            _context.Textbooks.Remove(textbook);
            await _context.SaveChangesAsync();

            // redirect to index

            return RedirectToAction(nameof(Index));
        }

        private bool TextbookExist(int id)
        {
            return _context.Textbooks.Any(e => e.TextBookID == id);
        }
    } 
}
