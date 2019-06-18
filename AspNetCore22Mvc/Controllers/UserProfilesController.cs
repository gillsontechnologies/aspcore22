using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore22Mvc.Areas.Identity.Pages.Account;
using AspNetCore22Mvc.Data;
using AspNetCore22Mvc.Models;
using AspNetCore22Mvc.Models.AccountViewmodels;
using AspNetCore22Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AspNetCore22Mvc.Controllers.ManageProfiles
{
    public class UserProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        //private readonly ILogger _logger;

        public UserProfilesController(
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
         ApplicationDbContext context)
        {
            _userManager = userManager;
            _emailSender = emailSender;
           
            _context = context;
        }

        // GET: UserProfiles
        [Authorize(Roles = "Super")]
        public async Task<IActionResult> Index()
        {

            var SuperUser = _context.Users
                .Join(_context.UserRoles, usr => usr.Id, role => role.UserId, (usr, role) => new
                {
                    role.RoleId,
                    usr
                })
                .Join(_context.Roles, ur => ur.RoleId, rl => rl.Id, (ur, rl) => new
                {
                    rl.Name,
                    ur.usr
                })
                .Where(x => x.Name == "Super")
                .Select(x => x.usr).FirstOrDefault();
            var userList = _context.Users.Where(x => x != SuperUser).ToListAsync();
            return View(await userList);
        }

        // GET: UserProfiles/Create
        [Authorize(Roles = "Super")]
        public IActionResult Create()
        {
            //ViewData["Reseller_Customer"] = new SelectList(_context.Customer, "CustomerId", "TradingName");
            //ViewData["Reseller"] = new SelectList(_context.Reseller, "ResellerId", "TradingName");
            ViewData["Roles"] = new SelectList(_context.Roles.Where(x => x.Name != "Super"), "Id", "Name");
            return View();
        }

        // POST: UserProfiles/Create
        [HttpPost]
        [Authorize(Roles = "Super")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterViewModel model, bool isApproved, string RoleId)

        {
            if (ModelState.IsValid)
            {
               
                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    Name = model.Name,
                    Surname = model.Surname,
                    Organisation = model.Organisation,
                    Position = model.Position,
                    //PhoneNumber = model.PhoneNumber
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Errors != null)
                {
                    if (result.Errors.Count() != 0)
                    {
                        
                        ModelState.AddModelError("Email", result.Errors.FirstOrDefault().Description);
                        ViewData["Roles"] = new SelectList(_context.Roles.Where(x => x.Name != "Super"), "Id", "Name", RoleId);
                        return View(model);
                    }
                }
                if (result.Succeeded)
                {
                    //_logger.LogInformation("User created a new account with password.");
                    string userId = _context.Users.Where(x => x.UserName == User.Identity.Name).Select(x => x.Id).FirstOrDefault();
                    //UserActivityManagement.addUserActivity(userId, User.Identity.Name, "Create", Convert.ToString(user.Id), "User", _context);
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    //await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    //Pre-approve User Profile
                    if (RoleId != null)
                    {
                        await ApproveForm(user.Id, isApproved, RoleId);
                    }

                    return RedirectToAction(nameof(UserProfilesController.Index), "UserProfiles");
                }
            }

            // If we got this far, something failed, redisplay form
            
            ViewData["Roles"] = new SelectList(_context.Roles.Where(x => x.Name != "Super"), "Id", "Name", RoleId);
            return View(model);
        }

        // GET: UserProfiles/Edit/5
        [Authorize(Roles = "Super")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProfile = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);
            if (userProfile == null)
            {
                return NotFound();
            }
            var userRoleId = await _context.UserRoles.Where(x => x.UserId == userProfile.Id).Select(x => x.RoleId).FirstOrDefaultAsync();
            
           

            ViewData["Roles"] = new SelectList(_context.Roles.Where(x => x.Name != "Super"), "Id", "Name", userRoleId);

            return View(userProfile);
        }

        // POST: UserProfiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Super")]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Email,PhoneNumber,Name,Organisation,Position,Surname")] ApplicationUser applicationUser, string lockStatus)
        {
            if (id != applicationUser.Id)
            {
                return Json("User Not Found!");
            }

            var user = _context.Users.Where(x => x.Id == applicationUser.Id).FirstOrDefault();
            user.PhoneNumber = applicationUser.PhoneNumber;
            user.Name = applicationUser.Name;
            user.Organisation = applicationUser.Organisation;
            user.Position = applicationUser.Position;
            user.Surname = applicationUser.Surname;
            user.Email = applicationUser.Email;
  
            if ("ON".Equals(lockStatus, StringComparison.OrdinalIgnoreCase))
            {
                user.LockoutEnd = null;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        string userId = _context.Users.Where(x => x.UserName == User.Identity.Name).Select(x => x.Id).FirstOrDefault();
                        //UserActivityManagement.addUserActivity(userId, User.Identity.Name, "Edit", Convert.ToString(user.Id), "User", _context);
                        //_logger.LogInformation("User updated.");
                        return Json("Updated Successfully!");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserProfileExists(user.Id))
                    {
                        return Json("User Not Found!");
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return Json("Please check required fields!");
        }

        [HttpPost]
        [Authorize(Roles = "Super")]
        public async Task<IActionResult> ApproveForm(string id, bool isApproved, string RoleId)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Json("User not found!");
                }

                //Assign User Role
                var tblRole = _context.UserRoles.Where(x => x.UserId == id).FirstOrDefault();
                if (tblRole != null)
                {
                    _context.Remove(tblRole);
                    await _context.SaveChangesAsync();
                }

                _context.UserRoles.Add(new IdentityUserRole<string>() { RoleId = RoleId, UserId = id });
                await _context.SaveChangesAsync();

                //Approve User Account
                var user = _context.Users.Where(x => x.Id == id).FirstOrDefault();
                user.isApproved = isApproved;
                _context.Update(user);

                await _context.SaveChangesAsync();

                //EmailSender emailSender = new EmailSender();
                //await emailSender.SendEmailAsync(user.Email, "Your account is approved!", "Your account registration on <strong> Service & Asset Tracker</strong> is accepted and approved.<br />Now you are able to login.");

                return Json("Updated successfully!");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Super")]
        public async Task<IActionResult> ChangePassword(string id, string Password)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Json("User not found!");
            }
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, Password);
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return Json("Something went wrong, try again!");
            }
            return Json("Password changed successfully!");
        }

        // GET: UserProfiles/Delete/5
        [Authorize(Roles = "Super")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProfile = await _context.Users
                .SingleOrDefaultAsync(m => m.Id == id);
            if (userProfile == null)
            {
                return NotFound();
            }

            return View(userProfile);
        }

        // POST: UserProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Super")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            //Delete Role
            var tblRole = _context.UserRoles.Where(x => x.UserId == id).FirstOrDefault();
            if (tblRole != null)
            {
                _context.Remove(tblRole);
                await _context.SaveChangesAsync();
            }

            //Delete Profile
            var userProfile = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);

            _context.Users.Remove(userProfile);
            await _context.SaveChangesAsync();

            string userId = _context.Users.Where(x => x.UserName == User.Identity.Name).Select(x => x.Id).FirstOrDefault();
            //UserActivityManagement.addUserActivity(userId, User.Identity.Name, "Delete", id, "User", _context);

            return RedirectToAction(nameof(Index));
        }

        private bool UserProfileExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}