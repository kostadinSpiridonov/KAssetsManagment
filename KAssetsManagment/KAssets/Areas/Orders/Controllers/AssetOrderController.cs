using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KAssets.Filters;
using KAssets.Resources.Translation.OrdersTr.AssetOrderTr;
using KAssets.Areas.AssetsActions.Models;
using KAssets.Controllers;
using KAssets.Areas.Orders.Models;

namespace KAssets.Areas.Orders.Controllers
{
    [HasSite]
    [Authorize]
    public class AssetOrderController : BaseController
    {
        //GET: Send request 
        [RightCheck(Right = "Create order for asset")]
        [HttpGet]
        public ActionResult SendRequest()
        {
            return View();
        }

        //GET: Get assets for choosing
        [HttpGet]
        [RightCheck(Right = "Create order for asset")]
        public ActionResult ChooseAssets()
        {
            var items = this.assetService.GetAll().Where(x => x.Status == "Active");

            var userOrg = this.userService.GetById(this.User.Identity.GetUserId()).Site.OrganisationId;
            items = items.Where(x => x.Site.OrganisationId == userOrg);


            var viewModel = items.ToList().ConvertAll(
                x => new AssetViewModel
                {
                    AssetModel = x.Model,
                    Brand = x.Brand,
                    InventoryNumber = x.InventoryNumber,
                    SiteName = x.Site.Name,
                    Price = x.Price.Value,
                    Currency = x.Price.Currency.Code,
                    CurrencyCourse = this.exchangeService.GetRate(x.Price.CurrencyId, this.User.Identity.GetUserId())
                });

            return PartialView(viewModel);
        }

        // POST: Send a request 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RightCheck(Right = "Create order for asset")]
        public ActionResult SendRequest(SendReqeustAssetOrderViewModel model)
        {
            if (!ModelState.IsValid || model.Assets == null || model.Assets.Count() == 0)
            {
                if (model != null || model.Assets == null || model.Assets.Count() == 0)
                {
                    this.ModelState.AddModelError("", AssetOrderTr.PleaseSelectAssets);
                }
                return View(model);
            }

            //Create a request
            var req = new RequestForAsset
            {
                FromId = this.User.Identity.GetUserId(),
                DateOfSend = DateTime.Now
            };

            var requestId = this.requestForAssetService.Add(req);

            //Add assets to request
            this.requestForAssetService.AddAssetsToRequest(requestId,
                model.Assets);

            //Add a event that is sent a request
            this.eventService.AddForUserGroup(
                new Event
                {
                    Content = "There is a new asset request for approving !",
                    Date = DateTime.Now,
                    EventRelocationUrl = "/Orders/AssetOrder/GetAllRequestsForAproving"
                }, "Approve orders for asset",
                this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()));

            return Redirect("/Orders/AssetOrder/SuccessfullySend");
        }

        //GET: Successfully sent request
        [RightCheck(Right = "Create order for asset")]
        public ActionResult SuccessfullySend()
        {
            return View();
        }

        //GET: Get all requests for approving
        [HttpGet]
        [RightCheck(Right = "Approve orders for asset")]
        public ActionResult GetAllRequestsForAproving()
        {
            //Get requests for approving
            var requests = this.requestForAssetService.GetAll()
                .Where(x => !x.IsApproved)
                .Where(x => !x.AreAssetsGave)
                .Where(x => !x.Finished);

            if (!this.IsMegaAdmin())
            {
                var userOrg = this.userService.GetById(this.User.Identity.GetUserId()).Site.OrganisationId;
                requests = requests.Where(x => x.From.Site.OrganisationId == userOrg);
            }

            var viewModel = requests.ToList().ConvertAll(
                x => new ListAssetRequestViewModel
                {
                    DateOfSend = x.DateOfSend,
                    From = new ShortUserDetails
                    {
                        FullName = (x.From.FirstName + x.From.SecondName + x.From.LastName) == "" ?
                            x.From.Email : x.From.FirstName + " " + x.From.SecondName + " " + x.From.LastName,
                        Id = x.From.Id
                    },
                    Id = x.Id
                });

            return View(viewModel);
        }

        //GET: View a request for approving
        [HttpGet]
        [RightCheck(Right = "Approve orders for asset")]
        public ActionResult RequestForApproving(int id)
        {
            var request = this.requestForAssetService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }


            var viewModel = new BaseRequestForAssetViewModel();

            //Add request data to viewmodel
            viewModel.Request = new RequestForAssetViewModel
            {
                DateOfSend = request.DateOfSend,
                From = new ShortUserDetails
                {
                    FullName = (request.From.FirstName + request.From.SecondName + request.From.LastName) == "" ?
                        request.From.Email : request.From.FirstName + " " + request.From.SecondName + " " + request.From.LastName,
                    Id = request.From.Id
                },
                Id = request.Id
            };

            //Add want asset to viewmodel
            viewModel.WantAssets = request.Assets.ToList().ConvertAll(
                 x => new RequestAssetViewModel
                 {
                     Asset = new AssetViewModel
                     {
                         AssetModel = x.Model,
                         Brand = x.Brand,
                         InventoryNumber = x.InventoryNumber,
                         SiteName = x.Site.Name,
                         Price = x.Price.Value,
                         Currency = x.Price.Currency.Code,
                         CurrencyCourse = this.exchangeService.GetRate(x.Price.CurrencyId, this.User.Identity.GetUserId())
                     },
                     Selected = true

                 });

            //Add approved asset to viewmodel
            viewModel.ApprovedAssets = request.Assets.ToList().ConvertAll(
                x => new RequestAssetViewModel
                {
                    Asset = new AssetViewModel
                    {
                        AssetModel = x.Model,
                        Brand = x.Brand,
                        InventoryNumber = x.InventoryNumber,
                        SiteName = x.Site.Name,
                        Price = x.Price.Value
                    },
                    Selected = true
                });

            return View(viewModel);
        }

        //GET: Decline a request
        [HttpGet]
        [RightCheck(Right = "Approve orders for asset")]
        public ActionResult Decline(int id)
        {
            var request = this.requestForAssetService.GetById(id);

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
        [ValidateAntiForgeryToken]
        [RightCheck(Right = "Approve orders for asset")]
        public ActionResult Decline(DeclineRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var request = this.requestForAssetService.GetById(model.RequestId);
            var itemsMessage = "";

            foreach (var item in request.Assets)
            {
                itemsMessage += item.InventoryNumber + ", ";
            }

            //Add event tha request was not approved
            var aEvent = new Event
            {
                UserId = request.FromId,
                Content = "Your request for assets with inventory numbers " + itemsMessage + " was not approved. " + model.Message,
                Date = DateTime.Now,
                EventRelocationUrl = "/Orders/AssetOrder/GetRequestForFinishingOrFinished/" + model.RequestId
            };

            this.eventService.Add(aEvent);

            //Set request is finished and not approved
            this.requestForAssetService.SetFinished(model.RequestId);
            this.requestForAssetService.SetNotApproved(model.RequestId);

            return Redirect("/Orders/AssetOrder/GetAllRequestsForAproving");
        }

        //POST:Approve a request
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RightCheck(Right = "Approve orders for asset")]
        public ActionResult Approve(BaseRequestForAssetViewModel model)
        {
            //Add approved assets to request
            var elements = model.ApprovedAssets.Where(x => x.Selected).Select(x => x.Asset.InventoryNumber).ToList();

            this.requestForAssetService.AddApprovedAssets(model.Request.Id, elements);

            //Set request is approved
            this.requestForAssetService.SetApproved(model.Request.Id);

            //Add a event that request is approved
            this.eventService.AddForUserGroup(
              new Event
              {
                  Content = "There is a new approved request for asset !",
                  Date = DateTime.Now,
                  EventRelocationUrl = "/Orders/AssetOrder/ViewApprovedRequests"
              }, "Give assets for asset orders",
                this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()));

            //Create request' packing slip
            var id = this.packingSlipService.Add(new PackingSlip
            {
                ToUserId = model.Request.From.Id
            });

            this.requestForAssetService.AddPackingSlip(model.Request.Id, id);

            return Redirect("/Orders/AssetOrder/GetAllRequestsForAproving");
        }

        //GET: View aproved requests
        [HttpGet]
        [RightCheck(Right = "Give assets for asset orders")]
        public ActionResult ViewApprovedRequests()
        {
            var requests = this.requestForAssetService.GetAll()
                .Where(x => x.IsApproved)
                .Where(x => !x.Finished)
                .Where(x => !x.AreAssetsGave);

            if (!this.IsMegaAdmin())
            {
                var userOrg = this.userService.GetById(this.User.Identity.GetUserId()).Site.OrganisationId;
                requests = requests.Where(x => x.From.Site.OrganisationId == userOrg);
            }

            var viewModel = requests.ToList().ConvertAll(
                  x => new RequestForAssetViewModel
                  {
                      DateOfSend = x.DateOfSend,
                      From = new ShortUserDetails
                      {
                          FullName = (x.From.FirstName + x.From.SecondName + x.From.LastName) == "" ?
                              x.From.Email : x.From.FirstName + " " + x.From.SecondName + " " + x.From.LastName,
                          Id = x.From.Id
                      },
                      Id = x.Id
                  });

            return View(viewModel);
        }

        //GET: View a request for approving
        [HttpGet]
        [RightCheck(Right = "Give assets for asset orders")]
        public ActionResult ViewApprovedRequest(int id)
        {
            var request = this.requestForAssetService.GetById(id);
            if (!this.IsMegaAdmin())
            {
                //Verify if request is from user organisation
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var viewModel = new BaseRequestForAssetViewModel();

            //Set request data to viewmodel
            viewModel.Request = new RequestForAssetViewModel
            {
                DateOfSend = request.DateOfSend,
                From = new ShortUserDetails
                {
                    FullName = (request.From.FirstName + request.From.SecondName + request.From.LastName) == "" ?
                        request.From.Email : request.From.FirstName + " " + request.From.SecondName + " " + request.From.LastName,
                    Id = request.From.Id
                },
                Id = request.Id
            };

            //Set want assets to viewmodel
            viewModel.WantAssets = request.Assets.ToList().ConvertAll(
                 x => new RequestAssetViewModel
                 {
                     Asset = new AssetViewModel
                     {
                         AssetModel = x.Model,
                         Brand = x.Brand,
                         InventoryNumber = x.InventoryNumber,
                         SiteName = x.Site.Name,
                         Price = x.Price.Value,
                         Currency = x.Price.Currency.Code
                     },
                     Selected = true

                 });

            //Set approved asset to viewmodel
            viewModel.ApprovedAssets = request.ApprovedAssets.ToList().ConvertAll(
              x => new RequestAssetViewModel
              {
                  Asset = new AssetViewModel
                  {
                      AssetModel = x.Model,
                      Brand = x.Brand,
                      InventoryNumber = x.InventoryNumber,
                      SiteName = x.Site.Name,
                      Price = x.Price.Value,
                      Currency = x.Price.Currency.Code,
                      CurrencyCourse = this.exchangeService.GetRate(x.Price.CurrencyId, this.User.Identity.GetUserId())
                  },
                  Selected = true

              });

            //Set given asset to viewmodel
            viewModel.GivenAssets = request.ApprovedAssets.ToList().ConvertAll(
             x => new RequestAssetViewModel
             {
                 Asset = new AssetViewModel
                 {
                     AssetModel = x.Model,
                     Brand = x.Brand,
                     InventoryNumber = x.InventoryNumber,
                     SiteName = x.Site.Name,
                     Price = x.Price.Value
                 },
                 Selected = true

             });

            return View(viewModel);
        }

        //GET: Assets are not given
        [HttpGet]
        [RightCheck(Right = "Give assets for asset orders")]
        public ActionResult NotGave(int id)
        {
            //Verify if request is from user organisation
            var request = this.requestForAssetService.GetById(id);
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
            return View("Decline", viewModel);
        }

        //POST:Assets are not given
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RightCheck(Right = "Give assets for asset orders")]
        public ActionResult NotGave(DeclineRequestViewModel model)
        {
            var request = this.requestForAssetService.GetById(model.RequestId);
            var itemsMessage = "";

            foreach (var item in request.Assets)
            {
                itemsMessage += item.InventoryNumber + ", ";
            }

            //Add a new event that asset were not givent
            var aEvent = new Event
            {
                UserId = request.FromId,
                Content = "Your request for assets with inventory numbers " + itemsMessage + " was approved. But the asset cannot be gave ! " + model.Message,
                Date = DateTime.Now,
                EventRelocationUrl = "/Orders/AssetOrder/GetRequestForFinishingOrFinished/" + model.RequestId
            };

            this.eventService.Add(aEvent);

            //Add from user to request' packing slip
            this.packingSlipService.SetFromUser(
                  request.PackingSlipId.Value,
                  this.User.Identity.GetUserId());


            //Set request is finished
            this.requestForAssetService.SetFinished(model.RequestId);

            return Redirect("/Orders/AssetOrder/ViewApprovedRequests");
        }

        //GET: Give items 
        [HttpPost]
        [RightCheck(Right = "Give assets for asset orders")]
        public ActionResult GiveItems(BaseRequestForAssetViewModel model)
        {
            //Set assets are given and add them to request
            this.requestForAssetService.SetAssetsGiven(model.Request.Id);

            var elements = model.GivenAssets.Where(x => x.Selected).Select(x => x.Asset.InventoryNumber).ToList();

            this.requestForAssetService.AddGivenAssets(model.Request.Id, elements);

            //Add a new event that assest are given
            var aEvent = new Event
            {
                UserId = model.Request.From.Id,
                Content = "You have a request for finishing !",
                Date = DateTime.Now,
                EventRelocationUrl = "/Orders/AssetOrder/RequestForFinishing"
            };

            this.eventService.Add(aEvent);

            //Set assets are given in packing slip
            this.packingSlipService.SetGiven(
                this.requestForAssetService.GetById(model.Request.Id).PackingSlipId.Value, DateTime.Now);

            //Set from user in packing slip
            this.packingSlipService.SetFromUser(
                  this.requestForAssetService.GetById(model.Request.Id).PackingSlipId.Value,
                  this.User.Identity.GetUserId());

            return Redirect("/Orders/AssetOrder/ViewApprovedRequests");
        }

        //GET: Get requests for finishing
        [HttpGet]
        [RightCheck(Right = "Create order for asset")]
        public ActionResult RequestForFinishing()
        {
            var requests = this.requestForAssetService.GetAll()
                .Where(x => x.IsApproved)
                .Where(x => !x.Finished)
                .Where(x => x.AreAssetsGave);

            if (!this.IsMegaAdmin())
            {
                requests = requests.Where(x => x.FromId == this.User.Identity.GetUserId());
            }

            var viewModel = requests.ToList().ConvertAll(
                x => new RequestForAssetViewModel
                {
                    DateOfSend = x.DateOfSend,
                    From = new ShortUserDetails
                    {
                        FullName = (x.From.FirstName + x.From.SecondName + x.From.LastName) == "" ?
                            x.From.Email : x.From.FirstName + " " + x.From.SecondName + " " + x.From.LastName,
                        Id = x.From.Id
                    },
                    Id = x.Id
                });

            return View(viewModel);
        }

        //GET: Get request for finishing or is finished
        [HttpGet]
        [RightCheck(Right = "Create order for asset")]
        public ActionResult GetRequestForFinishingOrFinished(int id)
        {
            var request = this.requestForAssetService.GetById(id);

            //Verify if requiest is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }


                if (request.Finished)
                {
                    if (request.FromId != this.User.Identity.GetUserId())
                    {
                        return Redirect("/Home/NotAuthorized");
                    }
                }
            }

            var viewModel = new BaseRequestForAssetViewModel();

            //Set request data to viewmodel
            viewModel.Request = new RequestForAssetViewModel
            {
                DateOfSend = request.DateOfSend,
                From = new ShortUserDetails
                {
                    FullName = (request.From.FirstName + request.From.SecondName + request.From.LastName) == "" ?
                        request.From.Email : request.From.FirstName + " " + request.From.SecondName + " " + request.From.LastName,
                    Id = request.From.Id
                },
                Id = request.Id,
                PackingSlipId = request.PackingSlip != null ? request.PackingSlipId.Value : 0,
                IsFinished = request.Finished
            };

            //Set want assets to viewmodel
            viewModel.WantAssets = request.Assets.ToList().ConvertAll(
                 x => new RequestAssetViewModel
                 {
                     Asset = new AssetViewModel
                     {
                         AssetModel = x.Model,
                         Brand = x.Brand,
                         InventoryNumber = x.InventoryNumber,
                         SiteName = x.Site.Name,
                         Price = x.Price.Value,
                         Currency = x.Price.Currency.Code
                     },
                     Selected = true

                 });

            //Set approved assets to viewmodel
            viewModel.ApprovedAssets = request.ApprovedAssets.ToList().ConvertAll(
              x => new RequestAssetViewModel
              {
                  Asset = new AssetViewModel
                  {
                      AssetModel = x.Model,
                      Brand = x.Brand,
                      InventoryNumber = x.InventoryNumber,
                      SiteName = x.Site.Name,
                      Price = x.Price.Value,
                      Currency = x.Price.Currency.Code
                  },
                  Selected = true

              });

            // Set giveen asset to viewmodel
            viewModel.GivenAssets = request.GivenAssets.ToList().ConvertAll(
             x => new RequestAssetViewModel
             {
                 Asset = new AssetViewModel
                 {
                     AssetModel = x.Model,
                     Brand = x.Brand,
                     InventoryNumber = x.InventoryNumber,
                     SiteName = x.Site.Name,
                     Price = x.Price.Value,
                     Currency = x.Price.Currency.Code,
                     CurrencyCourse = this.exchangeService.GetRate(x.Price.CurrencyId, this.User.Identity.GetUserId())
                 },
                 Selected = true

             });

            viewModel.IsApproved = request.IsApproved;
            viewModel.AreItemGave = request.AreAssetsGave;

            return View(viewModel);
        }

        //POST: Receive assets
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RightCheck(Right = "Create order for asset")]
        public ActionResult Receive(int id)
        {
            var request = this.requestForAssetService.GetById(id);

            //Verify if requiest is from user organisation

            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }


            //Set request is finished and asset are received in packing slip
            this.packingSlipService.SetReceived(request.PackingSlipId.Value, DateTime.Now);
            this.requestForAssetService.SetFinished(id);

            //Change site, location and user of assets and add a new history row
            foreach (var item in request.GivenAssets)
            {
                this.assetService.Relocate(item.InventoryNumber, request.From.SiteId.Value);
                this.assetService.ChangeLocation(item.InventoryNumber, request.From.LocationId);
                this.assetService.ChangeUser(item.InventoryNumber, request.FromId);

                this.assetHistoryService.AddHistoryRow(new HistoryRow
                    {
                        Content = "The asset was part of asset order ! The location, site or user can be changed !",
                        Date = DateTime.Now
                    }, item.InventoryNumber);
            }
            return Redirect("/Orders/AssetOrder/RequestForFinishing");
        }

        //GET: Get history requests
        [HttpGet]
        [RightCheck(Right = "Create order for asset")]
        public ActionResult HistoryRequests()
        {
            var requests = this.requestForAssetService.GetAll()
                .Where(x => x.FromId == this.User.Identity.GetUserId())
                .Where(x => x.Finished);

            var viewModel = requests.ToList().ConvertAll(
                x => new RequestForAssetViewModel
                {
                    DateOfSend = x.DateOfSend,
                    From = new ShortUserDetails
                    {
                        FullName = (x.From.FirstName + x.From.SecondName + x.From.LastName) == "" ?
                            x.From.Email : x.From.FirstName + " " + x.From.SecondName + " " + x.From.LastName,
                        Id = x.From.Id
                    },
                    Id = x.Id
                });

            return View(viewModel);
        }

        //GET: Get all history request
        [HttpGet]
        [RightCheck(Right = "Approve orders for asset")]
        public ActionResult HistoryRequestAll(int id)
        {
            var request = this.requestForAssetService.GetById(id);

            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }
            var viewModel = new BaseRequestForAssetViewModel();

            //Set request data to viewmodel
            viewModel.Request = new RequestForAssetViewModel
            {
                DateOfSend = request.DateOfSend,
                From = new ShortUserDetails
                {
                    FullName = (request.From.FirstName + request.From.SecondName + request.From.LastName) == "" ?
                        request.From.Email : request.From.FirstName + " " + request.From.SecondName + " " + request.From.LastName,
                    Id = request.From.Id
                },
                Id = request.Id,
                PackingSlipId = request.PackingSlip != null ? request.PackingSlipId.Value : 0,
                IsFinished = request.Finished
            };

            //Set want assets to viewmodel
            viewModel.WantAssets = request.Assets.ToList().ConvertAll(
                 x => new RequestAssetViewModel
                 {
                     Asset = new AssetViewModel
                     {
                         AssetModel = x.Model,
                         Brand = x.Brand,
                         InventoryNumber = x.InventoryNumber,
                         SiteName = x.Site.Name,
                         Price = x.Price.Value,
                         Currency = x.Price.Currency.Code
                     },
                     Selected = true

                 });

            //Set aproved asset to viewmodel
            viewModel.ApprovedAssets = request.ApprovedAssets.ToList().ConvertAll(
              x => new RequestAssetViewModel
              {
                  Asset = new AssetViewModel
                  {
                      AssetModel = x.Model,
                      Brand = x.Brand,
                      InventoryNumber = x.InventoryNumber,
                      SiteName = x.Site.Name,
                      Price = x.Price.Value,
                      Currency = x.Price.Currency.Code
                  },
                  Selected = true

              });

            //Set givven aset to viewmodel
            viewModel.GivenAssets = request.GivenAssets.ToList().ConvertAll(
             x => new RequestAssetViewModel
             {
                 Asset = new AssetViewModel
                 {
                     AssetModel = x.Model,
                     Brand = x.Brand,
                     InventoryNumber = x.InventoryNumber,
                     SiteName = x.Site.Name,
                     Price = x.Price.Value,
                     Currency = x.Price.Currency.Code,
                     CurrencyCourse = this.exchangeService.GetRate(x.Price.CurrencyId, this.User.Identity.GetUserId())
                 },
                 Selected = true

             });

            viewModel.IsApproved = request.IsApproved;
            viewModel.AreItemGave = request.AreAssetsGave;

            return View("GetRequestForFinishingOrFinished", viewModel);
        }

        //GET: Get all history requests
        [HttpGet]
        [RightCheck(Right = "Approve orders for asset")]
        public ActionResult HistoryRequestsAll()
        {

            var requests = this.requestForAssetService.GetAll()
                .Where(x => x.Finished);

            if (!this.IsMegaAdmin())
            {
                var userOrg = this.userService.GetById(this.User.Identity.GetUserId()).Site.OrganisationId;
                requests = requests.Where(x => x.From.Site.OrganisationId == userOrg);
            }

            var viewModel = requests.ToList().ConvertAll(
                x => new RequestForAssetViewModel
                {
                    DateOfSend = x.DateOfSend,
                    From = new ShortUserDetails
                    {
                        FullName = (x.From.FirstName + x.From.SecondName + x.From.LastName) == "" ?
                            x.From.Email : x.From.FirstName + " " + x.From.SecondName + " " + x.From.LastName,
                        Id = x.From.Id
                    },
                    Id = x.Id
                });

            return View(viewModel);
        }
    }
}