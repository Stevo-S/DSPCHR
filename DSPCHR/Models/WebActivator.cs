using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DSPCHR.Models
{
    public class WebActivator
    {
        public long Id { get; set; }
        public string Name { get; set; }

        [StringLength(255)]
        public string Banner { get; set; }

        public string UssdFallBack { get; set; }

        public bool ForwardClickId { get; set; }

        public string PostBackUrl { get; set; }

        public long OfferId { get; set; }

        public virtual Offer Offer { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
