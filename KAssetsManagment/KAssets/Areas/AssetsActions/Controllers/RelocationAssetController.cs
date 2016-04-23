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
using KAssets.Areas.Admin.Models;
using KAssets.Areas.HelpModule.Models;


namespace KAssets.Areas.AssetsActions.Controllers
{
    [Authorize]
    [HasSite]
    public class RelocationAssetController : BaseController
    {
        //GET: Choose asset
        [HttpGet]
        [RightCheck(Right = "Send request for relocation")]
        public ActionResult ChooseAsset()
        {
            var userSite = this.userService.GetById(this.User.Identity.GetUserId()).Site;
            var assets = this.assetService.GetAll().Where(x => x.Status == "Active");
            if (!this.IsMegaAdmin())
            {
                assets = assets.Where(x => x.Site.OrganisationId == userSite.OrganisationId);
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

        //GET: Choose users
        [HttpGet]
        [RightCheck(Right = "Send request for relocation")]
        public ActionResult ChooseUsers(string id)
        {
            var users = this.userService.GetAll().Where(x => x.Status == "Active")
                .Where(x => x.Site != null);

            if (id != null && id != "")
            {
                var asset = this.assetService.GetById(id);
                users = users.Where(x => x.Site.OrganisationId == asset.Site.OrganisationId);
            }
            else
            {
                var userSite = this.userService.GetById(this.User.Identity.GetUserId()).Site;
                users = users.Where(x => x.Site.OrganisationId == userSite.OrganisationId);
            }

            var viewModel = users.ToList().ConvertAll(
                x => new ShowUserViewModel
                {
                    Id = x.Id,
                    Name = (x.FirstName != null) ?
                    x.FirstName + " " + x.SecondName + " " + x.LastName :
                    x.Email
                });

            return PartialView(viewModel);
        }

        //GET: Choose location
        [RightCheck(Right = "Send request for relocation")]
        public ActionResult ChooseLocation(string id)
        {
            var locations = this.locationService.GetAll();

            if (id != null && id != "")
            {
                var asset = this.assetService.GetById(id);
                locations = locations.Where(x => x.OrganisationId == asset.Site.OrganisationId).ToList();
            }
            else
            {
                var userSite = this.userService.GetById(this.User.Identity.GetUserId()).Site;
                locations = locations.Where(x => x.OrganisationId == userSite.OrganisationId).ToList();

            }


            var viewModel = locations.ToList().ConvertAll(
                x => new LocationViewModel
                {
                    Code = x.Code,
                    Country = x.Country,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    Street = x.Street,
                    StreetNumber = x.StreetNumber,
                    Town = x.Town
                });

            return PartialView(viewModel);
        }

        //GET: Get all sites for a organisation
        [HttpGet]
        public ActionResult GetAllPartial(string id)
        {
            var organisationId = 0;
            if (id != null && id != "")
            {
                organisationId = this.assetService.GetById(id).Site.OrganisationId;
            }
            else
            {
                organisationId = this.userService.GetById(this.User.Identity.GetUserId()).Site.OrganisationId;
            }

            var sites = this.siteService.GetForOrganisation(organisationId).
                        ToList().ConvertAll(
                            x => new SiteViewModel
                            {
                                Id = x.Id,
                                Name = x.Name
                            });

            var viewModel = new ShowSitesViewModel
            {
                OrganisationId = organisationId,
                Sites = sites
            };

            return PartialView(viewModel);
        }


        //GET: Choose users from site
        [HttpGet]
        [RightCheck(Right = "Send request for relocation")]
        public ActionResult ChooseUsersFromSite(int id)
        {
            var users = this.userService.GetAll().Where(x => x.Status == "Active")
                .Where(x => x.SiteId == id);

            var viewModel = users.ToList().ConvertAll(
                x => new ShowUserViewModel
                {
                    Id = x.Id,
                    Name = (x.FirstName != null) ?
                    x.FirstName + " " + x.SecondName + " " + x.LastName :
                    x.Email
                });

            return PartialView("ChooseUsers", viewModel);
        }

        // GET: Create a relocation request
        [HttpGet]
        [RightCheck(Right = "Send request for relocation")]
        public ActionResult CreateRelocationRequest()
        {
            return View();
        }

        //GET: Get user id
        [HttpGet]
        public JsonResult GetUserSiteId(string id)
        {
            var siteId = this.userService.GetById(id).SiteId;

            return Json(siteId, JsonRequestBehavior.AllowGet);
        }

        //GET: Get user' location id
        [HttpGet]
        public JsonResult GetUserLocationId(string id)
        {
            var locationId = this.userService.GetById(id).LocationId;

            return Json(locationId, JsonRequestBehavior.AllowGet);
        }

        //POST: Create a relocation request
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RightCheck(Right = "Send request for relocation")]
        public ActionResult CreateRelocationRequest(CreateRelocationRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Check is selected site cointains selected user
            if (model.ToUser != null)
            {
                if (this.userService.GetById(model.ToUser).Site != null)
                {
                    if (!this.siteService.GetById(int.Parse(model.ToSite)).Users.Select(x => x.Id).Contains(model.ToUser))
                    {
                        return View(model);
                    }
                }
            }

            //Check if exist request with same asset id
            if (this.requestForRelocationService.Exist(model.SelectedAsset))
            {
                return Redirect("/AssetsActions/RelocationAsset/ThereIsRequest");
            }

            if (model.ToUser != null)
            {
                var user = this.userService.GetById(model.ToUser);
                model.ToLocation = user.Location != null ? user.LocationId.ToString() : "";
                model.ToSite = user.Site != null ? user.SiteId.Value.ToString() : "";
            }

            //Add a new request
            var asset = this.assetService.GetById(model.SelectedAsset);
            this.requestForRelocationService.Add(new RequestForRelocation
                {
                    AssetId = model.SelectedAsset,
                    DateOfSend = DateTime.Now,
                    FromId = this.User.Identity.GetUserId(),
                    ToSiteId = int.Parse(model.ToSite),
                    OldSiteName = asset.Site.Name,
                    ToLocationId = model.ToLocation != "" ? model.ToLocation : null,
                    ToUserId = model.ToUser != "" ? model.ToUser : null,
                    OldLocationCode = asset.Location != null ? asset.LocationId : null,
                    OldUserId = asset.User != null ? asset.UserId : null
                });

            //Adda event that is added a new request
            this.eventService.AddForUserGroup(new Event
                {
                    Content = "You have a new requst for relocation for approving !",
                    Date = DateTime.Now,
                    EventRelocationUrl = "/AssetsActions/RelocationAsset/GetRequestsForApproving"
                }, "Approve request for relocation",
                 this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()));

            return Redirect("/AssetsActions/RelocationAsset/SuccessSend");
        }

        //GET: Get requests for approving
        [HttpGet]
        [RightCheck(Right = "Approve request for relocation")]
        public ActionResult GetRequestsForApproving()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var requests = this.requestForRelocationService.GetAll()
                .Where(x => !x.SeenFromApprover)
                .Where(x => !x.IsFinished);

            if (!this.IsMegaAdmin())
            {
                requests = requests.Where(x => x.From.Site.OrganisationId == userOrg);
            }

            var viewModel = requests.ToList().ConvertAll(
                x => new RequestForRelocationViewModel
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

        //GET: View request for approving
        [HttpGet]
        [RightCheck(Right = "Approve request for relocation")]
        public ActionResult ViewRequestForApproving(int id)
        {
            var request = this.requestForRelocationService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }
            var oldUserName = this.userService.GetById(request.OldUserId);

            //Set request data to viewmodel
            var viewModel = new RequestForRelocationDetails
            {
                DateOfSend = request.DateOfSend.ToString(),
                FromName = (request.From.FirstName != null) ?
                    request.From.FirstName + " " + request.From.SecondName + " " + request.From.LastName :
                    request.From.Email,
                ToSiteName = request.ToSite.Name,
                Id = request.Id
            };

            //Set asset data to viewmodel
            viewModel.Asset = new AssetDetailsViewModel
            {
                Brand = request.Asset.Brand,
                Guarantee = request.Asset.Guarantee,
                InventoryNumber = request.Asset.InventoryNumber,
                ItemModel = request.Asset.Model,
                Price = request.Asset.Price.Value,
                Producer = request.Asset.Producer,
                Type = request.Asset.Type,
                UserName = request.Asset.User != null ? ((request.Asset.User.FirstName != null) ?
                    request.Asset.User.FirstName + " " + request.Asset.User.SecondName + " " + request.Asset.User.LastName :
                    request.Asset.User.Email) : "",
                Currency = request.Asset.Price.Currency.Code
            };

            if (request.Asset.Location != null)
            {
                viewModel.Asset.Location = new LocationViewModel
                {
                    Country = request.Asset.Location.Country != null ? request.Asset.Location.Country + " " : " ",
                    Latitude = request.Asset.Location.Latitude != null ? request.Asset.Location.Latitude + " " : " ",
                    Longitude = request.Asset.Location.Longitude != null ? request.Asset.Location.Longitude + " " : " ",
                    Street = request.Asset.Location.Street != null ? " " + request.Asset.Location.Street + " " : " ",
                    StreetNumber = request.Asset.Location.StreetNumber != null ? request.Asset.Location.StreetNumber.Value : 0,
                    Town = request.Asset.Location.Town != null ? request.Asset.Location.Town + " " : " ",

                };
            }
            viewModel.Asset.SiteName = request.Asset.Site.Name;

            if (request.ToLocation != null)
            {
                viewModel.ToLocation =
                    (request.ToLocation.Country != null ? request.ToLocation.Country : " ") + " " +
                    (request.ToLocation.Latitude != null ? request.ToLocation.Latitude : " ") + " " + " " +
                    (request.ToLocation.Longitude != null ? request.ToLocation.Longitude : " ") + " " +
                    (request.ToLocation.Town != null ? request.ToLocation.Town : " ") + " " +
                    (request.ToLocation.Street != null ? request.ToLocation.Street : " ") + " " +
                    (request.ToLocation.StreetNumber != null ? request.ToLocation.StreetNumber.Value.ToString() : " ");

            }

            if (request.ToUser != null)
            {
                viewModel.ToUser =
                  (request.ToUser.FirstName != null) ?
                  (request.ToUser.FirstName + " " + request.ToUser.SecondName + " " + request.ToUser.LastName) :
                  (request.ToUser.Email);

            }
            return View(viewModel);
        }

        //POST: Approve a request for relocation
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RightCheck(Right = "Approve request for relocation")]
        public ActionResult ApproveRequest(int id)
        {
            var request = this.requestForRelocationService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            //Set request is approved
            this.requestForRelocationService.SetApproved(id);

            //Set request is seen by approver
            this.requestForRelocationService.SetSeenFromApprover(id);

            //Add event that request is approved
            var relocateUrl = "";

            if (request.OldUserId != null)
            {
                relocateUrl = "/AssetsActions/RelocationAsset/RequestsForIssueWithUser";
            }
            else
            {
                relocateUrl = "/AssetsActions/RelocationAsset/RequestsForIssueWithOutUser";
            }

            this.eventService.Add(
                new Event
                {
                    UserId = request.FromId,
                    Content = "Your request for relocate the asset with inventory number " + request.Asset.InventoryNumber + " was approved. ",
                    Date = DateTime.Now,
                    EventRelocationUrl = relocateUrl
                });

            //Add a new packing slip for request
            var pSId = this.packingSlipService.Add(new PackingSlip());
            this.requestForRelocationService.AddPackingSlip(id, pSId);

            return Redirect("/AssetsActions/RelocationAsset/GetRequestsForApproving");
        }

        //GET: Get requests for issue with user
        [HttpGet]
        //[RightCheck(Right = "Send request for relocation")]
        public ActionResult RequestsForIssueWithUser()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var requests = this.requestForRelocationService.GetAll()
                .Where(x => !x.IsFinished)
                .Where(x => x.IsApproved)
                .Where(x => x.SeenFromApprover)
                .Where(x => x.PackingSlip.FromUser == null)
                .Where(x => x.OldUserId != null)
                .Where(x => x.OldUserId == this.User.Identity.GetUserId());


            var viewModel = requests.ToList().ConvertAll(
                x => new RequestForRelocationViewModel
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

        //GET: Get requests for issue without user
        [HttpGet]
        [RightCheck(Right = "Approve request for relocation")]
        public ActionResult RequestsForIssueWithOutUser()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var requests = this.requestForRelocationService.GetAll()
                .Where(x => !x.IsFinished)
                .Where(x => x.IsApproved)
                .Where(x => x.SeenFromApprover)
                .Where(x => x.PackingSlip.FromUser == null)
                .Where(x => x.OldUserId == null);


            if (!this.IsMegaAdmin())
            {
                requests = requests.Where(x => x.From.Site.OrganisationId == userOrg);
            }

            var viewModel = requests.ToList().ConvertAll(
                x => new RequestForRelocationViewModel
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

        //GET: Get success notification page for sending
        [HttpGet]
        [RightCheck(Right = "Send request for relocation")]
        public ActionResult SuccessSend()
        {
            return View();
        }

        //GET: There is a request for this asset
        [HttpGet]
        [RightCheck(Right = "Send request for relocation")]
        public ActionResult ThereIsRequest()
        {
            return View();
        }

        //GET: Decline a request
        [HttpGet]
        [RightCheck(Right = "Approve request for relocation")]
        public ActionResult Decline(int id)
        {
            var request = this.requestForRelocationService.GetById(id);

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
        [RightCheck(Right = "Approve request for relocation")]
        [ValidateAntiForgeryToken]
        public ActionResult Decline(DeclineRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var request = this.requestForRelocationService.GetById(model.RequestId);

            //Add event that request is not approved
            var aEvent = new Event
            {
                UserId = request.FromId,
                Content = "Your request for relocate the asset with inventory number " + request.Asset.InventoryNumber + " was not approved. " + model.Message,
                Date = DateTime.Now,
                EventRelocationUrl = "/AssetsActions/RelocationAsset/ViewHistoryRequest/" + model.RequestId
            };

            this.eventService.Add(aEvent);
            this.requestForRelocationService.SetFinished(model.RequestId);

            return Redirect("/AssetsActions/RelocationAsset/GetRequestsForApproving");
        }

        //GET: View request for giving asset with user
        [HttpGet]
        //[RightCheck(Right = "Send request for relocation")]
        public ActionResult ViewRequestForGivingAssetWithUser(int id)
        {
            var request = this.requestForRelocationService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var oldUserName = this.userService.GetById(request.OldUserId);

            var viewModel = new RequestForRelocationDetails
            {
                DateOfSend = request.DateOfSend.ToString(),
                FromName = (request.From.FirstName != null) ?
                    request.From.FirstName + " " + request.From.SecondName + " " + request.From.LastName :
                    request.From.Email,
                ToSiteName = request.ToSite.Name,
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
                UserName = request.Asset.User != null ? ((request.Asset.User.FirstName != null) ?
                    request.Asset.User.FirstName + " " + request.Asset.User.SecondName + " " + request.Asset.User.LastName :
                    request.Asset.User.Email) : "",
                Currency = request.Asset.Price.Currency.Code
            };

            if (request.Asset.Location != null)
            {
                viewModel.Asset.Location = new LocationViewModel
                {
                    Country = request.Asset.Location.Country != null ? request.Asset.Location.Country : " ",
                    Latitude = request.Asset.Location.Latitude != null ? request.Asset.Location.Latitude : " ",
                    Longitude = request.Asset.Location.Longitude != null ? request.Asset.Location.Longitude : " ",
                    Street = request.Asset.Location.Street != null ? request.Asset.Location.Street : " ",
                    StreetNumber = request.Asset.Location.StreetNumber != null ? request.Asset.Location.StreetNumber.Value : 0,
                    Town = request.Asset.Location.Town != null ? request.Asset.Location.Town : " ",

                };
            }
            viewModel.Asset.SiteName = request.Asset.Site.Name;

            if (request.ToLocation != null)
            {
                viewModel.ToLocation =
                    (request.ToLocation.Country != null ? request.ToLocation.Country : " ") + " " +
                    (request.ToLocation.Latitude != null ? request.ToLocation.Latitude : " ") + " " + " " +
                    (request.ToLocation.Longitude != null ? request.ToLocation.Longitude : " ") + " " +
                    (request.ToLocation.Town != null ? request.ToLocation.Town : " ") + " " +
                    (request.ToLocation.Street != null ? request.ToLocation.Street : " ") + " " +
                    (request.ToLocation.StreetNumber != null ? request.ToLocation.StreetNumber.Value.ToString() : " ");

            }

            if (request.ToUser != null)
            {
                viewModel.ToUser =
                  (request.ToUser.FirstName != null) ?
                  (request.ToUser.FirstName + " " + request.ToUser.SecondName + " " + request.ToUser.LastName) :
                  (request.ToUser.Email);

            }
            return View(viewModel);
        }

        //GET: View request for giving asset without user
        [HttpGet]
        [RightCheck(Right = "Approve request for relocation")]
        public ActionResult ViewRequestForGivingAssetWithOutUser(int id)
        {
            var request = this.requestForRelocationService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var oldUserName = this.userService.GetById(request.OldUserId);

            var viewModel = new RequestForRelocationDetails
            {
                DateOfSend = request.DateOfSend.ToString(),
                FromName = (request.From.FirstName != null) ?
                    request.From.FirstName + " " + request.From.SecondName + " " + request.From.LastName :
                    request.From.Email,
                ToSiteName = request.ToSite.Name,
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
                UserName = request.Asset.User != null ? ((request.Asset.User.FirstName != null) ?
                    request.Asset.User.FirstName + " " + request.Asset.User.SecondName + " " + request.Asset.User.LastName :
                    request.Asset.User.Email) : "",
                Currency = request.Asset.Price.Currency.Code
            };

            if (request.Asset.Location != null)
            {
                viewModel.Asset.Location = new LocationViewModel
                {
                    Country = request.Asset.Location.Country != null ? request.Asset.Location.Country : " ",
                    Latitude = request.Asset.Location.Latitude != null ? request.Asset.Location.Latitude : " ",
                    Longitude = request.Asset.Location.Longitude != null ? request.Asset.Location.Longitude : " ",
                    Street = request.Asset.Location.Street != null ? request.Asset.Location.Street : " ",
                    StreetNumber = request.Asset.Location.StreetNumber != null ? request.Asset.Location.StreetNumber.Value : 0,
                    Town = request.Asset.Location.Town != null ? request.Asset.Location.Town : " ",

                };
            }
            viewModel.Asset.SiteName = request.Asset.Site.Name;

            if (request.ToLocation != null)
            {
                viewModel.ToLocation =
                    (request.ToLocation.Country != null ? request.ToLocation.Country : " ") + " " +
                    (request.ToLocation.Latitude != null ? request.ToLocation.Latitude : " ") + " " + " " +
                    (request.ToLocation.Longitude != null ? request.ToLocation.Longitude : " ") + " " +
                    (request.ToLocation.Town != null ? request.ToLocation.Town : " ") + " " +
                    (request.ToLocation.Street != null ? request.ToLocation.Street : " ") + " " +
                    (request.ToLocation.StreetNumber != null ? request.ToLocation.StreetNumber.Value.ToString() : " ");

            }

            if (request.ToUser != null)
            {
                viewModel.ToUser =
                  (request.ToUser.FirstName != null) ?
                  (request.ToUser.FirstName + " " + request.ToUser.SecondName + " " + request.ToUser.LastName) :
                  (request.ToUser.Email);

            }
            return View(viewModel);
        }

        //POST: Give items without user
        [HttpPost]
        [RightCheck(Right = "Approve request for relocation")]
        [ValidateAntiForgeryToken]
        public ActionResult GiveAssetWithOutUsers(int id)
        {
            var request = this.requestForRelocationService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }
            //Set assets are given
            this.requestForRelocationService.SetAssetGiven(id);

            //Add event that assets are givem
            var aEvent = new Event
            {
                Content = "You have a new asset with inventory number " + request.Asset.InventoryNumber + " for receiving. ",
                Date = DateTime.Now
            };

            if (request.ToUser != null)
            {
                aEvent.EventRelocationUrl = "/AssetsActions/RelocationAsset/RequestsForReceivingWithUser";
                aEvent.UserId = request.ToUserId;
                this.eventService.Add(aEvent);
            }
            else
            {
                aEvent.EventRelocationUrl = "/RelocationAsset/RequestsForReceivingWithOutUser";
                aEvent.UserId = request.ToUserId;
                this.eventService.AddForUserGroup(aEvent, "Approve request for relocation",
                     this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()));
            }

            //Set assets are given and from in packing slip
            this.packingSlipService.SetGiven(request.PackingSlipId.Value, DateTime.Now);

            this.packingSlipService.SetFromUser(request.PackingSlipId.Value, this.User.Identity.GetUserId());

            return Redirect("/AssetsActions/RelocationAsset/RequestsForIssueWithOutUser");
        }

        //POST: Give asset with user
        [HttpPost]
        //[RightCheck(Right = "Send request for relocation")]
        [ValidateAntiForgeryToken]
        public ActionResult GiveAssetWithUsers(int id)
        {
            var request = this.requestForRelocationService.GetById(id);
            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            //Set assets are given
            this.requestForRelocationService.SetAssetGiven(id);

            //Add event that assets are given
            var aEvent = new Event
            {
                Content = "You have a new asset with inventory number " + request.Asset.InventoryNumber + " for receiving. ",
                Date = DateTime.Now
            };

            if (request.ToUser != null)
            {
                aEvent.EventRelocationUrl = "/AssetsActions/RelocationAsset/RequestsForReceivingWithUser";
                aEvent.UserId = request.ToUserId;
                this.eventService.Add(aEvent);
            }
            else
            {
                aEvent.EventRelocationUrl = "/AssetsActions/RelocationAsset/RequestsForReceivingWithOutUser";
                aEvent.UserId = request.ToUserId;
                this.eventService.AddForUserGroup(aEvent, "Approve request for relocation",
                     this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()));
            }

            //Add assets are given and from to packing slip
            this.packingSlipService.SetGiven(request.PackingSlipId.Value, DateTime.Now);

            this.packingSlipService.SetFromUser(request.PackingSlipId.Value, this.User.Identity.GetUserId());

            return Redirect("/AssetsActions/RelocationAsset/RequestsForIssueWithUser");
        }

        //GET: Get requests for receiving with user
        [HttpGet]
        //[RightCheck(Right = "Send request for relocation")]
        public ActionResult RequestsForReceivingWithUser()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var requests = this.requestForRelocationService.GetAll()
                .Where(x => !x.IsFinished)
                .Where(x => x.IsApproved)
                .Where(x => x.SeenFromApprover)
                .Where(x => x.PackingSlip.FromUser != null)
                .Where(x => x.ToUserId != null)
                .Where(x => x.ToUserId == this.User.Identity.GetUserId())
                .Where(x => x.From.Site.OrganisationId == userOrg);

            var viewModel = requests.ToList().ConvertAll(
                x => new RequestForRelocationViewModel
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

        //GET: Get requests for receiving without user
        [HttpGet]
        [RightCheck(Right = "Approve request for relocation")]
        public ActionResult RequestsForReceivingWithOutUser()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var requests = this.requestForRelocationService.GetAll()
                .Where(x => !x.IsFinished)
                .Where(x => x.IsApproved)
                .Where(x => x.SeenFromApprover)
                .Where(x => x.PackingSlip.FromUser != null)
                .Where(x => x.ToUserId == null);

            if (!this.IsMegaAdmin())
            {
                requests = requests.Where(x => x.From.Site.OrganisationId == userOrg);
            }

            var viewModel = requests.ToList().ConvertAll(
                x => new RequestForRelocationViewModel
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

        //GET: View request for receiving with user
        [HttpGet]
        //[RightCheck(Right = "Send request for relocation")]
        public ActionResult ViewRequestForReceivingAssetWithUser(int id)
        {
            var request = this.requestForRelocationService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var oldUserName = this.userService.GetById(request.OldUserId);

            var viewModel = new RequestForRelocationDetails
            {
                DateOfSend = request.DateOfSend.ToString(),
                FromName = (request.From.FirstName != null) ?
                    request.From.FirstName + " " + request.From.SecondName + " " + request.From.LastName :
                    request.From.Email,
                ToSiteName = request.ToSite.Name,
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
                UserName = request.Asset.User != null ? ((request.Asset.User.FirstName != null) ?
                    request.Asset.User.FirstName + " " + request.Asset.User.SecondName + " " + request.Asset.User.LastName :
                    request.Asset.User.Email) : "",
                Currency = request.Asset.Price.Currency.Code
            };

            if (request.Asset.Location != null)
            {
                viewModel.Asset.Location = new LocationViewModel
                {
                    Country = request.Asset.Location.Country != null ? request.Asset.Location.Country : " ",
                    Latitude = request.Asset.Location.Latitude != null ? request.Asset.Location.Latitude : " ",
                    Longitude = request.Asset.Location.Longitude != null ? request.Asset.Location.Longitude : " ",
                    Street = request.Asset.Location.Street != null ? request.Asset.Location.Street : " ",
                    StreetNumber = request.Asset.Location.StreetNumber != null ? request.Asset.Location.StreetNumber.Value : 0,
                    Town = request.Asset.Location.Town != null ? request.Asset.Location.Town : " ",

                };
            }
            viewModel.Asset.SiteName = request.Asset.Site.Name;

            if (request.ToLocation != null)
            {
                viewModel.ToLocation =
                    (request.ToLocation.Country != null ? request.ToLocation.Country : " ") + " " +
                    (request.ToLocation.Latitude != null ? request.ToLocation.Latitude : " ") + " " + " " +
                    (request.ToLocation.Longitude != null ? request.ToLocation.Longitude : " ") + " " +
                    (request.ToLocation.Town != null ? request.ToLocation.Town : " ") + " " +
                    (request.ToLocation.Street != null ? request.ToLocation.Street : " ") + " " +
                    (request.ToLocation.StreetNumber != null ? request.ToLocation.StreetNumber.Value.ToString() : " ");

            }

            if (request.ToUser != null)
            {
                viewModel.ToUser =
                  (request.ToUser.FirstName != null) ?
                  (request.ToUser.FirstName + " " + request.ToUser.SecondName + " " + request.ToUser.LastName) :
                  (request.ToUser.Email);

            }
            return View(viewModel);
        }

        //GET: View request for receiving without user
        [HttpGet]
        [RightCheck(Right = "Approve request for relocation")]
        public ActionResult ViewRequestForRecevingAssetWithOutUser(int id)
        {
            var request = this.requestForRelocationService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var oldUserName = this.userService.GetById(request.OldUserId);

            var viewModel = new RequestForRelocationDetails
            {
                DateOfSend = request.DateOfSend.ToString(),
                FromName = (request.From.FirstName != null) ?
                    request.From.FirstName + " " + request.From.SecondName + " " + request.From.LastName :
                    request.From.Email,
                ToSiteName = request.ToSite.Name,
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
                UserName = request.Asset.User != null ? ((request.Asset.User.FirstName != null) ?
                    request.Asset.User.FirstName + " " + request.Asset.User.SecondName + " " + request.Asset.User.LastName :
                    request.Asset.User.Email) : "",
                Currency = request.Asset.Price.Currency.Code
            };

            if (request.Asset.Location != null)
            {
                viewModel.Asset.Location = new LocationViewModel
                {
                    Country = request.Asset.Location.Country != null ? request.Asset.Location.Country : " ",
                    Latitude = request.Asset.Location.Latitude != null ? request.Asset.Location.Latitude : " ",
                    Longitude = request.Asset.Location.Longitude != null ? request.Asset.Location.Longitude : " ",
                    Street = request.Asset.Location.Street != null ? request.Asset.Location.Street : " ",
                    StreetNumber = request.Asset.Location.StreetNumber != null ? request.Asset.Location.StreetNumber.Value : 0,
                    Town = request.Asset.Location.Town != null ? request.Asset.Location.Town : " ",

                };
            }
            viewModel.Asset.SiteName = request.Asset.Site.Name;

            if (request.ToLocation != null)
            {
                viewModel.ToLocation =
                    (request.ToLocation.Country != null ? request.ToLocation.Country : " ") + " " +
                    (request.ToLocation.Latitude != null ? request.ToLocation.Latitude : " ") + " " + " " +
                    (request.ToLocation.Longitude != null ? request.ToLocation.Longitude : " ") + " " +
                    (request.ToLocation.Town != null ? request.ToLocation.Town : " ") + " " +
                    (request.ToLocation.Street != null ? request.ToLocation.Street : " ") + " " +
                    (request.ToLocation.StreetNumber != null ? request.ToLocation.StreetNumber.Value.ToString() : " ");

            }

            if (request.ToUser != null)
            {
                viewModel.ToUser =
                  (request.ToUser.FirstName != null) ?
                  (request.ToUser.FirstName + " " + request.ToUser.SecondName + " " + request.ToUser.LastName) :
                  (request.ToUser.Email);

            }
            return View(viewModel);
        }

        //POST: Receive asset without user
        [HttpPost]
        [RightCheck(Right = "Approve request for relocation")]
        [ValidateAntiForgeryToken]
        public ActionResult ReceiveAssetWithOutUsers(int id)
        {
            var request = this.requestForRelocationService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }
            //Set request is finished
            this.requestForRelocationService.SetFinished(id);

            //Change asset site
            this.assetService.Relocate(request.AssetId, request.ToSiteId);

            //Change asset location
            this.assetService.ChangeLocation(request.AssetId, request.ToLocationId);

            //Change asset user
            this.assetService.ChangeUser(request.AssetId, request.ToUserId);

            //Set asset is  received in packing slip
            this.packingSlipService.SetReceived(request.PackingSlipId.Value, DateTime.Now);

            //Set received from in packing slip
            this.packingSlipService.SetToUser(request.PackingSlipId.Value, this.User.Identity.GetUserId());

            //Add history row that asset is relocated
            this.assetHistoryService.AddHistoryRow(new HistoryRow
            {
                Content = "The asset was relocated.",
                Date = DateTime.Now
            }, request.Asset.InventoryNumber);


            //Add event that asset is relocated
            this.eventService.Add(
             new Event
             {
                 UserId = request.FromId,
                 Content = "Аsset with inventory number " + request.Asset.InventoryNumber + " was relocated. ",
                 Date = DateTime.Now
             });

            return Redirect("/AssetsActions/RelocationAsset/RequestsForReceivingWithOutUser");
        }

        //POST: Receive asset with user
        [HttpPost]
        //[RightCheck(Right = "Send request for relocation")]
        [ValidateAntiForgeryToken]
        public ActionResult ReceiveAssetWithUsers(int id)
        {
            var request = this.requestForRelocationService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            //Set request is finished
            this.requestForRelocationService.SetFinished(id);

            //Change asset site
            this.assetService.Relocate(request.AssetId, request.ToSiteId);

            //Change asset location
            this.assetService.ChangeLocation(request.AssetId, request.ToLocationId);

            //Change asset user
            this.assetService.ChangeUser(request.AssetId, request.ToUserId);

            //Set asset is received in packing slip
            this.packingSlipService.SetReceived(request.PackingSlipId.Value, DateTime.Now);

            //Set to user in packing slip
            this.packingSlipService.SetToUser(request.PackingSlipId.Value, this.User.Identity.GetUserId());

            //Add a new event that assets is relocated
            this.eventService.Add(
             new Event
             {
                 UserId = request.FromId,
                 Content = "Аsset with inventory number " + request.Asset.InventoryNumber + " was relocated. ",
                 Date = DateTime.Now
             });

            //Add a new history row taht asset was relcoated
            this.assetHistoryService.AddHistoryRow(new HistoryRow
            {
                Content = "The asset was relocated.",
                Date = DateTime.Now
            }, request.Asset.InventoryNumber);

            return Redirect("/AssetsActions/RelocationAsset/RequestsForReceivingWithUser");
        }

        //GET: View your own history requests
        [HttpGet]
        [RightCheck(Right = "Send request for relocation")]
        public ActionResult HistoryRequests()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var requests = this.requestForRelocationService.GetAll()
                .Where(x => x.IsFinished)
                .Where(x => x.FromId == this.User.Identity.GetUserId())
                .Where(x => x.From.Site.OrganisationId == userOrg);

            var viewModel = requests.ToList().ConvertAll(
                x => new RequestForRelocationViewModel
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
        [RightCheck(Right = "Send request for relocation")]
        public ActionResult ViewHistoryRequest(int id)
        {
            var request = this.requestForRelocationService.GetById(id);

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

            var viewModel = new RelocationHistoryRequest
            {
                Brand = asset.Brand,
                Producer = asset.Producer,
                InventoryNumber = asset.InventoryNumber,
                ItemModel = asset.Model,
                Guarantee = asset.Guarantee,
                Type = asset.Type.ToString(),
                Price = asset.Price.Value,
                SiteName = request.OldSiteName,
                FromId = user.Id,
                FromName = (user.FirstName != null && user.SecondName != null && user.LastName != null) ?
                        user.FirstName + " " + user.SecondName + " " + user.LastName : user.Email,
                Id = id,
                IsApproved = request.IsApproved,
                ToSite = request.ToSite.Name,
                PackingSlipId = request.PackingSlip != null ? request.PackingSlipId.Value : 0,
                Currency = request.Asset.Price.Currency.Code
            };
            if (request.ToLocation != null)
            {
                viewModel.ToLocation =
                    (request.ToLocation.Country != null ? request.ToLocation.Country : " ") + " " +
                    (request.ToLocation.Latitude != null ? request.ToLocation.Latitude : " ") + " " + " " +
                    (request.ToLocation.Longitude != null ? request.ToLocation.Longitude : " ") + " " +
                    (request.ToLocation.Town != null ? request.ToLocation.Town : " ") + " " +
                    (request.ToLocation.Street != null ? request.ToLocation.Street : " ") + " " +
                    (request.ToLocation.StreetNumber != null ? request.ToLocation.StreetNumber.Value.ToString() : " ");

            }

            if (request.ToUser != null)
            {
                viewModel.ToUser =
                  (request.ToUser.FirstName != null) ?
                  (request.ToUser.FirstName + " " + request.ToUser.SecondName + " " + request.ToUser.LastName) :
                  (request.ToUser.Email);

            }

            if (request.OldLocationCode != null)
            {
                var oldLocation = this.locationService.GetById(request.OldLocationCode);
                viewModel.FromLocation =
                    (oldLocation.Country != null ? oldLocation.Country : " ") + " " +
                    (oldLocation.Latitude != null ? oldLocation.Latitude : " ") + " " + " " +
                    (oldLocation.Longitude != null ? oldLocation.Longitude : " ") + " " +
                    (oldLocation.Town != null ? oldLocation.Town : " ") + " " +
                    (oldLocation.Street != null ? oldLocation.Street : " ") + " " +
                    (oldLocation.StreetNumber != null ? oldLocation.StreetNumber.Value.ToString() : " ");

            }

            if (request.OldUserId != null)
            {
                var oldUser = this.userService.GetById(request.OldUserId);
                viewModel.FromUser =
                  (oldUser.FirstName != null) ?
                  (oldUser.FirstName + " " + oldUser.SecondName + " " + oldUser.LastName) :
                  (oldUser.Email);

            }
            return View(viewModel);
        }

        //GET: View all history requests
        [HttpGet]
        [RightCheck(Right = "Approve request for relocation")]
        public ActionResult AllHistoryRequests()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var requests = this.requestForRelocationService.GetAll()
                .Where(x => x.IsFinished);

            if (!this.IsMegaAdmin())
            {
                requests = requests.Where(x => x.From.Site.OrganisationId == userOrg);
            }

            var viewModel = requests.ToList().ConvertAll(
                 x => new RequestForRelocationViewModel
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
        [RightCheck(Right = "Approve request for relocation")]
        public ActionResult ViewAllHistoryRequest(int id)
        {
            var request = this.requestForRelocationService.GetById(id);

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

            var viewModel = new RelocationHistoryRequest
            {
                Brand = asset.Brand,
                Producer = asset.Producer,
                InventoryNumber = asset.InventoryNumber,
                ItemModel = asset.Model,
                Price = asset.Price.Value,
                Guarantee = asset.Guarantee,
                Type = asset.Type.ToString(),
                SiteName = request.OldSiteName,
                FromId = user.Id,
                FromName = (user.FirstName != null && user.SecondName != null && user.LastName != null) ?
                        user.FirstName + " " + user.SecondName + " " + user.LastName : user.Email,
                Id = id,
                IsApproved = request.IsApproved,
                ToSite = request.ToSite.Name,
                PackingSlipId = request.PackingSlip != null ? request.PackingSlipId.Value : 0,
                Currency = request.Asset.Price.Currency.Code
            };

            if (request.ToLocation != null)
            {
                viewModel.ToLocation =
                    (request.ToLocation.Country != null ? request.ToLocation.Country : " ") + " " +
                    (request.ToLocation.Latitude != null ? request.ToLocation.Latitude : " ") + " " + " " +
                    (request.ToLocation.Longitude != null ? request.ToLocation.Longitude : " ") + " " +
                    (request.ToLocation.Town != null ? request.ToLocation.Town : " ") + " " +
                    (request.ToLocation.Street != null ? request.ToLocation.Street : " ") + " " +
                    (request.ToLocation.StreetNumber != null ? request.ToLocation.StreetNumber.Value.ToString() : " ");

            }

            if (request.ToUser != null)
            {
                viewModel.ToUser =
                  (request.ToUser.FirstName != null) ?
                  (request.ToUser.FirstName + " " + request.ToUser.SecondName + " " + request.ToUser.LastName) :
                  (request.ToUser.Email);

            }

            if (request.OldLocationCode != null)
            {
                var oldLocation = this.locationService.GetById(request.OldLocationCode);
                viewModel.FromLocation =
                    (oldLocation.Country != null ? oldLocation.Country : " ") + " " +
                    (oldLocation.Latitude != null ? oldLocation.Latitude : " ") + " " + " " +
                    (oldLocation.Longitude != null ? oldLocation.Longitude : " ") + " " +
                    (oldLocation.Town != null ? oldLocation.Town : " ") + " " +
                    (oldLocation.Street != null ? oldLocation.Street : " ") + " " +
                    (oldLocation.StreetNumber != null ? oldLocation.StreetNumber.Value.ToString() : " ");

            }

            if (request.OldUserId != null)
            {
                var oldUser = this.userService.GetById(request.OldUserId);
                viewModel.FromUser =
                  (oldUser.FirstName != null) ?
                  (oldUser.FirstName + " " + oldUser.SecondName + " " + oldUser.LastName) :
                  (oldUser.Email);

            }
            return View("ViewHistoryRequest", viewModel);
        }
    }
}