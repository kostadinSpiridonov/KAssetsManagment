using KAssets.Filters;
using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using KAssets.Resources.Translation.SiteTr;
using KAssets.Controllers;
using KAssets.Areas.Admin.Models;

namespace KAssets.Areas.Admin.Controllers
{
    [Authorize]
    public class SiteController : BaseController
    {
        //GET: Get all sites for a organisation
        [HttpGet]
        public ActionResult GetAll(int id)
        {
            var sites = this.siteService.GetForOrganisation(id).
                        ToList().ConvertAll(
                            x => new SiteViewModel
                            {
                                Id = x.Id,
                                Name = x.Name
                            });

            var viewModel = new ShowSitesViewModel
            {
                OrganisationId = id,
                Sites = sites
            };

            return View(viewModel);
        }

        //GET: Get all sites for a organisation
        [HttpGet]
        public ActionResult GetAllPartial()
        {
            var id = this.userService.GetById(this.User.Identity.GetUserId()).Site.OrganisationId;
            var sites = this.siteService.GetForOrganisation(id).
                        ToList().ConvertAll(
                            x => new SiteViewModel
                            {
                                Id = x.Id,
                                Name = x.Name
                            });

            var viewModel = new ShowSitesViewModel
            {
                OrganisationId = id,
                Sites = sites
            };

            return PartialView(viewModel);
        }

        // GET: Add a site to organisation 
        [RightCheck(Right = "Low admin")]
        [HttpGet]
        public ActionResult Add(int id)
        {
            var viewModel = new AddSiteViewModel
            {
                OrganisationId = id
            };

            return View(viewModel);
        }

        //POST: Add a site to organisation
        [RightCheck(Right = "Low admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddSiteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Check is there another site with the same name
            if (this.siteService.IsExist(model.Name))
            {
                this.ModelState.AddModelError("", SiteTr.ExistSite);
                return View(model);
            }

            this.siteService.Add(new Site
                {
                    Id = model.Id,
                    Name = model.Name,
                    OrganisationId = model.OrganisationId
                });

            return Redirect("/Admin/Organisation/Details/" + model.OrganisationId);
        }

        //GET: Site details
        [HttpGet]
        public ActionResult Details(int id)
        {
            var site = this.siteService.GetById(id);

            var viewModel = new SiteViewModel
            {
                Id = site.Id,
                Name = site.Name,
                OrganisationId=site.OrganisationId,
                OrganisationName=site.Organisation.Name
            };

            return View(viewModel);
        }

        //GET: Edit a site
        [RightCheck(Right = "Low admin")]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var site = this.siteService.GetById(id);

            var viewModel = new EditSiteViewModel
            {
                Id = site.Id,
                Name = site.Name
            };

            return View(viewModel);
        }

        //POST: Edit a site
        [RightCheck(Right = "Low admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditSiteViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            //Check is there another site with the same name
            if (this.siteService.IsExistUpdate(model.Name,model.Id))
            {
                this.ModelState.AddModelError("", SiteTr.ExistSite);
                return View(model);
            }

            this.siteService.Update(new Site
                {
                   Name=model.Name,
                   Id=model.Id
                });

            return Redirect("/Admin//Site/Details/" + model.Id);
        }

        //POST: Delete a site
        [RightCheck(Right = "Low admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(DeleteSiteViewMode model)
        {
            if(model.Id!=0)
            {
                this.siteService.Remove(model.Id);
            }

            return Redirect("/Admin/Organisation/Details/" + model.OrganisationId);
        }

        //GET: Get all sites in a organisation
        [HttpGet]
        public ActionResult GetSitesInOrganisation(int id)
        {
            var sites = this.siteService.GetForOrganisation(id);

            var viewModel = sites.ToList().ConvertAll(
                x => new EditSiteViewModel
                {
                    Name = x.Name,
                    Id = x.Id
                });
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }
    }
}