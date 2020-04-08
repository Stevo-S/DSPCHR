using DSPCHR.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSPCHR.ViewModels
{
    public class OfferViewModel
    {
        public Offer Offer { get; set; }
        public List<SelectListItem> ShortCodes { get; set; }
    }
}
