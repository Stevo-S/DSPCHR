using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DSPCHR.Data;
using DSPCHR.Models;
using Microsoft.AspNetCore.Authorization;

namespace DSPCHR.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ShortCodesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShortCodesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ShortCodes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ShortCodes.ToListAsync());
        }

        // GET: ShortCodes/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shortCode = await _context.ShortCodes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shortCode == null)
            {
                return NotFound();
            }

            return View(shortCode);
        }

        // GET: ShortCodes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ShortCodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,IsActive,CreatedAt,LastUpdatedAt")] ShortCode shortCode)
        {
            if (ModelState.IsValid)
            {
                shortCode.CreatedAt = DateTime.Now;
                shortCode.LastUpdatedAt = DateTime.Now;

                _context.Add(shortCode);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(shortCode);
        }

        // GET: ShortCodes/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shortCode = await _context.ShortCodes.FindAsync(id);
            if (shortCode == null)
            {
                return NotFound();
            }
            return View(shortCode);
        }

        // POST: ShortCodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Code,IsActive,CreatedAt,LastUpdatedAt")] ShortCode shortCode)
        {
            if (id != shortCode.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shortCode);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShortCodeExists(shortCode.Id))
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
            return View(shortCode);
        }

        // GET: ShortCodes/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shortCode = await _context.ShortCodes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shortCode == null)
            {
                return NotFound();
            }

            return View(shortCode);
        }

        // POST: ShortCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var shortCode = await _context.ShortCodes.FindAsync(id);
            _context.ShortCodes.Remove(shortCode);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShortCodeExists(long id)
        {
            return _context.ShortCodes.Any(e => e.Id == id);
        }
    }
}
