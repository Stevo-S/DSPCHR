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
        public async Task<IActionResult> Index(DateTime? from, DateTime? to, int pageNumber = 1, int pageSize = 25, string subscriptionStatus = "",
            string phoneNumber = "", string offerCode = "")
        {
            var currentUser = _userManager.GetUserAsync(User).Result;
            var model = new SubscriberListViewModel();

            int offset = (pageSize * pageNumber) - pageSize;

            var allowedOfferCodes = new HashSet<string>();

            var offerCodesToQuery = new HashSet<string>();

            var allOffers = _context.Offers.Include(o => o.ShortCode).ToList();

            var subscribers = from s in _context.Subscribers select s;

            // Only admins are allowed to view subscribers of any/all offer codes by default
            if (!User.IsInRole(RoleNames.AdministratorsRoleName))
            {
                var allOfferCodes = new HashSet<string>(allOffers.Select(o => o.OfferCode));
                allowedOfferCodes = new HashSet<string>(_authorisationResources.AllowedOfferCodes(currentUser));

                // Load all offer codes into the view model Offers that user is allowed to view
                model.Offers.AddRange(allOffers.Where(o => allowedOfferCodes.Contains(o.OfferCode)).Select(o =>
                    new SelectListItem
                    {
                        Text = o.ShortCode.Code,
                        Value = o.OfferCode,
                        Selected = o.OfferCode.Equals(offerCode) // Mark as selected if offerCode was specified
                    }
                ).ToList());

                // If offer code is not specified,
                // query subscribers of all offer codes user is allowed to
                if (string.IsNullOrEmpty(offerCode))
                {
                    offerCodesToQuery = allowedOfferCodes;
                }
                else
                {
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
                    offerCodesToQuery.Add(offerCode);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(offerCode))
                {
                    offerCodesToQuery.Add(offerCode);
                }
                // Load all offer codes into the view model Offers select list if user is admin
                model.Offers.AddRange(allOffers.Select(o =>
                    new SelectListItem
                    {
                        Text = o.ShortCode.Code,
                        Value = o.OfferCode,
                        Selected = o.OfferCode.Equals(offerCode) // Mark as selected if offerCode was specified
                    }
                ).ToList());
            }

            // Filter by offer codes
            if (offerCodesToQuery.Any())
            {
                subscribers = subscribers.Where(s => offerCodesToQuery.Contains(s.OfferCode));
            }

            // Filter by phoneNumber
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                subscribers = subscribers.Where(s => s.Msisdn.Contains(phoneNumber));
                model.PhoneNumber = phoneNumber;
            }

            // Filter by subscription status
            subscriptionStatus ??= "";
            if (!string.IsNullOrEmpty(subscriptionStatus))
            {
                var status = subscriptionStatus.ToLower();

                if (status == "active")
                {
                    subscribers = subscribers.Where(s => s.IsActive == true);
                }
                else if (status == "inactive")
                {
                    subscribers = subscribers.Where(p => !p.IsActive == true);
                }
            }
            model.SubscriptionStatus = subscriptionStatus;
            model.Statuses.Where(s => s.Value.Equals(subscriptionStatus)).FirstOrDefault().Selected = true;

            if (from == null)
            {
                from = DateTime.Today;
            }
            
            if (to < from || to == null)
            {
                to = DateTime.Today.AddDays(1);
            }

            subscribers = subscribers.Where(s => s.LastSubscribedAt > from && s.LastSubscribedAt < to);
            model.From = from;
            model.To = to;

            var result = new PagedResult<Subscriber>
            {
                TotalItems = await subscribers.CountAsync(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            subscribers = subscribers.OrderByDescending(s => s.LastSubscribedAt)
                                    .Skip(offset).Take(pageSize);

            result.Data = await subscribers.AsNoTracking().ToListAsync();

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
