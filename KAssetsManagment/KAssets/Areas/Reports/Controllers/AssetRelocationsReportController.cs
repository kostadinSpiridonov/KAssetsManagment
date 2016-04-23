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
    [RightCheck(Right = "Report for asset relocations")]
    public class AssetRelocationsReportController : BaseController
    {
        // GET: Create report
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        //GET: Choose asset
        [HttpGet]
        public ActionResult ChooseAsset()
        {
            var assets = this.assetService.GetAll().Where(x => x.Status == "Active");
            if (!this.IsMegaAdmin())
            {
                var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
                assets = assets.Where(x => x.Site.OrganisationId == userOrg).ToList();
            }

            var viewModel = assets.ToList().ConvertAll(
                x => new ChooseAssetViewModel
                {
                    AssetModel = x.Model,
                    Brand = x.Brand,
                    InventoryNumber = x.InventoryNumber,
                    LocationCode = x.Location != null ? x.LocationId : "",
                    UserId = x.User != null ? x.UserId : ""

                });

            return PartialView(viewModel);
        }

        //POST: Result
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Result(CreateAssetRelocationsReportViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }

            var ass = this.assetService.GetById(model.SelectedAsset);
          
            if (!this.IsMegaAdmin())
            {
                //Verify if asset is from user organisation
                if (ass.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var relocations = this.requestForRelocationService.GetAll()
                .Where(x => x.AssetId == model.SelectedAsset)
                .Where(x => x.AreAssetGiven)
                .Where(x => x.IsApproved)
                .Where(x => x.IsFinished)
                .Where(x => x.SeenFromApprover);

            var viewModel = new AssetRelocationsReportResult
                {
                    History = new List<AssetRelocationReportViewModel>()
                };

            viewModel.AssetDetails = CreateAssetViewModel(model.SelectedAsset);

            foreach (var item in relocations)
            {
                var viewModelItem = new AssetRelocationReportViewModel
                {
                    DateOfGiven = item.PackingSlip.DateOfGiven == DateTime.MinValue ? " - " : item.PackingSlip.DateOfGiven.ToString(),
                    DateOfReceived = item.PackingSlip.DateOfReceived == DateTime.MinValue ? " - " : item.PackingSlip.DateOfReceived.ToString(),
                    ToSite = item.ToSite.Name,
                    FromSite = item.OldSiteName
                };

                if (item.ToLocation != null)
                {
                    viewModelItem.ToLocation =
                        (item.ToLocation.Country != null ? item.ToLocation.Country : " ") + " " +
                        (item.ToLocation.Latitude != null ? item.ToLocation.Latitude : " ") + " " + " " +
                        (item.ToLocation.Longitude != null ? item.ToLocation.Longitude : " ") + " " +
                        (item.ToLocation.Town != null ? item.ToLocation.Town : " ") + " " +
                        (item.ToLocation.Street != null ? item.ToLocation.Street : " ") + " " +
                        (item.ToLocation.StreetNumber != null ? item.ToLocation.StreetNumber.Value.ToString() : " ");
                }

                if (item.ToUser != null)
                {
                    viewModelItem.ToUser =
                      (item.ToUser.FirstName != null) ?
                      (item.ToUser.FirstName + " " + item.ToUser.SecondName + " " + item.ToUser.LastName) :
                      (item.ToUser.Email);

                }

                if (item.Asset.Location != null)
                {
                    viewModelItem.FromLocation =
                        (item.Asset.Location.Country != null ? item.Asset.Location.Country : " ") + " " +
                        (item.Asset.Location.Latitude != null ? item.Asset.Location.Latitude : " ") + " " + " " +
                        (item.Asset.Location.Longitude != null ? item.Asset.Location.Longitude : " ") + " " +
                        (item.Asset.Location.Town != null ? item.Asset.Location.Town : " ") + " " +
                        (item.Asset.Location.Street != null ? item.Asset.Location.Street : " ") + " " +
                        (item.Asset.Location.StreetNumber != null ? item.Asset.Location.StreetNumber.Value.ToString() : " ");

                }

                if (item.Asset.User != null)
                {
                    viewModelItem.FromUser =
                      (item.Asset.User.FirstName != null) ?
                      (item.Asset.User.FirstName + " " + item.Asset.User.SecondName + " " + item.Asset.User.LastName) :
                      (item.Asset.User.Email);

                }
                viewModel.History.Add(viewModelItem);
            }

            return View(viewModel);
        }

        //Create asset viewmodel from base model
        public AssetDetailsViewModel CreateAssetViewModel(string id)
        {
            var asset = this.assetService.GetById(id);

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

            if (asset.User != null)
            {
                viewModel.UserName = (asset.User.FirstName != null) ?
                    asset.User.FirstName + " " + asset.User.SecondName + " " + asset.User.LastName :
                    asset.User.Email;
            }

            viewModel.SiteName = asset.Site.Name;

            return viewModel;
        }
    }
}