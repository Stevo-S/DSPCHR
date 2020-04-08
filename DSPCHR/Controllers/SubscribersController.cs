using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DSPCHR.Data;
using DSPCHR.Models;
using cloudscribe.Pagination.Models;
using System.Threading;
using DSPCHR.ViewModels;

namespace DSPCHR.Controllers
{
    public class SubscribersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SubscribersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Subscribers
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 25)
        {
            //return View(await _context.Subscribers.OrderByDescending(s => s.LastSubscribedAt).Take(100).ToListAsync());

            var model = new SubscriberListViewModel();

            int offset = (pageSize * pageNumber) - pageSize;

            var query = _context.Subscribers.OrderByDescending(s => s.LastSubscribedAt)
                                    .Skip(offset).Take(pageSize);

            var result = new PagedResult<Subscriber>();
            result.Data = await query.AsNoTracking().ToListAsync();
            result.TotalItems = await _context.Subscribers.CountAsync();
            result.PageNumber = pageNumber;
            result.PageSize = pageSize;

            model.Subscribers = result;

            return View(model);
        }

        // GET: Subscribers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscriber = await _context.Subscribers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subscriber == null)
            {
                return NotFound();
            }

            return View(subscriber);
        }

        // GET: Subscribers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Subscribers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Msisdn,IsActive,OfferCode,FirstSubscribedAt,LastSubscribedAt,LastUnsubscribedAt")] Subscriber subscriber)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subscriber);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subscriber);
        }

        // GET: Subscribers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscriber = await _context.Subscribers.FindAsync(id);
            if (subscriber == null)
            {
                return NotFound();
            }
            return View(subscriber);
        }

        // POST: Subscribers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Msisdn,IsActive,OfferCode,FirstSubscribedAt,LastSubscribedAt,LastUnsubscribedAt")] Subscriber subscriber)
        {
            if (id != subscriber.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subscriber);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubscriberExists(subscriber.Id))
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
            return View(subscriber);
        }

        // GET: Subscribers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscriber = await _context.Subscribers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subscriber == null)
            {
                return NotFound();
            }

            return View(subscriber);
        }

        // POST: Subscribers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subscriber = await _context.Subscribers.FindAsync(id);
            _context.Subscribers.Remove(subscriber);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubscriberExists(int id)
        {
            return _context.Subscribers.Any(e => e.Id == id);
        }
    }
}
