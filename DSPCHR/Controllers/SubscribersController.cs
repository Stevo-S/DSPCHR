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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using DSPCHR.Authorisation;

namespace DSPCHR.Controllers
{
    [Authorize]
    public class SubscribersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Resources _authorisationResources;
        private readonly UserManager<ApplicationUser> _userManager;
        public SubscribersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, 
            Resources resources)
        {
            _context = context;
            _userManager = userManager;
            _authorisationResources = resources;
        }

        // GET: Subscribers
        public async Task<IActionResult> Index(DateTime? from, DateTime? to, int pageNumber = 1, int pageSize = 25, string subscriptionStatus = "any", 
            string phoneNumber = "", string offerCode = "")
        {
            var currentUser = _userManager.GetUserAsync(User).Result;
            var model = new SubscriberListViewModel();

            int offset = (pageSize * pageNumber) - pageSize;

            var allowedOfferCodes = new HashSet<string>();

            var subscribers = from s in _context.Subscribers select s;

            // Only admins are allowed to view subscribers of any/all offer codes by default
            if (!User.IsInRole(RoleNames.AdministratorsRoleName))
            {
                // If offer code is not specified,
                // query subscribers of all offer codes user is allowed to
                if (string.IsNullOrEmpty(offerCode))
                {
                    allowedOfferCodes = new HashSet<string>(_authorisationResources.AllowedOfferCodes(currentUser));
                }
                else
                {
                    var allOfferCodes = new HashSet<string>(_context.Offers.Select(o => o.Name));

                    // Check if offer code exists
                    if (!allOfferCodes.Contains(offerCode))
                    {
                        return NotFound(); 
                    }

                    if (!allowedOfferCodes.Contains(offerCode))
                    {
                        return Forbid();
                    }

                    // Add offer code if it exists and is allowed for the user
                    allowedOfferCodes.Add(offerCode);
                }
            }
            
            // Filter by offer codes
            if (allowedOfferCodes.Any())
            {
                subscribers = subscribers.Where(s => allowedOfferCodes.Contains(s.OfferCode));
            }

            // Filter by phoneNumber
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                subscribers = subscribers.Where(s => s.Msisdn.Contains(phoneNumber));
                model.PhoneNumber = phoneNumber;
            }

            // Filter by subscription status
            if (!string.IsNullOrEmpty(subscriptionStatus))
            {
                if (subscriptionStatus == "active")
                {
                    subscribers = subscribers.Where(s => s.IsActive == true);
                }
                else if (subscriptionStatus == "inactive")
                {
                    subscribers = subscribers.Where(p => !p.IsActive == true);
                }

                model.SubscriptionStatus = subscriptionStatus;
            }

            if (from != null)
            {
                if (to < from || to == null)
                {
                    to = DateTime.Today.AddDays(1);
                }

                subscribers = subscribers.Where(s => s.LastSubscribedAt > from && s.LastSubscribedAt < to);

                model.From = from;
                model.To = to;
            }



            var result = new PagedResult<Subscriber>(); 
            result.TotalItems = await subscribers.CountAsync();

            subscribers = subscribers.OrderByDescending(s => s.LastSubscribedAt)
                                    .Skip(offset).Take(pageSize);

            result.Data = await subscribers.AsNoTracking().ToListAsync();
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
        //public IActionResult Create()
        //{
        //    return View();
        //}

        // POST: Subscribers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Msisdn,IsActive,OfferCode,FirstSubscribedAt,LastSubscribedAt,LastUnsubscribedAt")] Subscriber subscriber)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(subscriber);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(subscriber);
        //}

        // GET: Subscribers/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var subscriber = await _context.Subscribers.FindAsync(id);
        //    if (subscriber == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(subscriber);
        //}

        // POST: Subscribers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Msisdn,IsActive,OfferCode,FirstSubscribedAt,LastSubscribedAt,LastUnsubscribedAt")] Subscriber subscriber)
        //{
        //    if (id != subscriber.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(subscriber);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!SubscriberExists(subscriber.Id))
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
        //    return View(subscriber);
        //}

        // GET: Subscribers/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var subscriber = await _context.Subscribers
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (subscriber == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(subscriber);
        //}

        // POST: Subscribers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var subscriber = await _context.Subscribers.FindAsync(id);
        //    _context.Subscribers.Remove(subscriber);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool SubscriberExists(int id)
        {
            return _context.Subscribers.Any(e => e.Id == id);
        }
    }
}
