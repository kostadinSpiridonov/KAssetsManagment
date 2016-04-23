using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KAssets.Filters;
using KAssets.Controllers;
using KAssets.Areas.AssetsActions.Models;
using KAssets.Areas.HelpModule.Models;


namespace KAssets.Areas.AssetsActions.Controllers
{
    [Authorize]
    [HasSite]
    public class RenovationAssetController : BaseController
    {
        //GET: Send request for renovation
        [RightCheck(Right = "Send request for renovation")]
        public ActionResult CreateRequestForRenovation()
        {
            return View();
        }

        //GET: Get assets
        [HttpGet]
        [RightCheck(Right = "Send request for renovation")]
        public ActionResult ChooseAsset()
        {
            var userSite = this.userService.GetById(this.User.Identity.GetUserId()).Site;
            var assets = this.assetService.GetAll().Where(x => x.Status == "Active");

            if (!this.IsMegaAdmin())
            {
                assets = assets.Where(x => x.Site.OrganisationId == userSite.OrganisationId);
            }

            var viewModel = assets.ToList().ConvertAll(
                x => new AssetViewModel
                {
                    AssetModel = x.Model,
                    Brand = x.Brand,
                    InventoryNumber = x.InventoryNumber
                });

            return PartialView(viewModel);
        }

        //POST: Send request for renovation
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RightCheck(Right = "Send request for renovation")]
        public ActionResult CreateRequestForRenovation(CreateRenovationRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Check is there a request for this asset
            if (this.requestForRenovationService.Exist(model.SelectedAsset))
            {
                return Redirect("/AssetsActions/RenovationAsset/ThereIsRequest");
            }

            //Add a request
            this.requestForRenovationService.Add(new RequestForRenovation
            {
                AssetId = model.SelectedAsset,
                DateOfSend = DateTime.Now,
                FromId = this.User.Identity.GetUserId(),
                ProblemMessage = model.ProblemMesssage
            });


            //Add event that there is a new request
            var aEvent = new Event
            {
                Content = "There are new request for renovation ! ",
                Date = DateTime.Now,
                EventRelocationUrl = "/AssetsActions/RenovationAsset/GetRequestsForApproving"
            };

            this.eventService.AddForUserGroup(aEvent, "Approve request for renovation",
                 this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()));

            return Redirect("/AssetsActions/RenovationAsset/SuccessSend");
        }

        //GET: Get success notification page for sending
        [HttpGet]
        [RightCheck(Right = "Send request for renovation")]
        public ActionResult SuccessSend()
        {
            return View();
        }

        //GET: There is a request for this asset
        [HttpGet]
        [RightCheck(Right = "Send request for renovation")]
        public ActionResult ThereIsRequest()
        {
            return View();
        }

        //GET: Get requests for approving
        [HttpGet]
        [RightCheck(Right = "Approve request for renovation")]
        public ActionResult GetRequestsForApproving()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var requests = this.requestForRenovationService.GetAll()
                .Where(x => !x.IsApproved)
                .Where(x => !x.IsAssetGave)
                .Where(x => !x.IsAssetRenovated)
                .Where(x => !x.IsFinished)
                .Where(x => !x.IsSeenFromApprover);

            if (!this.IsMegaAdmin())
            {
                requests = requests.Where(x => x.From.Site.OrganisationId == userOrg);
            }

            var viewModel = requests.ToList().ConvertAll(
                x => new RequestForRenovationViewModel
                {
                    InventoryNumber = x.Asset.InventoryNumber,
                    Id = x.Id,
                    FromName = (x.From.FirstName != null) ?
                   x.From.FirstName + " " + x.From.SecondName + " " + x.From.LastName :
                   x.From.Email,
                    DateOfSend = x.DateOfSend.ToString(),
                });
            return View(viewModel);

        }

        //GET: View request for approving
        [HttpGet]
        [RightCheck(Right = "Approve request for renovation")]
        public ActionResult ViewRequestForApproving(int id)
        {
            var request = this.requestForRenovationService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var viewModel = new RequestForRenovationDetails
            {
                DateOfSend = request.DateOfSend.ToString(),
                FromName = (request.From.FirstName != null) ?
                    request.From.FirstName + " " + request.From.SecondName + " " + request.From.LastName :
                    request.From.Email,
                ProblemMessage = request.ProblemMessage,
                Id = request.Id
            };

            viewModel.Asset = new AssetDetailsViewModel
            {
                Brand = request.Asset.Brand,
                Guarantee = request.Asset.Guarantee,
                InventoryNumber = request.Asset.InventoryNumber,
                ItemModel = request.Asset.Model,
                Price = request.Asset.Price.Value,
                Producer = request.Asset.Producer,
                Type = request.Asset.Type,
                Currency = request.Asset.Price.Currency.Code
            };

            if (request.Asset.Location != null)
            {
                viewModel.Asset.Location = new LocationViewModel
                {
                    Country = request.Asset.Location.Country != null ? request.Asset.Location.Country : "",
                    Latitude = request.Asset.Location.Latitude != null ? request.Asset.Location.Latitude : "",
                    Longitude = request.Asset.Location.Longitude != null ? request.Asset.Location.Longitude : "",
                    Street = request.Asset.Location.Street != null ? request.Asset.Location.Street : "",
                    StreetNumber = request.Asset.Location.StreetNumber != null ? request.Asset.Location.StreetNumber.Value : 0,
                    Town = request.Asset.Location.Town != null ? request.Asset.Location.Town : "",

                };
            }
            viewModel.Asset.SiteName = request.Asset.Site.Name;

            return View(viewModel);
        }

        //GET: Decline a request
        [HttpGet]
        [RightCheck(Right = "Approve request for renovation")]
        public ActionResult Decline(int id)
        {
            var request = this.requestForRenovationService.GetById(id);
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
        [RightCheck(Right = "Approve request for renovation")]
        public ActionResult Decline(DeclineRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var request = this.requestForRenovationService.GetById(model.RequestId);

            //Add event that request is not approved
            var aEvent = new Event
            {
                UserId = request.FromId,
                Content = "Your request for renovation the asset with inventory number " + request.Asset.InventoryNumber + " was not approved. " + model.Message,
                Date = DateTime.Now,
                EventRelocationUrl = "/AssetsActions/RenovationAsset/ViewHistoryRequest/" + model.RequestId
            };

            this.eventService.Add(aEvent);

            //Set request is finished and it is seen by approver
            this.requestForRenovationService.SetFinished(model.RequestId);
            this.requestForRenovationService.SetIsSeenFromApprover(model.RequestId);

            return Redirect("/AssetsActions/RenovationAsset/GetRequestsForApproving");
        }

        //POST: Approve a request for renovation
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RightCheck(Right = "Approve request for renovation")]
        public ActionResult ApproveRequest(int id)
        {
            var request = this.requestForRenovationService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            //Set request is approved and it is seen by approver
            this.requestForRenovationService.SetApproved(id);
            this.requestForRenovationService.SetIsSeenFromApprover(id);

            //Add event that request is approved
            this.eventService.Add(
                new Event
                {
                    UserId = request.FromId,
                    Content = "Your request for renovation the asset with inventory number " + request.Asset.InventoryNumber + " was approved. ",
                    Date = DateTime.Now,
                    EventRelocationUrl = "/AssetsActions/RenovationAsset/GetApprovedRequestsForUser"
                });

            //Create packing slip for issue
            var issuePackingSlipId = this.packingSlipService.Add(new PackingSlip
            {
                FromUserId = request.FromId
            });

            this.requestForRenovationService.AddIssuePackingSlip(request.Id, issuePackingSlipId);

            //Add packign slip for acceptance
            var acceptancePackingSlipId = this.packingSlipService.Add(new PackingSlip
            {
                ToUserId = request.FromId
            });

            this.requestForRenovationService.AddAcceptancePackingSlip(request.Id, acceptancePackingSlipId);

            return Redirect("/AssetsActions/RenovationAsset/GetRequestsForApproving");
        }

        //GET: Get approved requests
        [HttpGet]
        [RightCheck(Right = "Send request for renovation")]
        public ActionResult GetApprovedRequestsForUser()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var requests = this.requestForRenovationService.GetAll()
                .Where(x => !x.IsFinished)
                .Where(x => x.IsSeenFromApprover)
                .Where(x => x.IsApproved)
                .Where(x => !x.IsAssetGave)
                .Where(x => !x.IsAssetRenovated);

            if (!this.IsMegaAdmin())
            {
                requests = requests.Where(x => x.From.Site.OrganisationId == userOrg)
                .Where(x => x.FromId == this.User.Identity.GetUserId());
            }

            var viewModel = requests.ToList().ConvertAll(
                x => new RequestForRenovationViewModel
                {
                    InventoryNumber = x.Asset.InventoryNumber,
                    Id = x.Id,
                    FromName = (x.From.FirstName != null) ?
                   x.From.FirstName + " " + x.From.SecondName + " " + x.From.LastName :
                   x.From.Email,
                    DateOfSend = x.DateOfSend.ToString()
                });
            return View(viewModel);

        }

        //GET: View Approved request
        [HttpGet]
        [RightCheck(Right = "Send request for renovation")]
        public ActionResult ViewApprovedRequest(int id)
        {
            var request = this.requestForRenovationService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var viewModel = new RequestForRenovationDetails
            {
                DateOfSend = request.DateOfSend.ToString(),
                FromName = (request.From.FirstName != null) ?
                    request.From.FirstName + " " + request.From.SecondName + " " + request.From.LastName :
                    request.From.Email,
                ProblemMessage = request.ProblemMessage,
                Id = request.Id
            };

            viewModel.Asset = new AssetDetailsViewModel
            {
                Brand = request.Asset.Brand,
                Guarantee = request.Asset.Guarantee,
                InventoryNumber = request.Asset.InventoryNumber,
                ItemModel = request.Asset.Model,
                Price = request.Asset.Price.Value,
                Producer = request.Asset.Producer,
                Type = request.Asset.Type,
                Currency = request.Asset.Price.Currency.Code
            };

            if (request.Asset.Location != null)
            {
                viewModel.Asset.Location = new LocationViewModel
                {
                    Country = request.Asset.Location.Country != null ? request.Asset.Location.Country : "",
                    Latitude = request.Asset.Location.Latitude != null ? request.Asset.Location.Latitude : "",
                    Longitude = request.Asset.Location.Longitude != null ? request.Asset.Location.Longitude : "",
                    Street = request.Asset.Location.Street != null ? request.Asset.Location.Street : "",
                    StreetNumber = request.Asset.Location.StreetNumber != null ? request.Asset.Location.StreetNumber.Value : 0,
                    Town = request.Asset.Location.Town != null ? request.Asset.Location.Town : "",

                };
            }
            viewModel.Asset.SiteName = request.Asset.Site.Name;

            return View(viewModel);
        }

        //POST: Decline a request
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RightCheck(Right = "Send request for renovation")]
        public ActionResult RemoveRequest(int id)
        {
            var request = this.requestForRenovationService.GetById(id);
            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            //Set request is finished
            this.requestForRenovationService.SetFinished(id);

            //Add event that request is removed
            this.eventService.AddForUserGroup(new Event
                {
                    Content = "The request for renovation was removed by user !",
                    Date = DateTime.Now,
                    EventRelocationUrl = "/AssetsActions/RenovationAsset/ViewHistoryRequest/" + id
                }, "Approve request for renovation",
                 this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()));
            return Redirect("/AssetsActions/RenovationAsset/GetApprovedRequestsForUser");
        }

        //POST:Give asset for renovating
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RightCheck(Right = "Send request for renovation")]
        public ActionResult GiveAsset(int id)
        {
            //Set asset is given 
            var request = this.requestForRenovationService.GetById(id);
            this.requestForRenovationService.SetIsAssetGave(id);

            //Add event that asset is given
            var aEvent = new Event
            {
                Content = "There are new asset for renovatings ! ",
                Date = DateTime.Now,
                EventRelocationUrl = "/AssetsActions/RenovationAsset/GetAssetsForRenovating"
            };

            this.eventService.AddForUserGroup(aEvent, "Renovate assets",
                 this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()));

            //Change status of asset
            this.assetService.ChangeStatus(request.AssetId, "Renovating");

            //Set asset is given in packing slip
            this.packingSlipService.SetGiven(request.IssuePackingSlipId.Value, DateTime.Now);

            return Redirect("/AssetsActions/RenovationAsset/GetApprovedRequestsForUser");
        }

        //GET: Get assets which are renovating
        [HttpGet]
        [RightCheck(Right = "Renovate assets")]
        public ActionResult GetAssetsForRenovating()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var requests = this.requestForRenovationService.GetAll()
                .Where(x => !x.IsFinished)
                .Where(x => x.IsSeenFromApprover)
                .Where(x => x.IsApproved)
                .Where(x => x.IsAssetGave)
                .Where(x => !x.IsAssetRenovated)
                .Where(x => !x.AssetIsReturned);

            if (!this.IsMegaAdmin())
            {
                requests = requests.Where(x => x.From.Site.OrganisationId == userOrg);
            }

            var viewModel = requests.ToList().ConvertAll(
                x => new RequestForRenovationViewModel
                {
                    InventoryNumber = x.Asset.InventoryNumber,
                    Id = x.Id,
                    FromName = (x.From.FirstName != null) ?
                   x.From.FirstName + " " + x.From.SecondName + " " + x.From.LastName :
                   x.From.Email,
                    DateOfSend = x.DateOfSend.ToString()
                });
            return View(viewModel);

        }

        //GET: Get asset which are renovating
        [HttpGet]
        [RightCheck(Right = "Renovate assets")]
        public ActionResult ViewAssetForRenovating(int id)
        {
            var request = this.requestForRenovationService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var viewModel = new RequestForRenovationDetails
            {
                DateOfSend = request.DateOfSend.ToString(),
                FromName = (request.From.FirstName != null) ?
                    request.From.FirstName + " " + request.From.SecondName + " " + request.From.LastName :
                    request.From.Email,
                ProblemMessage = request.ProblemMessage,
                Id = request.Id,
                IsRecieve = request.IssuePackingSlip.Received
            };

            viewModel.Asset = new AssetDetailsViewModel
            {
                Brand = request.Asset.Brand,
                Guarantee = request.Asset.Guarantee,
                InventoryNumber = request.Asset.InventoryNumber,
                ItemModel = request.Asset.Model,
                Price = request.Asset.Price.Value,
                Producer = request.Asset.Producer,
                Type = request.Asset.Type,
                Currency = request.Asset.Price.Currency.Code
            };

            if (request.Asset.Location != null)
            {
                viewModel.Asset.Location = new LocationViewModel
                {
                    Country = request.Asset.Location.Country != null ? request.Asset.Location.Country : "",
                    Latitude = request.Asset.Location.Latitude != null ? request.Asset.Location.Latitude : "",
                    Longitude = request.Asset.Location.Longitude != null ? request.Asset.Location.Longitude : "",
                    Street = request.Asset.Location.Street != null ? request.Asset.Location.Street : "",
                    StreetNumber = request.Asset.Location.StreetNumber != null ? request.Asset.Location.StreetNumber.Value : 0,
                    Town = request.Asset.Location.Town != null ? request.Asset.Location.Town : "",

                };
            }
            viewModel.Asset.SiteName = request.Asset.Site.Name;

            return View(viewModel);
        }

        //POST: Set the asset is renovated
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RightCheck(Right = "Renovate assets")]
        public ActionResult Renovate(int id)
        {
            var request = this.requestForRenovationService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            //Set asset is renovated and change status
            this.requestForRenovationService.SetIsRenovated(id);
            this.assetService.ChangeStatus(request.AssetId, "Active");

            //Add event that asset i renovated
            var aEvent = new Event
            {
                UserId = request.FromId,
                Content = "Your asset with inventory number " + request.Asset.InventoryNumber + " was renovated and now it is active !",
                Date = DateTime.Now,
                EventRelocationUrl = "/AssetsActions/RenovationAsset/ReturnedAssets"
            };

            // Add a new history row that asset was renovated
            this.assetHistoryService.AddHistoryRow(new HistoryRow
            {
                Content = "The asset was renovated.",
                Date = DateTime.Now
            }, request.Asset.InventoryNumber);

            this.eventService.Add(aEvent);

            //Set asset is returned, set asset is given and from in packing slip
            this.requestForRenovationService.SetReturned(id);
            this.packingSlipService.SetGiven(request.AcceptancePackingSlipId.Value, DateTime.Now);
            this.packingSlipService.SetFromUser(request.AcceptancePackingSlipId.Value, this.User.Identity.GetUserId());

            return Redirect("/AssetsActions/RenovationAsset/GetAssetsForRenovating");
        }

        //POST: Set asset is receive issue
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RightCheck(Right = "Renovate assets")]
        public ActionResult ReceiveIssue(int id)
        {
            var request = this.requestForRenovationService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            //Set asset is receive and from
            this.packingSlipService.SetReceived(request.IssuePackingSlipId.Value, DateTime.Now);
            this.packingSlipService.SetToUser(request.IssuePackingSlipId.Value, this.User.Identity.GetUserId());

            return Redirect("/AssetsActions/RenovationAsset/ViewAssetForRenovating/" + id);
        }

        //GET: Set the asset is not renovated
        [HttpGet]
        [RightCheck(Right = "Renovate assets")]
        public ActionResult AssetIsNotRenovate(int id)
        {
            var request = this.requestForRenovationService.GetById(id);
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

        //POST: Set the asset is not renovated
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RightCheck(Right = "Renovate assets")]
        public ActionResult AssetIsNotRenovate(DeclineRequestViewModel model)
        {
            var request = this.requestForRenovationService.GetById(model.RequestId);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            //Add event that asset is not renovated
            var aEvent = new Event
            {
                UserId = request.FromId,
                Content = "Asset with inventory number " + request.Asset.InventoryNumber + " was not renovated and now it is active. " + model.Message,
                Date = DateTime.Now,
                EventRelocationUrl = "/AssetsActions/RenovationAsset/ReturnedAssets"
            };

            //Set asset is given and from in packing slip
            this.packingSlipService.SetGiven(request.AcceptancePackingSlipId.Value, DateTime.Now);
            this.packingSlipService.SetFromUser(request.AcceptancePackingSlipId.Value, this.User.Identity.GetUserId());

            //Set asset is returned and change status
            this.requestForRenovationService.SetReturned(model.RequestId);
            this.assetService.ChangeStatus(request.AssetId, "Broken");
            this.eventService.Add(aEvent);



            return Redirect("/AssetsActions/RenovationAsset/GetAssetsForRenovating");
        }

        //GET: Get assets which are renovating
        [HttpGet]
        [RightCheck(Right = "Send request for renovation")]
        public ActionResult ReturnedAssets()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var requests = this.requestForRenovationService.GetAll()
                .Where(x => !x.IsFinished)
                .Where(x => x.IsSeenFromApprover)
                .Where(x => x.IsAssetGave)
                .Where(x => x.FromId == this.User.Identity.GetUserId())
                .Where(x => x.AssetIsReturned);

            if (!this.IsMegaAdmin())
            {
                requests = requests.Where(x => x.From.Site.OrganisationId == userOrg);
            }

            var viewModel = requests.ToList().ConvertAll(
                x => new RequestForRenovationViewModel
                {
                    InventoryNumber = x.Asset.InventoryNumber,
                    Id = x.Id,
                    FromName = (x.From.FirstName != null) ?
                   x.From.FirstName + " " + x.From.SecondName + " " + x.From.LastName :
                   x.From.Email,
                    DateOfSend = x.DateOfSend.ToString()
                });
            return View(viewModel);

        }

        //GET: View returned asset
        [HttpGet]
        [RightCheck(Right = "Send request for renovation")]
        public ActionResult ReturnedAsset(int id)
        {
            var request = this.requestForRenovationService.GetById(id);

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

            var viewModel = new RenovationHistoryRequest
            {
                Brand = asset.Brand,
                Producer = asset.Producer,
                InventoryNumber = asset.InventoryNumber,
                ItemModel = asset.Model,
                Guarantee = asset.Guarantee,
                Price = asset.Price.Value,
                Type = asset.Type.ToString(),
                SiteName = request.Asset.Site.Name,
                FromId = user.Id,
                FromName = (user.FirstName != null && user.SecondName != null && user.LastName != null) ?
                        user.FirstName + " " + user.SecondName + " " + user.LastName : user.Email,
                Id = id,
                IsApproved = request.IsApproved,
                IsRenovated = request.IsAssetRenovated,
                Currency = request.Asset.Price.Currency.Code
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RightCheck(Right = "Send request for renovation")]
        public ActionResult ReceiceAcceptance(int id)
        {
            var request = this.requestForRenovationService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            //Set asset is received in packing slip
            this.packingSlipService.SetReceived(request.AcceptancePackingSlipId.Value, DateTime.Now);

            //Set request is finished
            this.requestForRenovationService.SetFinished(id);

            return Redirect("/AssetsActions/RenovationAsset/ReturnedAssets");
        }

        //GET: View your own history requests
        [HttpGet]
        [RightCheck(Right = "Send request for renovation")]
        public ActionResult HistoryRequests()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var requests = this.requestForRenovationService.GetAll()
                .Where(x => x.IsFinished)
                .Where(x => x.FromId == this.User.Identity.GetUserId())
                .Where(x => x.From.Site.OrganisationId == userOrg);

            var viewModel = requests.ToList().ConvertAll(
                x => new RequestForRenovationViewModel
                {
                    DateOfSend = x.DateOfSend.ToString(),
                    InventoryNumber = x.Asset.InventoryNumber,
                    FromName = (x.From.FirstName != null) ?
                    x.From.FirstName + " " + x.From.SecondName + " " + x.From.LastName :
                    x.From.Email,
                    Id = x.Id
                });

            return View(viewModel);
        }

        //GET: View your own history request
        [HttpGet]
        [RightCheck(Right = "Send request for renovation")]
        public ActionResult ViewHistoryRequest(int id)
        {
            var request = this.requestForRenovationService.GetById(id);

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

            var viewModel = new RenovationHistoryRequest
            {
                Brand = asset.Brand,
                Producer = asset.Producer,
                InventoryNumber = asset.InventoryNumber,
                ItemModel = asset.Model,
                Guarantee = asset.Guarantee,
                Price = asset.Price.Value,
                Type = asset.Type.ToString(),
                SiteName = request.Asset.Site.Name,
                FromId = user.Id,
                FromName = (user.FirstName != null && user.SecondName != null && user.LastName != null) ?
                        user.FirstName + " " + user.SecondName + " " + user.LastName : user.Email,
                Id = id,
                IsApproved = request.IsApproved,
                IsRenovated = request.IsAssetRenovated,
                IssuePackingSlipId = request.IssuePackingSlip != null ? request.IssuePackingSlipId.Value : 0,
                AcceptancePackingSlipId = request.AcceptancePackingSlip != null ? request.AcceptancePackingSlipId.Value : 0,
                Currency = request.Asset.Price.Currency.Code
            };

            return View(viewModel);
        }

        //GET: View all history requests
        [HttpGet]
        [RightCheck(Right = "Approve request for renovation")]
        public ActionResult AllHistoryRequests()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var requests = this.requestForRenovationService.GetAll()
                .Where(x => x.IsFinished);

            if (!this.IsMegaAdmin())
            {
                requests = requests.Where(x => x.From.Site.OrganisationId == userOrg);
            }

            var viewModel = requests.ToList().ConvertAll(
                 x => new RequestForRenovationViewModel
                 {
                     DateOfSend = x.DateOfSend.ToString(),
                     InventoryNumber = x.Asset.InventoryNumber,
                     FromName = (x.From.FirstName != null) ?
                     x.From.FirstName + " " + x.From.SecondName + " " + x.From.LastName :
                     x.From.Email,
                     Id = x.Id
                 });


            return View(viewModel);
        }

        //GET: View a history request
        [HttpGet]
        [RightCheck(Right = "Approve request for renovation")]
        public ActionResult ViewAllHistoryRequest(int id)
        {
            var request = this.requestForRenovationService.GetById(id);

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

            var viewModel = new RenovationHistoryRequest
            {
                Brand = asset.Brand,
                Producer = asset.Producer,
                InventoryNumber = asset.InventoryNumber,
                ItemModel = asset.Model,
                Price = asset.Price.Value,
                Guarantee = asset.Guarantee,
                Type = asset.Type.ToString(),
                SiteName = request.Asset.Site.Name,
                FromId = user.Id,
                FromName = (user.FirstName != null && user.SecondName != null && user.LastName != null) ?
                        user.FirstName + " " + user.SecondName + " " + user.LastName : user.Email,
                Id = id,
                IsApproved = request.IsApproved,
                IsRenovated = request.IsAssetRenovated,
                IssuePackingSlipId = request.IssuePackingSlipId.Value,
                AcceptancePackingSlipId = request.AcceptancePackingSlipId.Value,
                Currency = request.Asset.Price.Currency.Code
            };


            return View("ViewHistoryRequest", viewModel);
        }
    }
}