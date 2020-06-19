using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSPCHR.Data;
using DSPCHR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Drawing;
using DSPCHR.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Web;

namespace DSPCHR.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class WebActivatorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _imageStorageDirectory;
        //private readonly string _imageStorageRequestPath = "/imagestore";
        private readonly string _bannerImagesFolderName = "banners";
        private readonly int _minimumImageBytes = 1024;

        public WebActivatorsController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _imageStorageDirectory = configuration["ImageStorageDirectory"];
        }

        public IActionResult Index()
        {
            var webActivators = _context.WebActivators;
            return View(webActivators.ToList());
        }

        // GET: ShortCodes/Create
        public IActionResult Create()
        {
            var newWebActivator = new WebActivatorViewModel
            {
                Offers = new List<SelectListItem>()
            };

            foreach (var shortCode in _context.ShortCodes.Include(s => s.Offers).ToList())
            {
                var codeSelectGroup = new SelectListGroup
                {
                    Name = shortCode.Code
                };

                foreach(var offer in shortCode.Offers)
                {
                    newWebActivator.Offers.Add(new SelectListItem 
                    { 
                        Text = offer.Name,
                        Value = offer.Id.ToString(),
                        Group = codeSelectGroup
                    });
                }
            }
            return View(newWebActivator);
        }

        // POST: ShortCodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,OfferId,UssdFallBack,ForwardClickId,PostBackUrl")] WebActivator webActivator, 
            IFormFile banner)
        {

            if (ModelState.IsValid)
            {
                if (banner != null)
                {
                    SaveBanner(banner, webActivator);
                }

                webActivator.CreatedAt = DateTime.Now;
                webActivator.LastUpdated = DateTime.Now;

                _context.Add(webActivator);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(webActivator);
        }

        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var webActivator = await _context.WebActivators
                .FirstOrDefaultAsync(m => m.Id == id);
            if (webActivator == null)
            {
                return NotFound();
            }

            return View(webActivator);
        }

        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var webActivator = await _context.WebActivators.FindAsync(id);
            if (webActivator == null)
            {
                return NotFound();
            }

            var webActivatorViewModel = new WebActivatorViewModel
            {
                Offers = new List<SelectListItem>(),
                WebActivator = webActivator
            };

            foreach (var shortCode in _context.ShortCodes.Include(s => s.Offers).ToList())
            {
                var codeSelectGroup = new SelectListGroup
                {
                    Name = shortCode.Code
                };

                foreach (var offer in shortCode.Offers)
                {
                    webActivatorViewModel.Offers.Add(new SelectListItem
                    {
                        Text = offer.Name,
                        Value = offer.Id.ToString(),
                        Group = codeSelectGroup,
                        Selected = offer.Id.Equals(webActivator.OfferId)
                    });
                }
            }

            return View(webActivatorViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,OfferId,UssdFallBack,ForwardClickId,PostBackUrl")] WebActivator webActivator, 
            IFormFile banner)
        {
            if (id != webActivator.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (banner != null)
                    {
                        SaveBanner(banner, webActivator);
                    }
                    webActivator.LastUpdated = DateTime.Now;
                    _context.Update(webActivator);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WebActivatorExists(webActivator.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(webActivator);
        }

        [AllowAnonymous]
        public IActionResult BannerImage(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var webActivator = _context.WebActivators.FirstOrDefault(m => m.Id == id);

            if (webActivator == null)
            {
                return NotFound();
            }

            var file = Path.Combine(_imageStorageDirectory, "banners", webActivator.Banner);
            
            return PhysicalFile(file, "image/*");
        }
        
        [AllowAnonymous]
        public async Task<IActionResult> Lobby(long? id, string clickId, string campaignId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var webActivator = await _context.WebActivators.Include(wa => wa.Offer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (webActivator == null)
            {
                return NotFound();
            }

            var viewModel = new WebActivatorLobbyViewModel
            {
                WebActivator = webActivator,
                SubscriptionLink = "#"
            };

            if (!string.IsNullOrEmpty(webActivator.UssdFallBack))
            {
                var subscriptionLink = "tel:" + webActivator.UssdFallBack;

                if (!string.IsNullOrEmpty(clickId))
                {

                    var webActivationClick = new WebActivationClick
                    {
                        ClickId = clickId,
                        OfferCode = webActivator.Offer.OfferCode,
                        Converted = false,
                        WebActivatorId = webActivator.Id,
                        CreatedAt = DateTime.Now,
                        LastUpdated = DateTime.Now
                    };

                    _context.Add(webActivationClick);
                    _context.SaveChanges();

                    if (webActivator.ForwardClickId)
                    {
                        viewModel.ClickId = webActivationClick.Id;
                        subscriptionLink = subscriptionLink[0..^1] + "*" + webActivationClick.Id.ToString() + "#";
                    }
                }
                

                viewModel.SubscriptionLink = subscriptionLink[0..4] + HttpUtility.UrlEncode(subscriptionLink[4..^0]);
            }

            return View(viewModel);
        }

        private bool WebActivatorExists(long id)
        {
            return _context.WebActivators.Any(e => e.Id == id);
        }

        private void SaveBanner(IFormFile banner, WebActivator webActivator)
        {
            if (banner.Length > _minimumImageBytes && banner.ContentType.Contains("image"))
            {
                //TODO: Validate file is actual image using file contents

                if (!Directory.Exists(_imageStorageDirectory))
                {
                    throw new DirectoryNotFoundException("Image Storage Directory Not Found!");
                }
                var bannersDirectory = _imageStorageDirectory + "/" + _bannerImagesFolderName;
                
                if (!Directory.Exists(bannersDirectory))
                {
                    Directory.CreateDirectory(bannersDirectory);
                }

                var newBannerName = Path.GetRandomFileName();
                var bannerFilePath = Path.Combine(bannersDirectory, newBannerName);

                using var stream = System.IO.File.Create(bannerFilePath);
                banner.CopyToAsync(stream).Wait();

                webActivator.Banner = newBannerName;
            }
        }

    }
}