using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSPCHR.Models
{
    public class Subscription
    {
        [BindProperty]
        public string Msisdn { get; set; }
        public string ShortCode { get; set; }
        public string OfferCode { get; set; }
        public string OfferName { get; set; }
        public string Type { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
