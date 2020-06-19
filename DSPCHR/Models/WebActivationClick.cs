using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSPCHR.Models
{
    public class WebActivationClick
    {
        public long Id { get; set; }
        public string Msisdn { get; set; }
        public string ClickId { get; set; }
        public string OfferCode { get; set; }
        public bool Converted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdated { get; set; }

        public long WebActivatorId { get; set; }
        public WebActivator WebActivator { get; set; }
    }
}
