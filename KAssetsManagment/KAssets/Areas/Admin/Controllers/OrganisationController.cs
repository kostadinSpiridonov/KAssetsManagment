using KAssets.Areas.Admin.Models;
using KAssets.Controllers;
using KAssets.Filters;
using KAssets.Models;
using KAssets.Resources.Translation;
using KAssets.Resources.Translation.OrganisationTr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KAssets.Areas.Admin.Controllers
{
    [Authorize]
    public class OrganisationController : BaseController
    {
        // GET: Get all organisations
        [HttpGet]
        public ActionResult GetAll()
        {
            var organisations = this.organisationService.GetAll();
            var viewModel = organisations.ToList().ConvertAll(
                x => new OrganisationViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                });

            return View(viewModel);
        }

        //GET: Add a new organisation
        [RightCheck(Right = "Low admin")]
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        //POST: Add a new organisation
        [RightCheck(Right = "Low admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(OrganisationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!model.EmaiClient.Contains("@outlook.com"))
            {
                this.ModelState.AddModelError("", OrganisationTr.EmailMustBeOutlook);
                return View(model);
            }

            //Check is there another organisation with the same name
            if (this.organisationService.IsExist(model.Name))
            {
                this.ModelState.AddModelError("", OrganisationTr.OrganisationExist);
                return View(model);
            }

            this.organisationService.Add(
                new Organisation
                {
                    Name = model.Name,
                    Address = model.Address,
                    EmailClient = model.EmaiClient,
                    EmailClientPassword = KAssets.Controllers.StaticFunctions.RSAALg.Encryption(model.EmailClientPassword)
                });

            return Redirect("/Admin/Organisation/GetAll");
        }

        //GET: Get a organisation's details
        [HttpGet]
        public ActionResult Details(int id)
        {
            var organisation = this.organisationService.GetById(id);
            var viewModel = new OrganisationViewModel
            {
                Id = organisation.Id,
                Name = organisation.Name,
                Address = organisation.Address
            };

            return View(viewModel);
        }

        //GET: Edit a organisation
        [RightCheck(Right = "Low admin")]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var organisation = this.organisationService.GetById(id);

            var viewModel = new EditOrganisationViewModel
            {
                Id = organisation.Id,
                Name = organisation.Name,
                Address = organisation.Address,
                EmaiClient = organisation.EmailClient,
                EmailClientPassword = ""
            };

            return View(viewModel);
        }

        //POST: Edit a organisation
        [RightCheck(Right = "Low admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditOrganisationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            if (!model.EmaiClient.Contains("@outlook.com"))
            {
                this.ModelState.AddModelError("", OrganisationTr.EmailMustBeOutlook);
                return View(model);
            }

            //Check is there another organisation with the same name
            if (this.organisationService.IsExistUpdate(model.Name, model.Id))
            {
                this.ModelState.AddModelError("", OrganisationTr.OrganisationExist);
                return View(model);
            }

            this.organisationService.Update(
                new Organisation
                {
                    Id = model.Id,
                    Name = model.Name,
                    Address = model.Address,
                    EmailClient = model.EmaiClient,
                    EmailClientPassword = KAssets.Controllers.StaticFunctions.RSAALg.Encryption(model.EmailClientPassword)
                });

            return Redirect("/Admin/Organisation/Details/" + model.Id);
        }

        //POST: Delete a organisation
        [RightCheck(Right = "Low admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            if (id != 0)
            {
                try
                {
                    this.organisationService.Remove(id);
                }
                catch
                {
                    TempData["DelError"] = Common.Error;
                    return Redirect("/Admin/Organisation/Details/"+id);
                };
            }

            return RedirectToAction("GetAll");
        }
    }
}