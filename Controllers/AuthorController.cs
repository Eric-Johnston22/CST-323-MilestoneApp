using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CST_323_MilestoneApp.Models;
using CST_323_MilestoneApp.Services;

namespace CST_323_MilestoneApp.Controllers
{
    public class AuthorController : Controller
    {
        // TO-DO: Remove LibraryContext and implement the AuthorDAO
        // TO-DO: Create Author view
        //private readonly LibraryContext _context;

        private readonly AuthorDAO _authorDAO;

        public AuthorController(AuthorDAO authorDAO)
        {
            _authorDAO = authorDAO;
        }

        // GET: Author
        public async Task<IActionResult> Index()
        {
            var authors = await _authorDAO.GetAllAuthorsAsync();

            return View(authors);
        }

        // GET: Author/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _authorDAO.GetAuthorById(id);
            

            return View(author);
        }

        // GET: Author/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Author/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Author_Id,Name")] Author author)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(author);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(author);
        //}

        //// GET: Author/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var author = await _context.Authors.FindAsync(id);
        //    if (author == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(author);
        //}

        //// POST: Author/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Author_Id,Name")] Author author)
        //{
        //    if (id != author.Author_Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(author);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!AuthorExists(author.Author_Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(author);
        //}

        //// GET: Author/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var author = await _context.Authors
        //        .FirstOrDefaultAsync(m => m.Author_Id == id);
        //    if (author == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(author);
        //}

        //// POST: Author/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var author = await _context.Authors.FindAsync(id);
        //    if (author != null)
        //    {
        //        _context.Authors.Remove(author);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool AuthorExists(int id)
        //{
        //    return _context.Authors.Any(e => e.Author_Id == id);
        //}
    }
}
