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

        public string Query { get; set; } = string.Empty;
        public PagedResult<Subscriber> Subscribers { get; set; }
    }
}
