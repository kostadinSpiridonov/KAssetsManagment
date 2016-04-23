using KAssets.Areas.Reports.Models;
using KAssets.Controllers;
using KAssets.Filters;
using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KAssets.Areas.AssetsActions.Models;
using KAssets.Areas.HelpModule.Models;

namespace KAssets.Areas.Reports.Controllers
{
    [Authorize]
    [HasSite]
    [RightCheck(Right = "Report for assets by status")]
    public class AssetStatusController : BaseController
    {
        // GET: Create report
        [HttpGet]
        public ActionResult CreateReport()
        {
            var viewModel = new CreateAssetStatusReport
            {
                Statuses = new List<string>
                {
                    "Active",
                    "Scrapped",
                    "Renovating"
                }
            };
            return View(viewModel);
        }

        //POST: Get result
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Result(CreateAssetStatusReport model)
        {
            var assets = this.assetService.GetAll();
            if (!this.IsMegaAdmin())
            {
                var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
                assets = assets.Where(x => x.Site.OrganisationId == userOrg).ToList();
            }
            var viewModel = assets
                .Where(x => x.Status == model.SelectedStatus)
                .ToList()
                .ConvertAll(x =>
                    new AssetViewModel
                    {
                        AssetModel = x.Model,
                        Brand = x.Brand,
                        InventoryNumber = x.InventoryNumber,
                        SiteName = x.Site.Name
                    });

            return View(viewModel);
        }

        //GEТ: Details asset
        [HttpGet]
        public ActionResult Details(string id)
        {
            var asset = this.assetService.GetById(id);
            
            if (!this.IsMegaAdmin())
            {
                //Verify if asset is from user organisation
                if (asset.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var viewModel = new AssetDetailsViewModel
            {
                Brand = asset.Brand,
                Guarantee = asset.Guarantee,
                InventoryNumber = asset.InventoryNumber,
                ItemModel = asset.Model,
                Price = asset.Price.Value,
                Producer = asset.Producer,
                Type = asset.Type,
                Status = asset.Status,
                Currency = asset.Price.Currency.Code
            };

            if (asset.Location != null)
            {
                viewModel.Location = new LocationViewModel
                {
                    Country = asset.Location.Country != null ? asset.Location.Country : "",
                    Latitude = asset.Location.Latitude != null ? asset.Location.Latitude : "",
                    Longitude = asset.Location.Longitude != null ? asset.Location.Longitude : "",
                    Street = asset.Location.Street != null ? asset.Location.Street : "",
                    StreetNumber = asset.Location.StreetNumber != null ? asset.Location.StreetNumber.Value : 0,
                    Town = asset.Location.Town != null ? asset.Location.Town : "",

                };
            }
            viewModel.SiteName = asset.Site.Name;

            if (asset.User != null)
            {
                viewModel.UserName = (asset.User.FirstName != null) ?
                    asset.User.FirstName + " " + asset.User.SecondName + " " + asset.User.LastName :
                    asset.User.Email;
            }

            return View(viewModel);
        }
    }
}