using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DSPCHR.Models
{
    public class SubscriptionOfferWideMessage
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public string OfferCode { get; set; }
        public string ShortCode { get; set; }
        public DateTime SendAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public IEnumerable<ValidationResult> Validate()
        {
            // StartTime should be some time in the future
            if (SendAt < DateTime.Now)
            {
                yield return new
                    ValidationResult("The Start time should be sometime in the future.");
            }

        }
    }
}
