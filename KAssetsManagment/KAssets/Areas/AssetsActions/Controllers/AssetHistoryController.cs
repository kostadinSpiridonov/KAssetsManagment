using KAssets.Filters;
using KAssets.Models;
using KAssets.Resources.Translation.AssetsTr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KAssets.Controllers;
using KAssets.Areas.AssetsActions.Models;


namespace KAssets.Areas.AssetsActions.Controllers
{
    [Authorize]
    [HasSite]
    public class AssetHistoryController : BaseController
    {
        // GET: Get aset history
        [HttpGet]
        [RightCheck(Right = "Manage assets")]
        public ActionResult ViewHistory(string id)
        {

            var history = this.assetHistoryService.GetByAssetId(id);

            //Verify if asset is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (history.Asset.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }
            var viewModel = new AssetHistoryViewModel
            {
                Id = id,
                Rows = history.Rows.ToList().ConvertAll(
                x => new HistoryRowViewModel
                {
                    Content = x.Content,
                    Date = x.Date.ToString()
                })
            };
            foreach (var item in viewModel.Rows)
            {
                switch (item.Content)
                {
                    case "The asset was acquired.":
                        {
                            item.Content = AssetTr.AssetWasAcquired;
                            break;
                        }

                    case "The asset information was updated.":
                        {
                            item.Content = AssetTr.AssetInfoWasUpdated;
                            break;
                        }

                    case "The asset was part of asset order ! The location, site or user can be changed !":
                        {
                            item.Content = AssetTr.AssetWasPartOfOrder;
                            break;
                        }

                    case "The asset was relocated.":
                        {
                            item.Content = AssetTr.AssetWasRelocated;
                            break;
                        }

                    case "The asset was renovated.":
                        {
                            item.Content = AssetTr.AssetWasRenovated;
                            break;
                        }

                    case "The asset was scrapped.":
                        {
                            item.Content = AssetTr.AssetWasScrapped;
                            break;
                        }
                }
            }
            return View(viewModel);
        }
    }
}