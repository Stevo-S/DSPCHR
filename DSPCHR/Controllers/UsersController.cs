using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSPCHR.Data;
using DSPCHR.Models;
using DSPCHR.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DSPCHR.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = dbContext;
        }
        // GET: Users
        public ActionResult Index()
        {
            return View(_userManager.Users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(string id)
        {
            var user = _context.Users.Include(u => u.ApplicationUserOffers).ThenInclude(auo => auo.Offer).
                ThenInclude(o => o.ShortCode).FirstOrDefault(u => u.Id.Equals(id));
            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                PasswordHash = user.PasswordHash,
                CreatedAt = user.CreatedAt,
                LastUpdated = user.LastUpdated,
                Offers = new List<UserAllowedOffer>(),
                Roles = _userManager.GetRolesAsync(user).Result
                            .Select(r => new UserRole 
                            {
                                Name = r
                            }).ToList()
            };

            if (user.ApplicationUserOffers != null)
            {
                foreach (var userOffer in user.ApplicationUserOffers)
                {
                    userViewModel.Offers.Add(
                        new UserAllowedOffer
                        {
                            ShortCode = userOffer.Offer.ShortCode.Code,
                            OfferName = userOffer.Offer.Name
                        }
                        );
                }
            }

            return View(userViewModel);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            var newUser = new UserViewModel
            {
                Offers = _context.Offers.Select(o => new UserAllowedOffer
                {
                    OfferId = o.Id,
                    ShortCode = o.ShortCode.Code,
                    OfferName = o.Name,
                    Allowed = false
                }).ToList(),

                Roles = _roleManager.Roles.Select(r => new UserRole
                {
                    Id = r.Id,
                    Name = r.Name,
                    Assigned = false
                }).ToList()
            };
            return View(newUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserViewModel newUser, long[] allowedOffers, string[] assignedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = newUser.UserName,
                    Email = newUser.Email,
                    PhoneNumber = newUser.PhoneNumber,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.Now,
                    LastUpdated = DateTime.Now
                };

                var IR = _userManager.CreateAsync(user, newUser.Password).Result;
                if (IR.Succeeded)
                {
                    IR = _userManager.AddToRolesAsync(user, assignedRoles).Result;
                }

                if (IR.Succeeded)
                {
                    UpdateAllowedOffers(allowedOffers, user);
                }

                _context.SaveChanges();

                return RedirectToAction(nameof(Details), new { user.Id });
            }
            return View(newUser);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(string id)
        {
            var user = _context.Users.Include(u => u.ApplicationUserOffers).FirstOrDefault(u => u.Id.Equals(id));
            var userRoles = _userManager.GetRolesAsync(user).Result;

            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,
                Offers = new List<UserAllowedOffer>(),
                Roles = _roleManager.Roles.Select(r => new UserRole
                {
                    Id = r.Id,
                    Name = r.Name,
                    Assigned = userRoles.Contains(r.Name)
                }).ToList()
            };

            var userOfferIds = new HashSet<long>();

            if (user.ApplicationUserOffers != null)
            {
                userOfferIds = new HashSet<long>(user.ApplicationUserOffers.Select(auo => auo.OfferId));
            }

            foreach (var offer in _context.Offers.Include(o => o.ShortCode))
            {
                userViewModel.Offers.Add(
                new UserAllowedOffer
                {
                    ShortCode = offer.ShortCode.Code,
                    OfferId = offer.Id,
                    OfferName = offer.Name,
                    Allowed = userOfferIds.Contains(offer.Id)
                });
            }

            return View(userViewModel);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, UserViewModel editedUser, long[] allowedOffers, string[] assignedRoles)
        {
            IdentityResult IR = null;
            ApplicationUser user = null;

            if (ModelState.IsValid)
            {
                user = _userManager.FindByIdAsync(id).Result;

                user.UserName = editedUser.UserName;
                user.Email = editedUser.Email;
                user.PhoneNumber = editedUser.PhoneNumber;
                user.LastUpdated = DateTime.Now;

                IR = _userManager.UpdateAsync(user).Result;
            }

            if (IR.Succeeded)
            {
                var currentRoles = _userManager.GetRolesAsync(user).Result;
                var removedRoles = currentRoles.Except(assignedRoles);
                var newRoles = assignedRoles.Except(currentRoles);

                IR = _userManager.RemoveFromRolesAsync(user, removedRoles).Result;

                if (IR.Succeeded)
                {
                    IR = _userManager.AddToRolesAsync(user, newRoles).Result;
                }
            }

            if (IR.Succeeded)
            {
                UpdateAllowedOffers(allowedOffers, user);
                _context.SaveChanges();
                return RedirectToAction(nameof(Details), new { user.Id });
            }


            return View();
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private void UpdateAllowedOffers(long[] allowedOffers, ApplicationUser user)
        {
            // Reload user using context because UserManager does not load related data
            user = _context.Users.Include(u => u.ApplicationUserOffers).SingleOrDefault(u => u.Id.Equals(user.Id));

            if (allowedOffers == null)
            {
                user.ApplicationUserOffers = new List<ApplicationUserOffer>();
                return;
            }

            //var allowedOffersSet = new HashSet<string>(allowedOffers);
            var userOffers = new HashSet<long>();

            if (user.ApplicationUserOffers == null)
            {
                user.ApplicationUserOffers = new List<ApplicationUserOffer>();
            }
            else
            {
                userOffers = new HashSet<long>(user.ApplicationUserOffers.Select(uo => uo.OfferId));
            }

            foreach (var offer in _context.Offers)
            {
                if (allowedOffers.Contains(offer.Id))
                {
                    // If offer is not already allowed for the user, then allow it
                    if (!userOffers.Contains(offer.Id))
                    {
                        user.ApplicationUserOffers.Add(
                            new ApplicationUserOffer
                            {
                                ApplicationUserId = user.Id,
                                OfferId = offer.Id
                            });
                    }
                }
                else
                {
                    // If offer was allowed for the user but has now been revoked
                    if (userOffers.Contains(offer.Id))
                    {
                        ApplicationUserOffer revokedOffer = user.ApplicationUserOffers.FirstOrDefault(auo => auo.OfferId == offer.Id);
                        _context.Remove(revokedOffer);
                    }
                }
            }
        }
    }
}