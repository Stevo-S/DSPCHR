using System;
using System.Collections.Generic;
using System.Text;
using DSPCHR.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DSPCHR.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.Migrate();
            //SeedData.Initialize();
        }

        protected ApplicationDbContext(DbContextOptions options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<ApplicationUserOffer>()
                .HasKey(auo => new { auo.ApplicationUserId, auo.OfferId });

            builder.Entity<ApplicationUserOffer>()
                .HasOne(auo => auo.ApplicationUser)
                .WithMany(au => au.ApplicationUserOffers)
                .HasForeignKey(auo => auo.ApplicationUserId);

            builder.Entity<ApplicationUserOffer>()
                .HasOne(auo => auo.Offer)
                .WithMany(o => o.ApplicationUserOffers)
                .HasForeignKey(auo => auo.OfferId);
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
