using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cloudscribe.Pagination.Models;
using DSPCHR.Models;

namespace DSPCHR.ViewModels
{
    public class SubscriberListViewModel
    {
        public SubscriberListViewModel()
        {
            Subscribers = new PagedResult<Subscriber>();
        }

        public PagedResult<Subscriber> Subscribers { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime? From { get; set; } = null;
        public DateTime? To { get; set; } = null;
        public string SubscriptionStatus { get; set; } = "any";

    }
}
