using KAssets.Filters;
using KAssets.Models;
using KAssets.Resources.Translation.LocationsTr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KAssets.Areas.Admin.Models;
using KAssets.Controllers;
using KAssets.Areas.HelpModule.Models;

namespace KAssets.Areas.HelpModule.Controllers
{
    [Authorize]
    [RightCheck(Right = "Manage locations")]
    [HasSite]
    public class LocationController : BaseController
    {
        // GET: All locations
        [HttpGet]
        public ActionResult Index()
        {
            var userORg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var locations = this.locationService.GetAll();
            if (!this.IsMegaAdmin())
            {
                locations = locations.Where(x => x.OrganisationId == userORg).ToList();
            }

            var viewModel = locations.ToList().ConvertAll(
                x => new ShowLocationViewModel
                {
                    Code = x.Code,
                    Organisation=x.Organisation.Name
                });

            return View(viewModel);
        }

        //GET: Add a new location
        [HttpGet]
        public ActionResult Add()
        {
            var viewModel = new AddLocationViewModel();
          
            if (!this.IsMegaAdmin())
            {
                viewModel.SeletedOrganisationId = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            }
            else
            {
                viewModel.Organisations = this.organisationService.GetAll().ToList()
                    .ConvertAll(x => new OrganisationViewModel
                    {
                        Id = x.Id,
                        Name = x.Name
                    });
            }
            return View(viewModel);
        }

        //POST: Add a new location
        [HttpPost]
        public ActionResult Add(AddLocationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (!this.IsMegaAdmin())
                {
                    model.SeletedOrganisationId = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
                }
                else
                {
                    model.Organisations = this.organisationService.GetAll().ToList()
                        .ConvertAll(x => new OrganisationViewModel
                        {
                            Id = x.Id,
                            Name = x.Name
                        });
                }
                return View(model);
            }

            //Check is there a location with the same code
            if (this.locationService.Exist(model.Code))
            {
                if (!this.IsMegaAdmin())
                {
                    model.SeletedOrganisationId = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
                }
                else
                {
                    model.Organisations = this.organisationService.GetAll().ToList()
                        .ConvertAll(x => new OrganisationViewModel
                        {
                            Id = x.Id,
                            Name = x.Name
                        });
                }
                this.ModelState.AddModelError("", LocationTr.ExistLocation);
                return View(model);
            }

            if ((model.Longitude != null && model.Latitude != null) ||
               (model.Country != null && model.Town != null && model.Street != null))
            {
                this.locationService.Add(new Location
                    {
                        Code = model.Code,
                        Country = model.Country,
                        Latitude = model.Latitude,
                        Longitude = model.Longitude,
                        Street = model.Street,
                        StreetNumber = model.StreetNumber != null ? model.StreetNumber.Value : 0,
                        Town = model.Town,
                        OrganisationId = model.SeletedOrganisationId
                    });

                return Redirect("/HelpModule/Location/Index");
            }
            ModelState.AddModelError("", LocationTr.Validate);
            return View(model);
        }

        //GET: Edit a location
        [HttpGet]
        public ActionResult Edit(string id)
        {
            var location = this.locationService.GetById(id);

            //Verify if asset is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (location.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var viewModel = new EditLocationViewModel
            {
                OldCode = location.Code,
                Country = location.Country,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Street = location.Street,
                StreetNumber = location.StreetNumber.Value,
                Town = location.Town
            };

            return View(viewModel);
        }

        //POST: Edit a location
        [HttpPost]
        public ActionResult Edit(EditLocationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if ((model.Longitude != null && model.Latitude != null) ||
                (model.Country != null && model.Town != null && model.Street != null))
            {
                this.locationService.Update((new Location
                    {
                        Code = model.OldCode,
                        Country = model.Country,
                        Latitude = model.Latitude,
                        Longitude = model.Longitude,
                        Street = model.Street,
                        StreetNumber = model.StreetNumber != null ? model.StreetNumber.Value : 0,
                        Town = model.Town
                    }));
                return Redirect("/HelpModule/Location/Index");
            }
            ModelState.AddModelError("", LocationTr.Validate);
            return View(model);
        }

        //GET: Location details
        [HttpGet]
        public ActionResult Details(string id)
        {
            var location = this.locationService.GetById(id);

            //Verify if asset is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (location.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var viewModel = new LocationViewModel
            {
                Code = location.Code,
                Country = location.Country,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Street = location.Street,
                StreetNumber = location.StreetNumber,
                Town = location.Town
            };

            return View(viewModel);
        }

        //GET: Delete location
        public ActionResult Delete(string id)
        {
            var location = this.locationService.GetById(id);

            //Verify if asset is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (location.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            try
            {
                this.locationService.Remove(id);
            }
            catch
            {
                this.TempData["DelError"] = LocationTr.LocationIsUsed;
            }
            return Redirect("/HelpModule/Location/Index");

        }
    }
}