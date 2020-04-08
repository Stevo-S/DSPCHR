using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSPCHR.Models
{
    public class OutboundMessage
    {
        public long Id { get; set; }
        public string ShortCode { get; set; }
        public string Destination { get; set; }
        public string OfferCode { get; set; }
        public string Content { get; set; }
        public string LinkId { get; set; }
        public DateTime SendAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
