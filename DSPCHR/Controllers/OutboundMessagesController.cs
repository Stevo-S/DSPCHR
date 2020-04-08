using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DSPCHR.Data;
using DSPCHR.Models;

namespace DSPCHR.Controllers
{
    public class OutboundMessagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OutboundMessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OutboundMessages
        public async Task<IActionResult> Index()
        {
            return View(await _context.OutboundMessages.ToListAsync());
        }

        // GET: OutboundMessages/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var outboundMessage = await _context.OutboundMessages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (outboundMessage == null)
            {
                return NotFound();
            }

            return View(outboundMessage);
        }

        // GET: OutboundMessages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OutboundMessages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ShortCode,Destination,OfferCode,LinkId,SendAt,CreatedAt,UpdatedAt")] OutboundMessage outboundMessage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(outboundMessage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(outboundMessage);
        }

        // GET: OutboundMessages/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var outboundMessage = await _context.OutboundMessages.FindAsync(id);
            if (outboundMessage == null)
            {
                return NotFound();
            }
            return View(outboundMessage);
        }

        // POST: OutboundMessages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,ShortCode,Destination,OfferCode,LinkId,SendAt,CreatedAt,UpdatedAt")] OutboundMessage outboundMessage)
        {
            if (id != outboundMessage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(outboundMessage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OutboundMessageExists(outboundMessage.Id))
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
            return View(outboundMessage);
        }

        // GET: OutboundMessages/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var outboundMessage = await _context.OutboundMessages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (outboundMessage == null)
            {
                return NotFound();
            }

            return View(outboundMessage);
        }

        // POST: OutboundMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var outboundMessage = await _context.OutboundMessages.FindAsync(id);
            _context.OutboundMessages.Remove(outboundMessage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OutboundMessageExists(long id)
        {
            return _context.OutboundMessages.Any(e => e.Id == id);
        }
    }
}
