using DSPCHR.Data;
using DSPCHR.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSPCHR.Authorisation
{
    public class Resources
    {
        private readonly ApplicationDbContext _context;
        public Resources(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public List<string> AllowedOfferCodes(ApplicationUser appUser)
        {
            var user = _context.Users.Include(u => u.ApplicationUserOffers).ThenInclude(auo => auo.Offer).
                ThenInclude(o => o.ShortCode).FirstOrDefault(u => u.Id.Equals(appUser.Id));

            return user.ApplicationUserOffers.Select(auo => auo.Offer.OfferCode).ToList();
        }
    }
}
