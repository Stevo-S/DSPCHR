using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSPCHR.Models
{
    public class Subscriber
    {
        public int Id { get; set; }
        public string Msisdn { get; set; }
        public bool IsActive { get; set; }
        public string ShortCode { get; set; }
        public string OfferCode { get; set; }
        public DateTime FirstSubscribedAt { get; set; }
        public DateTime LastSubscribedAt { get; set; }
        public DateTime LastUnsubscribedAt { get; set; }
    }
}
