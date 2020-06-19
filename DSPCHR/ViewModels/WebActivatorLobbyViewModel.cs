using DSPCHR.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSPCHR.ViewModels
{
    public class WebActivatorLobbyViewModel
    {
        public WebActivator WebActivator { get; set; }
        public string SubscriptionLink { get; set; }
        public long ClickId { get; set; }
        public string CampaignId { get; set; }
    }
}
