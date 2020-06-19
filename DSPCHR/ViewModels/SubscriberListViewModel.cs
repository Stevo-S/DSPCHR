using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using cloudscribe.Pagination.Models;
using DSPCHR.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DSPCHR.ViewModels
{
    public class SubscriberListViewModel
    {
        public SubscriberListViewModel()
        {
            Subscribers = new PagedResult<Subscriber>();
            Offers.Add(new SelectListItem { Text = "All", Value = "" });
        }

        public PagedResult<Subscriber> Subscribers { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; } = string.Empty;
        public string OfferCode { get; set; } = string.Empty;
        public List<SelectListItem> Offers { get; set; } = new List<SelectListItem>();
        public DateTime? From { get; set; } = null;
        public DateTime? To { get; set; } = null;
        public string SubscriptionStatus { get; set; } = "any";

        public List<SelectListItem> Statuses { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Text = "Any", Value = "" },
            new SelectListItem { Text = "Active", Value = "active" },
            new SelectListItem { Text = "Inactive", Value = "inactive" }
        };

    }
}
