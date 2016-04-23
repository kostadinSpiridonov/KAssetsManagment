using KAssets.Areas.Admin.Models;
using KAssets.Filters;
using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using KAssets.Resources;
using KAssets.Controllers;
using KAssets.Areas.HelpModule.Models;

namespace KAssets.Areas.Admin.Controllers
{
    [RightCheck(Right = "Low admin")]
    public class UserController : BaseController
    {

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public UserController()
        {
        }

        public UserController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Get all users
        [HttpGet]
        public ActionResult GetAll()
        {
            var users = this.userService.GetAll().Where(x => x.Status == "Active");

            var viewModel = users.ToList().ConvertAll(
                x => new UserViewModel
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    SecondName = x.SecondName,
                    LastName = x.LastName,
                    Email = x.Email
                });

            return View(viewModel);
        }

        // GET: Get all blocked users
        [HttpGet]
        public ActionResult GetAllBlocked()
        {
            var users = this.userService.GetAll().Where(x => x.Status == "Blocked");
            var viewModel = users.ToList().ConvertAll(
                x => new UserViewModel
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    SecondName = x.SecondName,
                    LastName = x.LastName,
                    Email = x.Email
                });

            return View(viewModel);
        }

        //GET: Add a new user
        [HttpGet]
        public ActionResult Add()
        {
            //Get security groups
            var securityGroups = this.securityGroupService.GetAll().ToList()
                .ConvertAll(x => new SecurityGroupViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                });

            var viewModel = new RegisterViewModel();

            //Set all locations
            viewModel.Locations = new List<DropDownLocationViewModel>();

            //Convert security groups to viewmodel
            viewModel.SelectedSecurityGroups = securityGroups.ConvertAll(
                x => new SelectSecurityGroupViewModel
                {
                    IsSelected = false,
                    SecurityGroup = x
                });

            //Set all organisations
            viewModel.Organisations = this.organisationService.GetAll().ToList()
                .ConvertAll(x => new OrganisationDropDownViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                });

            return View(viewModel);
        }

        //POST: Add a new user
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(RegisterViewModel model)
        {
            //Check if site and organisation is selected
            if (model.SelectedOrganisationId != null)
            {
                if (model.SelectedSiteId == null)
                {
                    var securityGroups = this.securityGroupService.GetAll().ToList()
                    .ConvertAll(x => new SecurityGroupViewModel
                    {
                        Id = x.Id,
                        Name = x.Name
                    });

                    if (model.SelectedOrganisationId == null || model.SelectedOrganisationId == 0)
                    {
                        model.Locations = new List<DropDownLocationViewModel>();
                    }
                    else
                    {
                        model.Locations = this.locationService.GetAll()
                            .Where(x => x.OrganisationId == model.SelectedOrganisationId)
                            .ToList().
                           ConvertAll(x => new DropDownLocationViewModel
                           {
                               Code = x.Code,
                               Location = x.Country + ", " + x.Town + ", " + x.Street + ", " + x.StreetNumber.Value
                           });
                    }

                    model.SelectedSecurityGroups = securityGroups.ConvertAll(
                        x => new SelectSecurityGroupViewModel
                        {
                            IsSelected = false,
                            SecurityGroup = x
                        });

                    model.Organisations = this.organisationService.GetAll().ToList()
                        .ConvertAll(x => new OrganisationDropDownViewModel
                        {
                            Id = x.Id,
                            Name = x.Name
                        });

                    this.ModelState.AddModelError("", Resources.Translation.AdminArea.Admin.OrgOrNothing);
                    return View(model);
                }
            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, Status = "Active" };
                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Add user to selected security groups
                    this.securityGroupService.AddUserToSecurityGroups(model.Email,
                        model.SelectedSecurityGroups.Where(x => x.IsSelected)
                        .Select(x => x.SecurityGroup.Id).ToList());

                    //Add location to user
                    if (model.SelectedLocationCode != null && model.SelectedLocationCode != "")
                    {
                        this.locationService.AddLocationToUser(model.Email, model.SelectedLocationCode);
                    }

                    //Add selected organisation and site to user
                    if (model.SelectedOrganisationId != null)
                    {
                        if (model.SelectedSiteId != null)
                        {
                            this.userService.UserSiteAndOrgUpdate(model.Email, model.SelectedOrganisationId.Value, model.SelectedSiteId.Value);
                        }
                        else
                        {
                            this.userService.UserSiteAndOrgUpdate(model.Email, model.SelectedOrganisationId.Value, 0);
                        }
                    }
                    return RedirectToAction("GetAll");
                }
                AddErrors(result);
            }

            //Set viewmodel data
            var securityGroupsS = this.securityGroupService.GetAll().ToList()
                  .ConvertAll(x => new SecurityGroupViewModel
                  {
                      Id = x.Id,
                      Name = x.Name
                  });

            //Set all locations
            if (model.SelectedOrganisationId == null || model.SelectedOrganisationId == 0)
            {
                model.Locations = new List<DropDownLocationViewModel>();
            }
            else
            {
                model.Locations = this.locationService.GetAll()
                    .Where(x => x.OrganisationId == model.SelectedOrganisationId)
                    .ToList().
                   ConvertAll(x => new DropDownLocationViewModel
                   {
                       Code = x.Code,
                       Location = x.Country + ", " + x.Town + ", " + x.Street + ", " + x.StreetNumber.Value
                   });
            }

            model.SelectedSecurityGroups = securityGroupsS.ConvertAll(
                x => new SelectSecurityGroupViewModel
                {
                    IsSelected = false,
                    SecurityGroup = x
                });

            model.Organisations = this.organisationService.GetAll().ToList()
                .ConvertAll(x => new OrganisationDropDownViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                });
            return View(model);
        }

        //GET: Edit a user
        [HttpGet]
        public ActionResult Edit(string id)
        {
            var user = this.userService.GetById(id);

            if (user.Email == BaseAdmin.Email)
            {
                return View(user);
            }

            var securityGroups = this.securityGroupService.GetAll().ToList()
               .ConvertAll(x => new SecurityGroupViewModel
               {
                   Id = x.Id,
                   Name = x.Name
               });

            var viewModel = new KAssets.Areas.Admin.Models.EditUserViewModel();
            viewModel.SecurityGroups = securityGroups.ConvertAll(
                x => new SelectSecurityGroupViewModel
                {
                    IsSelected = user.SecurityGroups.Any(y => y.Id == x.Id) ? true : false,
                    SecurityGroup = x
                });

            //Set all locations
            if (user.Site != null)
            {
                viewModel.Locations = this.locationService.GetAll()
                    .Where(x => x.OrganisationId == user.Site.OrganisationId)
                    .ToList().
                   ConvertAll(x => new DropDownLocationViewModel
                   {
                       Code = x.Code,
                       Location = x.Country + ", " + x.Town + ", " + x.Street + ", " + x.StreetNumber.Value
                   });
            }
            else
            {
                viewModel.Locations = new List<DropDownLocationViewModel>();
            }

            viewModel.Organisations = this.organisationService.GetAll().ToList()
               .ConvertAll(x => new OrganisationDropDownViewModel
               {
                   Id = x.Id,
                   Name = x.Name,
                   Sites = x.Sites.ToList().ConvertAll(
                   y => new SiteViewModel
                   {
                       Id = y.Id,
                       Name = y.Name
                   })
               });

            viewModel.SelectedOrganisationId = user.Site != null ? user.Site.OrganisationId : 0;
            viewModel.SelectedSiteId = user.SiteId;
            viewModel.SelectedLocationCode = user.LocationId;
            viewModel.AboutMe = user.AboutMe;
            viewModel.Email = user.Email;
            viewModel.FirstName = user.FirstName;
            viewModel.LastName = user.LastName;
            viewModel.SecondName = user.SecondName;
            viewModel.Skype = user.Skype;
            viewModel.Id = user.Id;

            return View(viewModel);
        }

        //POST: Edit a user
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(KAssets.Areas.Admin.Models.EditUserViewModel model)
        {
            if (model.SelectedOrganisationId != null)
            {
                if (model.SelectedSiteId == null)
                {
                    var securityGroups = this.securityGroupService.GetAll().ToList()
                    .ConvertAll(x => new SecurityGroupViewModel
                    {
                        Id = x.Id,
                        Name = x.Name
                    });

                    //Set all locations
                    if (model.SelectedOrganisationId == null || model.SelectedOrganisationId == 0)
                    {
                        model.Locations = new List<DropDownLocationViewModel>();
                    }
                    else
                    {
                        model.Locations = this.locationService.GetAll()
                            .Where(x => x.OrganisationId == model.SelectedOrganisationId)
                            .ToList().
                           ConvertAll(x => new DropDownLocationViewModel
                           {
                               Code = x.Code,
                               Location = x.Country + ", " + x.Town + ", " + x.Street + ", " + x.StreetNumber.Value
                           });
                    }

                    model.SecurityGroups = securityGroups.ConvertAll(
                        x => new SelectSecurityGroupViewModel
                        {
                            IsSelected = false,
                            SecurityGroup = x
                        });

                    model.Organisations = this.organisationService.GetAll().ToList()
                        .ConvertAll(x => new OrganisationDropDownViewModel
                        {
                            Id = x.Id,
                            Name = x.Name
                        });

                    this.ModelState.AddModelError("", Resources.Translation.AdminArea.Admin.OrgOrNothing);
                    return View(model);
                }
            }
            //Check if the email is same as baseadmin email
            if (model.Email == BaseAdmin.Email || !ModelState.IsValid)
            {
                var securityGroupsS = this.securityGroupService.GetAll().ToList()
                .ConvertAll(x => new SecurityGroupViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                });

                //Set all locations
                if (model.SelectedOrganisationId == null || model.SelectedOrganisationId == 0)
                {
                    model.Locations = new List<DropDownLocationViewModel>();
                }
                else
                {
                    model.Locations = this.locationService.GetAll()
                        .Where(x => x.OrganisationId == model.SelectedOrganisationId)
                        .ToList().
                       ConvertAll(x => new DropDownLocationViewModel
                       {
                           Code = x.Code,
                           Location = x.Country + ", " + x.Town + ", " + x.Street + ", " + x.StreetNumber.Value
                       });
                }

                model.SecurityGroups = securityGroupsS.ConvertAll(
                    x => new SelectSecurityGroupViewModel
                    {
                        IsSelected = false,
                        SecurityGroup = x
                    });

                model.Organisations = this.organisationService.GetAll().ToList()
                    .ConvertAll(x => new OrganisationDropDownViewModel
                    {
                        Id = x.Id,
                        Name = x.Name
                    });
                return View(model);
            }


            var user = new ApplicationUser();

            user.AboutMe = model.AboutMe;
            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.SecondName = model.SecondName;
            user.Skype = model.Skype;
            user.Id = model.Id;

            //Remova all security groups of user
            this.securityGroupService.RemoveUserFromAllSecurityGroups(model.Id);

            //Add a new security groups to user
            this.securityGroupService.AddUserToSecurityGroups(
                model.Email, model.SecurityGroups.Where(x => x.IsSelected)
                .Select(x => x.SecurityGroup.Id).ToList());

            //Add locations to user
            this.locationService.AddLocationToUser(model.Email, model.SelectedLocationCode);

            //Create a new password of user
            if (model.Password != null && model.Password != "")
            {
                var passToken = UserManager.GeneratePasswordResetToken(model.Id);
                UserManager.ResetPassword(model.Id, passToken, model.Password);
            }

            //Change a location of user
            if (model.SelectedOrganisationId != null)
            {
                if (model.SelectedSiteId != null)
                {
                    this.userService.UserSiteAndOrgUpdate(model.Email, model.SelectedOrganisationId.Value, model.SelectedSiteId.Value);
                }
                else
                {
                    this.userService.UserSiteAndOrgUpdate(model.Email, model.SelectedOrganisationId.Value, 0);
                }
            }
            else
            {
                this.userService.UserSiteAndOrgUpdate(model.Email, 0, 0);
            }

            user.PasswordHash = this.userService.GetById(model.Id).PasswordHash;
            this.userService.Update(user);

            return Redirect("/Admin/User/Details/" + model.Id);
        }

        //GET: User details
        [HttpGet]
        public ActionResult Details(string id)
        {
            var user = this.userService.GetById(id);

            var viewModel = new UserDetailsViewModel();
            viewModel.Id = user.Id;
            viewModel.AboutMe = user.AboutMe;
            viewModel.Email = user.Email;
            viewModel.FirstName = user.FirstName;
            viewModel.LastName = user.LastName;
            viewModel.SecondName = user.SecondName;
            viewModel.Site = user.Site != null ? user.Site.Name : "";
            viewModel.Status = user.Status;
            viewModel.Organisation = user.Site != null ? user.Site.Organisation.Name : "";
            viewModel.SecurityGroups = user.SecurityGroups.ToList().
                ConvertAll(x => new SecurityGroupViewModel
                {
                    Name = x.Name,
                    Id = x.Id
                });

            viewModel.Location = new LocationViewModel();
            if (user.Location != null)
            {
                viewModel.Location = new LocationViewModel
                {
                    Country = user.Location.Country != null ? user.Location.Country : "",
                    Latitude = user.Location.Latitude != null ? user.Location.Latitude : "",
                    Longitude = user.Location.Longitude != null ? user.Location.Longitude : "",
                    Street = user.Location.Street != null ? user.Location.Street : "",
                    StreetNumber = user.Location.StreetNumber != null ? user.Location.StreetNumber.Value : 0,
                    Town = user.Location.Town != null ? user.Location.Town : ""
                };
            }
            return View(viewModel);
        }

        //POST: Delete user
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Delete(string id)
        {
            if (id != null && id != "")
            {
                var user = userService.GetById(id);

                if (user.Email == BaseAdmin.Email)
                {
                    return RedirectToAction("GetAll");
                }

                this.userService.Block(id);
            }

            return RedirectToAction("GetAll");
        }

        //POST: Delete user
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Unblock(string id)
        {
            if (id != null && id != "")
            {
                var user = userService.GetById(id);

                if (user.Email == BaseAdmin.Email)
                {
                    return RedirectToAction("GetAll");
                }

                this.userService.UnBlock(id);
            }

            return RedirectToAction("GetAll");
        }

        //Add a new errors to modelState
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        //GET: Get locations for choosing
        [HttpGet]
        public JsonResult ChooseLocations(int id)
        {
            if (id != 0)
            {
                var locations = this.locationService.GetAll()
                    .Where(x => x.OrganisationId == id).ToList();

                var viewModel = locations.ToList()
                   .ConvertAll(x => new DropDownLocationViewModel
                   {
                       Code = x.Code,
                       Location = x.Country + ", " + x.Town + ", " + x.Street + ", " + x.StreetNumber.Value
                   });

                return Json(viewModel, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }


}