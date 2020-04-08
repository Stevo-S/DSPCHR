using System;
using System.Collections.Generic;
using System.Text;
using DSPCHR.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DSPCHR.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected ApplicationDbContext(DbContextOptions options)
        {

        }
        public DbSet<ShortCode> ShortCodes { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<SubscriptionOfferWideMessage> SubscriptionOfferWideMessages { get; set; }
        public DbSet<OutboundMessage> OutboundMessages { get; set; }
        // TODO: Add Delivery Reports
        // TODO: Add Mo Messages

    }
}
