using KAssets.Filters;
using KAssets.Models;
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
    public class ScrappingAssetController : BaseController
    {
        // GET: Send request for scrapping
        [HttpGet]
        [RightCheck(Right = "Manage assets")]
        public ActionResult SendRequestForScrapping(string id)
        {
            var asset = this.assetService.GetById(id);

            //Verify if asset is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (asset.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            //Check status of asset
            if (asset.Status != "Active" && asset.Status != "Broken")
            {
                return Redirect("/AssetsActions/Asset/AssetIsNotActive");
            }

            var user = this.userService.GetById(this.User.Identity.GetUserId());

            var viewModel = new SendRequestForScrappingViewModel
            {
                Brand = asset.Brand,
                Producer = asset.Producer,
                InventoryNumber = asset.InventoryNumber,
                ItemModel = asset.Model,
                Guarantee = asset.Guarantee,
                Price = asset.Price.Value,
                Type = asset.Type.ToString(),
                SiteName = asset.Site.Name,
                FromId = user.Id,
                FromName = (user.FirstName != null && user.SecondName != null && user.LastName != null) ?
                        user.FirstName + " " + user.SecondName + " " + user.LastName : user.Email,
                Currency = asset.Price.Currency.Code
            };

            return View(viewModel);
        }

        // POST: Send request for scrapping
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RightCheck(Right = "Manage assets")]
        public ActionResult SendRequestForScrapping(SendRequestForScrappingViewModel model)
        {
            var asset = this.assetService.GetById(model.InventoryNumber);

            //Verify if asset is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (asset.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            //Check is there a request for this asset
            if (this.requestForScrappingService.Exist(model.InventoryNumber))
            {
                return Redirect("/AssetsActions/ScrappingAsset/ThereIsRequest");
            }

            var request = new RequestForScrapping
            {
                AssetId = model.InventoryNumber,
                IsApproved = false,
                FromId = model.FromId,
                DateOfSend = DateTime.Now
            };

            this.requestForScrappingService.Add(request);

            //Add event that there is a new request
            this.eventService.AddForUserGroup(new Event
                {
                    Content = "There is a new sccraping request for approving !",
                    Date = DateTime.Now,
                    EventRelocationUrl = "/AssetsActions/ScrappingAsset/RequestsForApproving"

                },
                "Approve request for scrapping",
                 this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()));
            return Redirect("/AssetsActions/ScrappingAsset/SuccessSend");
        }

        //GET: Get success notification page for sending
        [HttpGet]
        [RightCheck(Right = "Manage assets")]
        public ActionResult SuccessSend()
        {
            return View();
        }

        //GET: There is a request for this asset
        [HttpGet]
        [RightCheck(Right = "Manage assets")]
        public ActionResult ThereIsRequest()
        {
            return View();
        }

        //GET: Get all request for aprooving
        [HttpGet]
        [RightCheck(Right = "Approve request for scrapping")]
        public ActionResult RequestsForApproving()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());

            var requests = this.requestForScrappingService.GetAll()
                .Where(x => !x.IsApproved)
                .Where(x => !x.IsFinished);
            if (!this.IsMegaAdmin())
            {
                requests = requests.Where(x => x.From.Site.OrganisationId == userOrg);
            }

            var viewModel = requests.ToList().ConvertAll(
                x => new RequestForScrappingViewModel
                {
                    DateOfSend = x.DateOfSend.ToShortDateString(),
                    AssetInvNumber = x.Asset.InventoryNumber,
                    FromName = (x.From.FirstName != null) ?
                    x.From.FirstName + " " + x.From.SecondName + " " + x.From.LastName :
                    x.From.Email,
                    Id = x.Id
                });

            return View(viewModel);
        }

        //GET: View request for approving
        [HttpGet]
        [RightCheck(Right = "Approve request for scrapping")]
        public ActionResult ViewRequestForApproving(int id)
        {
            var request = this.requestForScrappingService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var asset = request.Asset;

            var user = request.From;

            var viewModel = new SendRequestForScrappingViewModel
            {
                Brand = asset.Brand,
                Producer = asset.Producer,
                InventoryNumber = asset.InventoryNumber,
                ItemModel = asset.Model,
                Guarantee = asset.Guarantee,
                Price = asset.Price.Value,
                Type = asset.Type.ToString(),
                SiteName = asset.Site.Name,
                FromId = user.Id,
                FromName = (user.FirstName != null && user.SecondName != null && user.LastName != null) ?
                        user.FirstName + " " + user.SecondName + " " + user.LastName : user.Email,
                Id = id,
                Currency = asset.Price.Currency.Code
            };

            return View(viewModel);
        }

        //POST:Approve scrapping request
        [HttpPost]
        [RightCheck(Right = "Approve request for scrapping")]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(int id)
        {
            var request = this.requestForScrappingService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            //Set request is approved
            this.requestForScrappingService.ApproveRequest(id);

            //Set request is finished
            this.requestForScrappingService.SetFinished(id);

            //Change status of asset
            this.assetService.ChangeStatus(request.AssetId, "Scrapped");

            //Add a new history row
            this.assetHistoryService.AddHistoryRow(new HistoryRow
            {
                Content = "The asset was scrapped.",
                Date = DateTime.Now
            }, request.Asset.InventoryNumber);

            //Add event that request is approved
            this.eventService.Add(
                new Event
            {
                UserId = request.FromId,
                Content = "Your request for scrap asset with inventory number " + request.Asset.InventoryNumber + " was approved. ",
                Date = DateTime.Now
            });

            //Add event that asset was scrapped
            this.eventService.Add(
               new Event
               {
                   UserId = request.FromId,
                   Content = "Аsset with inventory number " + request.Asset.InventoryNumber + " was scrapped. ",
                   Date = DateTime.Now,
                   EventRelocationUrl = "/AssetsActions/ScrappingAsset/ViewHistoryRequest/" + id
               });


            return Redirect("/AssetsActions/ScrappingAsset/TheAssetWasScrapped");
        }

        //GET:The asset was scrapped
        [HttpGet]
        [RightCheck(Right = "Approve request for scrapping")]
        public ActionResult TheAssetWasScrapped()
        {
            return View();
        }

        //GET: Decline a request
        [HttpGet]
        [RightCheck(Right = "Approve request for scrapping")]
        public ActionResult Decline(int id)
        {
            var request = this.requestForScrappingService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var viewModel = new DeclineRequestViewModel
            {
                RequestId = id
            };
            return View(viewModel);
        }

        //POST:Decline a request
        [HttpPost]
        [RightCheck(Right = "Approve request for scrapping")]
        [ValidateAntiForgeryToken]
        public ActionResult Decline(DeclineRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var request = this.requestForScrappingService.GetById(model.RequestId);

            var aEvent = new Event
            {
                UserId = request.FromId,
                Content = "Your request for scrapping asset with inventory number " + request.Asset.InventoryNumber + " was not approved. " + model.Message,
                Date = DateTime.Now,
                EventRelocationUrl = "/AssetsActions/ScrappingAsset/ViewHistoryRequest/" + model.RequestId
            };

            this.eventService.Add(aEvent);

            this.requestForScrappingService.SetFinished(model.RequestId);

            return Redirect("/AssetsActions/ScrappingAsset/RequestsForApproving");
        }

        //GET: History
        [HttpGet]
        [RightCheck(Right = "Manage assets")]
        public ActionResult HistoryRequests()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var requests = this.requestForScrappingService.GetAll()
                .Where(x => x.IsFinished)
                .Where(x => x.FromId == this.User.Identity.GetUserId())
                .Where(x => x.From.Site.OrganisationId == userOrg);

            var viewModel = requests.ToList().ConvertAll(
                x => new RequestForScrappingViewModel
                {
                    DateOfSend = x.DateOfSend.ToShortDateString(),
                    AssetInvNumber = x.Asset.InventoryNumber,
                    FromName = (x.From.FirstName != null) ?
                    x.From.FirstName + " " + x.From.SecondName + " " + x.From.LastName :
                    x.From.Email,
                    Id = x.Id
                });

            return View(viewModel);
        }

        //GET: View your history request
        [HttpGet]
        [RightCheck(Right = "Manage assets")]
        public ActionResult ViewHistoryRequest(int id)
        {
            var request = this.requestForScrappingService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            if (request.FromId != this.User.Identity.GetUserId())
            {
                return Redirect("/Home/NotAuthorized");
            }

            var asset = request.Asset;

            var user = request.From;

            var viewModel = new ScrappingHistoryRequest
            {
                Brand = asset.Brand,
                Producer = asset.Producer,
                InventoryNumber = asset.InventoryNumber,
                ItemModel = asset.Model,
                Guarantee = asset.Guarantee,
                Type = asset.Type.ToString(),
                Price = asset.Price.Value,
                SiteName = asset.Site.Name,
                FromId = user.Id,
                FromName = (user.FirstName != null && user.SecondName != null && user.LastName != null) ?
                        user.FirstName + " " + user.SecondName + " " + user.LastName : user.Email,
                Id = id,
                IsApproved = request.IsApproved,
                Currency = asset.Price.Currency.Code
            };

            return View(viewModel);
        }

        //GET: Your history
        [HttpGet]
        [RightCheck(Right = "Approve request for scrapping")]
        public ActionResult AllHistoryRequests()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var requests = this.requestForScrappingService.GetAll()
                .Where(x => x.IsFinished);
            if (!this.IsMegaAdmin())
            {
                requests = requests.Where(x => x.From.Site.OrganisationId == userOrg);
            }

            var viewModel = requests.ToList().ConvertAll(
                x => new RequestForScrappingViewModel
                {
                    DateOfSend = x.DateOfSend.ToShortDateString(),
                    AssetInvNumber = x.Asset.InventoryNumber,
                    FromName = (x.From.FirstName != null) ?
                    x.From.FirstName + " " + x.From.SecondName + " " + x.From.LastName :
                    x.From.Email,
                    Id = x.Id
                });

            return View(viewModel);
        }


        //GET: View history request
        [HttpGet]
        [RightCheck(Right = "Approve request for scrapping")]
        public ActionResult ViewAllHistoryRequest(int id)
        {
            var request = this.requestForScrappingService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var asset = request.Asset;

            var user = request.From;

            var viewModel = new ScrappingHistoryRequest
            {
                Brand = asset.Brand,
                Producer = asset.Producer,
                InventoryNumber = asset.InventoryNumber,
                ItemModel = asset.Model,
                Guarantee = asset.Guarantee,
                Price = asset.Price.Value,
                Type = asset.Type.ToString(),
                SiteName = asset.Site.Name,
                FromId = user.Id,
                FromName = (user.FirstName != null && user.SecondName != null && user.LastName != null) ?
                        user.FirstName + " " + user.SecondName + " " + user.LastName : user.Email,
                Id = id,
                IsApproved = request.IsApproved,
                Currency = asset.Price.Currency.Code
            };

            return View("ViewHistoryRequest", viewModel);
        }
    }
}