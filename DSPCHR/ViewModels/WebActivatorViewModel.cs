using DSPCHR.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DSPCHR.ViewModels
{
    public class WebActivatorViewModel
    {
        public WebActivator WebActivator { get; set; }
        public List<SelectListItem> Offers { get; set; }
    }
}
