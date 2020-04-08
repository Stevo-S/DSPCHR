using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSPCHR.Models
{
    public class Offer
    {
        public long Id { get; set; }
        public string OfferCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ActivationKeywords { get; set; }
        public string DeactivationKeywords { get; set; }
        public string ActivationWelcomeMessage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public long ShortCodeId { get; set; }
        public virtual ShortCode ShortCode { get; set; }
    }
}
