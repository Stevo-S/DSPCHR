using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSPCHR.Models
{
    public class ShortCode
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public virtual ICollection<Offer> Offers { get; set; }
    }
}
