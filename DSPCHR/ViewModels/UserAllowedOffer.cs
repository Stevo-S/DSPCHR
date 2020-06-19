using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSPCHR.ViewModels
{
    public class UserAllowedOffer
    {
        public long OfferId { get; set; }
        public string ShortCode { get; set; }
        public string OfferName { get; set; }
        public bool Allowed { get; set; }
    }
}
