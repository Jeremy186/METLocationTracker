using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MapsProposal.Models;
using System.Net;
using MapsProposal.DAL;
using System.Threading.Tasks;

namespace MapsProposal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private MapsProposalContext db = new MapsProposalContext();

        public AdminController()
        {
        }

        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Admin
        public ActionResult Index()
        {
            var model = new AdminViewModel
            {
                Users = UserManager.Users
            };
            return View(model);
        }

        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = UserManager.FindById<ApplicationUser, string>(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var userLocations = db.Locations.Where(l => l.UserId.ToString() == user.Id);
            Role role = new Role();
            if (UserManager.IsInRole(user.Id, "Admin"))
                role = Role.Admin;
            else
                role = Role.User;
            var model = new DetailsViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = role,
                Locations = userLocations
            };
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(string id, DetailsViewModel model)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            
          
            var userToUpdate = await UserManager.FindByIdAsync(id);

            var role = model.Role;
            
            
            return RedirectToAction("Edit", "Admin");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}