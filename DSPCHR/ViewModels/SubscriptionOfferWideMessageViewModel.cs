using DSPCHR.Data;
using DSPCHR.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSPCHR.ViewModels
{
    public class SubscriptionOfferWideMessageViewModel
    {
        public SubscriptionOfferWideMessageViewModel()
        {
            Offers = new List<SelectListItem>();
        }
        public  List<SelectListItem> Offers{ get; set; }

        public long Id { get; set; }
        public string Content { get; set; }
        public string OfferCode { get; set; }
        public IEnumerable<string> OfferCodes { get; set; }
        public string ShortCode { get; set; }
        public DateTime SendAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

    }
}
