using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoreLinq;
using KAssets.Filters;
using KAssets.Resources.Translation.UserSiteTr;
using KAssets.Controllers;

namespace KAssets.Areas.Admin.Controllers
{
    [Authorize]
    public class UserSiteController : BaseController
    {
        // GET: Get users for a site
        [HttpGet]
        public ActionResult GetUsersForSite(int id)
        {
            var users = this.siteService.GetById(id).Users.
                ToList().ConvertAll(x =>
                new ShowUserViewModel
                {
                    Id = x.Id,
                    Name = (x.FirstName != null && x.SecondName != null && x.LastName != null) ?
                        x.FirstName + " " + x.SecondName + " " + x.LastName : x.Email
                });

            var viewMode = new ShowUsersViewModel
            {
                SiteId = id,
                Users = users
            };

            return View(viewMode);
        }

        //GET: Add users to a site
        [RightCheck(Right = "Low admin")]
        [HttpGet]
        public ActionResult Add(int id)
        {
            var users = this.userService.GetAll().ToList().Where(x=>x.Status=="Active");
            var availableUsers = this.siteService.GetById(id).Users.ToList();
            users = users.ExceptBy(availableUsers, x => x.Id).ToList();

           var usersViewModel = users.ToList().ConvertAll(x =>
           new ChooseUserViewModel
           {
               Id = x.Id,
               Name = (x.FirstName != null && x.SecondName != null && x.LastName != null) ?
                   x.FirstName + " " + x.SecondName + " " + x.LastName : x.Email,
               IsSelected = false
           });

            var viewModel = new AddUserToSiteViewModel
            {
                SiteId = id,
                Users = usersViewModel
            };

            return View(viewModel);
        }

        //POST: Add users to a site
        [RightCheck(Right = "Low admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddUserToSiteViewModel model)
        {
            //Chck are there selected users
            if (!model.Users.Any(x => x.IsSelected))
            {
                this.ModelState.AddModelError("", UserSiteTr.PleaseSelectUser);
                return View(model);
            }

            var userIds = model.Users.Where(x => x.IsSelected).Select(x => x.Id).ToList();
            this.siteService.AddUsersToSite(model.SiteId, userIds);

            return Redirect("/Admin/Site/Details/" + model.SiteId);
        }

        //POST: Remove  a user from site
        [RightCheck(Right = "Low admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(string userId, int siteId)
        {
            if (userId != null&&userId!="" && siteId != 0)
            {
                this.siteService.RemoveUserFromSite(siteId, userId);
            }

            return Redirect("/Admin/Site/Details/" + siteId);
        }
    }
}