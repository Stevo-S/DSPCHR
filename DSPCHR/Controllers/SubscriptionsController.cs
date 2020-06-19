using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DSPCHR.Data;
using DSPCHR.Models;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DSPCHR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly Jobs.Subscriptions _subscriptionsJob;

        public SubscriptionsController(ApplicationDbContext context, Jobs.Subscriptions subscriptionsJob)
        {
            _context = context;
            _subscriptionsJob = subscriptionsJob;
        }

        // GET: api/Subscriptions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subscriber>>> Get()
        {
            return await _context.Subscribers.OrderByDescending(s => s.Id).Take(100).ToListAsync();
        }

        // GET: api/Subscriptions/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Subscriptions
        [HttpPost]
        public IActionResult Post([FromBody] Subscription subscription)
        {
            if (!IsValid(subscription))
            {
                return BadRequest();
            }

            BackgroundJob.Enqueue(() => _subscriptionsJob.ProcessNotification(subscription));
            return Ok();
        }

        // PUT: api/Subscriptions/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        // POST: /api/subscriptions/activate
        [HttpPost]
        [Route("/api/subscriptions/activate")]
        public ActionResult Activate(SubscriptionRequistion subscriptionRequistion)
        {
            if (ModelState.IsValid)
            {
                var webActivationClick = _context.WebActivationClicks.Find(subscriptionRequistion.ClickId);

                // Add MSISDN to webActivationClick if it exists for the current click ID
                if (webActivationClick != null)
                {
                    webActivationClick.Msisdn = subscriptionRequistion.Msisdn;
                    webActivationClick.LastUpdated = DateTime.Now;
                    _context.SaveChanges();
                }

                BackgroundJob.Enqueue(() => _subscriptionsJob.
                    SendActivationRequest(subscriptionRequistion.Msisdn, 
                                          subscriptionRequistion.OfferCode,
                                          subscriptionRequistion.ShortCode ?? ""));
                return Ok(new { Status = "Processing Activation Request" });
            }

            return BadRequest();
        }

        private bool IsValid(Subscription subscription)
        {
            if (subscription == null)
            {
                return false;
            }

            bool valid = string.IsNullOrEmpty(subscription.Msisdn) ? false : true;
            valid = string.IsNullOrEmpty(subscription.OfferCode) ? false : true;
            valid = string.IsNullOrEmpty(subscription.Type) ? false : true;

            return valid;
        }

    }

    public class SubscriptionRequistion
    {
        [DataType(DataType.PhoneNumber)]
        public string Msisdn { get; set; }

        [Required]
        public string OfferCode { get; set; }

        public string ShortCode { get; set; }
        public long ClickId { get; set; }
        public string CampaignId { get; set; }
    }
}
