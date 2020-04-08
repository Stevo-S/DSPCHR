using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DSPCHR.Data;
using DSPCHR.Models;
using DSPCHR.ViewModels;
using Hangfire;

namespace DSPCHR.Controllers
{
    public class SubscriptionOfferWideMessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Jobs.Messages _messagesJob;

        public SubscriptionOfferWideMessagesController(ApplicationDbContext context, Jobs.Messages messagesJob)
        {
            _context = context;
            _messagesJob = messagesJob;
        }

        // GET: SubscriptionOfferWideMessages
        public async Task<IActionResult> Index()
        {
            return View(await _context.SubscriptionOfferWideMessages.ToListAsync());
        }

        // GET: SubscriptionOfferWideMessages/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscriptionOfferWideMessage = await _context.SubscriptionOfferWideMessages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subscriptionOfferWideMessage == null)
            {
                return NotFound();
            }

            return View(subscriptionOfferWideMessage);
        }

        // GET: SubscriptionOfferWideMessages/Create
        public IActionResult Create()
        {
            var viewModel = new SubscriptionOfferWideMessageViewModel
            {
                SendAt = DateTime.Now.AddMinutes(2)
            };

            return View(PrepareViewModel(viewModel));
        }

        // POST: SubscriptionOfferWideMessages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Content,OfferCode,ShortCode,SendAt,CreatedAt,LastUpdatedAt")]
                                                        SubscriptionOfferWideMessage subscriptionOfferWideMessage, string[] offerCodes)
        {
            if (ModelState.IsValid)
            {
                subscriptionOfferWideMessage.CreatedAt = DateTime.Now;
                subscriptionOfferWideMessage.LastUpdatedAt = DateTime.Now;

                if (offerCodes != null)
                {
                    string parentJobId = "";
                    var selectedOffers = _context.Offers.Where(s => offerCodes.Contains(s.OfferCode)).Include(s => s.ShortCode).ToList();
                    var timeLeft = subscriptionOfferWideMessage.SendAt - DateTime.Now;

                    foreach (var offer in selectedOffers)
                    {
                        var offerMessage = new SubscriptionOfferWideMessage
                        {
                            Content = subscriptionOfferWideMessage.Content,
                            OfferCode = offer.OfferCode,
                            ShortCode = offer.ShortCode.Code,
                            SendAt = subscriptionOfferWideMessage.SendAt
                        };

                        _context.Add(offerMessage);
                        _context.SaveChanges();

                        if (!String.IsNullOrEmpty(parentJobId))
                        {
                            // If this is not the first job in the batch, chain current job to already scheduled job
                            parentJobId = BackgroundJob.ContinueJobWith(parentJobId, () => _messagesJob.SendToSubscribers(offerMessage.Id));
                        }
                        else
                        {
                            // This is the first job in this batch, schedule it then
                            parentJobId = BackgroundJob.Schedule(() => _messagesJob.SendToSubscribers(offerMessage.Id), timeLeft);
                        }
                    }
                }

                //await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subscriptionOfferWideMessage);
        }

        // GET: SubscriptionOfferWideMessages/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscriptionOfferWideMessage = await _context.SubscriptionOfferWideMessages.FindAsync(id);
            if (subscriptionOfferWideMessage == null)
            {
                return NotFound();
            }
            return View(subscriptionOfferWideMessage);
        }

        // POST: SubscriptionOfferWideMessages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Content,OfferCode,ShortCode,SendAt,CreatedAt,LastUpdatedAt")] SubscriptionOfferWideMessage subscriptionOfferWideMessage, string[] ServiceIds)
        {
            if (id != subscriptionOfferWideMessage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subscriptionOfferWideMessage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubscriptionOfferWideMessageExists(subscriptionOfferWideMessage.Id))
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
            return View(subscriptionOfferWideMessage);
        }

        // GET: SubscriptionOfferWideMessages/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscriptionOfferWideMessage = await _context.SubscriptionOfferWideMessages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subscriptionOfferWideMessage == null)
            {
                return NotFound();
            }

            return View(subscriptionOfferWideMessage);
        }

        // POST: SubscriptionOfferWideMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var subscriptionOfferWideMessage = await _context.SubscriptionOfferWideMessages.FindAsync(id);
            _context.SubscriptionOfferWideMessages.Remove(subscriptionOfferWideMessage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubscriptionOfferWideMessageExists(long id)
        {
            return _context.SubscriptionOfferWideMessages.Any(e => e.Id == id);
        }

        private SubscriptionOfferWideMessageViewModel PrepareViewModel(SubscriptionOfferWideMessageViewModel viewModel)
        {
            // Load Offers Including ShortCodes
            //var offers = _context.Offers.Include(o => o.ShortCode).ToList()
            //    .GroupBy(o => o.ShortCode.Code).ToList();

            //foreach (var shortCodeGroup in offers)
            //{
            //    var optionGroup = new SelectListGroup
            //    {
            //        Name = shortCodeGroup.Key
            //    };

            //    foreach (var offer in shortCodeGroup)
            //    {
            //        viewModel.Offers.Add(
            //            new SelectListItem
            //            {
            //                Value = offer.OfferCode,
            //                Text = offer.Name,
            //                Group = optionGroup
            //            }
            //        );
            //    }
            //}


            // Load ShortCodes Including offers
            //var shortCodes = _context.ShortCodes.Include(sc => sc.Offers).ToList();

            //foreach(var shortCode in shortCodes)
            //{
            //    SelectListGroup optGroup = new SelectListGroup { Name = shortCode.Code };

            //    foreach(var offer in shortCode.Offers)
            //    {
            //        viewModel.Offers.Add(new SelectListItem 
            //        {
            //            Text = offer.Name,
            //            Value = offer.OfferCode,
            //            Group = optGroup
            //        });
            //    }
            //}

            // Load Offers and Short Codes Separately
            var offers = _context.Offers.ToList();
            var shortCodes = _context.ShortCodes.ToList();

            foreach (var shortCode in shortCodes)
            {
                SelectListGroup optGroup = new SelectListGroup { Name = shortCode.Code };

                var shortCodeOffers = offers.Where(o => o.ShortCodeId == shortCode.Id).ToList();

                foreach (var offer in shortCodeOffers)
                {
                    viewModel.Offers.Add(new SelectListItem
                    {
                        Text = offer.Name,
                        Value = offer.OfferCode,
                        Group = optGroup
                    });
                }

                //foreach(var offer in offers)
                //{
                //    if (offer.ShortCodeId.e)
                //}
            }

            return viewModel;
        }
    }
}
