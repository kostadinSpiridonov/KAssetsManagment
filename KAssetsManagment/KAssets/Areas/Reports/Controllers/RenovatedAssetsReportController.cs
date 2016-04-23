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

namespace KAssets.Areas.Reports.Controllers
{
    [Authorize]
    [HasSite]
    [RightCheck(Right = "Report for renovated assets")]
    public class RenovatedAssetsReportController : BaseController
    {
        // GET: Create report
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        //POST: Result
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Result(CreateRenovatedAssetsReportViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }

            var assets = this.requestForRenovationService.GetAll()
                .Where(x => x.IsAssetRenovated)
                .Where(x => x.IsFinished)
                .Where(x => x.AcceptancePackingSlip.DateOfGiven >= model.From
                && x.AcceptancePackingSlip.DateOfGiven < model.To);
                

            if (!this.IsMegaAdmin())
            {
                var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());           
                assets=assets.Where(x=>x.From.Site.OrganisationId==userOrg);
            }

            var convertedAssets = assets.Select(x => x.Asset).ToList();

            var viewModelAssets = convertedAssets.ToList()
                .ConvertAll(x =>
                new AssetViewModel
                {
                    AssetModel = x.Model,
                    Brand = x.Brand,
                    Currency = x.Price.Currency.Code,
                    InventoryNumber = x.InventoryNumber,
                    Price = x.Price.Value,
                    SiteName = x.Site.Name
                });

            var viewModel = new RenovatedAssetsReportResultViewModel
            {
                Assets = viewModelAssets,
                FromDate = model.From == DateTime.MinValue ? " - " : model.From.ToShortDateString(),
                ToDate = model.To == DateTime.MinValue ? " - " : model.To.ToShortDateString()
            };

            return View(viewModel);
        }
    }
}