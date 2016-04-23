using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KAssets.Filters;
using MoreLinq;
using KAssets.Resources.Translation.OrdersTr.ItemOrderTr;
using KAssets.Controllers;
using KAssets.Areas.Orders.Models;
using KAssets.Areas.HelpModule.Models;
using KAssets.Areas.Items.Models;

namespace KAssets.Areas.Orders.Controllers
{
    [Authorize]
    [HasSite]
    public class ItemOrderController : BaseController
    {
        // GET: Send a request for aqcuisition
        [HttpGet]
        [RightCheck(Right = "Create order for item")]
        public ActionResult SendRequest()
        {
            return View();
        }

        //GET: Get assets for choosing
        [HttpGet]
        public ActionResult ChooseItems()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var items = this.itemService.GetAll().Where(x => x.Status == "Active");
            items = items.Where(x => x.OrganisationId == userOrg);

            var viewModel = items.ToList().ConvertAll(
                x => new ItemViewModel
                {
                    Id = x.Id,
                    Brand = x.Brand,
                    Model = x.Model,
                    OrganisationName = x.Organisation.Name,
                    Price = x.Price.Value,
                    Currency = x.Price.Currency.Code,
                    CurrencyCourse = this.exchangeService.GetRate(x.Price.CurrencyId, this.User.Identity.GetUserId())
                });

            return PartialView(viewModel);
        }

        //GET: Get locations for choosing
        [HttpGet]
        [RightCheck(Right = "Create order for item")]
        public ActionResult ChooseLocations()
        {
            var locations = this.locationService.GetAll();
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            locations = locations.Where(x => x.OrganisationId == userOrg).ToList();

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

        //GET: Get users for choosing
        [HttpGet]
        [RightCheck(Right = "Create order for item")]
        public ActionResult ChooseUser()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var users = this.userService.GetAll()
                .Where(x => x.Status == "Active")
                .Where(x => x.Site != null);

            users = users.Where(x => x.Site.OrganisationId == userOrg);

            var viewModel = users.ToList().ConvertAll(
                x => new ShowUserViewModel
                {
                    Id = x.Id,
                    Name = (x.FirstName + x.SecondName + x.LastName) == "" ?
                        x.Email : x.FirstName + " " + x.SecondName + " " + x.LastName,
                });

            return PartialView(viewModel);
        }

        // POST: Send a request for aqcuisition
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RightCheck(Right = "Create order for item")]
        public ActionResult SendRequest(AddItemAcquisitionRequestViewModel model)
        {
            //Check are there selected items
            if (!ModelState.IsValid || model.Items.Count() == 0)
            {
                if (model != null || model.Items.Count() == 0)
                {
                    this.ModelState.AddModelError("", ItemOrderTr.PleaseSelectItems);
                }
                return View(model);
            }

            //Create request
            var req = new RequestToAcquireItems
            {
                FromId = this.User.Identity.GetUserId(),
                DateOfSend = DateTime.Now
            };

            if (model.SelectedLocation != null)
            {
                req.LocationId = model.SelectedLocation;
            }

            if (model.SelectedUser != null)
            {
                req.ToUserId = model.SelectedUser;
            }

            var requestId = this.requestToAcquireItemService.Add(req);

            //Add items to request
            this.requestToAcquireItemService.AddItemsToRequest(requestId,
                model.Items.Select(x => x.Id).ToList());

            //Add count want items to request
            var dictionary = new Dictionary<int, int>();

            dictionary = model.Items.Select((element, index) => new { element = element.Id, index = element.Count })
             .ToDictionary(ele => ele.element, ele => ele.index);

            this.requestToAcquireItemService.AddWantCountItems(dictionary, requestId);

            //Add event that is created a new item request
            this.eventService.AddForUserGroup(
                new Event
                {
                    Content = "There is a new request for items !",
                    Date = DateTime.Now,
                    EventRelocationUrl = "/Orders/ItemOrder/GetAllRequestsForAproving"
                },
                "Approve order for item",
                this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()));

            return Redirect("/Orders/ItemOrder/SuccessfullySend");
        }

        //GET: Success sent
        [HttpGet]
        [RightCheck(Right = "Create order for item")]
        public ActionResult SuccessfullySend()
        {
            return View();
        }

        //GET: Get all requests for approving
        [HttpGet]
        [RightCheck(Right = "Approve order for item")]
        public ActionResult GetAllRequestsForAproving()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var requests = this.requestToAcquireItemService.GetAll()
                .Where(x => !x.IsApproved)
                .Where(x => !x.AreItemsGave)
                .Where(x => !x.Finished);

            if (!this.IsMegaAdmin())
            {
                requests = requests.Where(x => x.From.Site.OrganisationId == userOrg);
            }

            var viewModel = requests.ToList().ConvertAll(
                x => new ListItemAcquisitionRequestViewModel
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
        [RightCheck(Right = "Approve order for item")]
        public ActionResult RequestForApproving(int id)
        {
            var request = this.requestToAcquireItemService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var viewModel = new RequestWithSelectedOffersViewModel();

            //Set request data to viewmodel
            viewModel.Request = new ViewItemAcquisitionRequestViewModel
            {
                DateOfSend = request.DateOfSend,
                From = new ShortUserDetails
                {
                    FullName = (request.From.FirstName + request.From.SecondName + request.From.LastName) == "" ?
                        request.From.Email : request.From.FirstName + " " + request.From.SecondName + " " + request.From.LastName,
                    Id = request.From.Id
                },
                Id = request.Id,
                Items = request.Items.ToList().ConvertAll(
                x => new ItemViewModel
                {
                    Id = x.Id,
                    Model = x.Model,
                    Brand = x.Brand
                }),
                SiteName = request.ToUser != null ? request.ToUser.Site.Name : request.From.Site.Name,
                ForUser = request.ToUser != null ? (request.ToUser.FirstName != null) ?
                    request.ToUser.FirstName + " " + request.ToUser.SecondName + " " + request.ToUser.LastName :
                    request.ToUser.Email : null
            };

            //Add selected offers to viewmodel
            viewModel.SelectedOffers = request.Items.ToList().ConvertAll(
                 x => new OfferItemViewModel
                 {
                     Brand = x.Brand,
                     Id = x.Id,
                     ItemModel = x.Model,
                     MotherId = x.OrganisationId,
                     MotherName = x.Organisation.Name,
                     Price = x.Price.Value,
                     Producer = x.Producer,
                     Quantity = x.Quantity,
                     IsSelected = false,
                     SelectedCount = request.CountSelectedItems.Where(y => y.Key == x.Id).First().Want,
                     Currency = x.Price.Currency.Code,
                     CurrencyCourse = this.exchangeService.GetRate(x.Price.CurrencyId, this.User.Identity.GetUserId()),
                     IsRotatingItem = x.RotatingItem

                 });

            if (request.Location != null)
            {
                viewModel.Request.LocationName = request.Location.Country + ", " + request.Location.Town
                    + ", " + request.Location.Street + ", " + request.Location.StreetNumber;
            }
            return View(viewModel);
        }

        //GET: Decline a request
        [HttpGet]
        [RightCheck(Right = "Approve order for item")]
        public ActionResult Decline(int id)
        {
            var request = this.requestToAcquireItemService.GetById(id);

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
        [RightCheck(Right = "Approve order for item")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Decline(DeclineRequestViewModel model)
        {
            var request = this.requestToAcquireItemService.GetById(model.RequestId);
            var itemsMessage = "";

            foreach (var item in request.Items)
            {
                itemsMessage += item.Brand + " " + item.Model + ", ";
            }

            //Add event that request is not approved
            var aEvent = new Event
             {
                 UserId = request.FromId,
                 Content = "Your request for " + itemsMessage + " was not approved. " + model.Message,
                 Date = DateTime.Now,
                 EventRelocationUrl = "/Orders/ItemOrder/GetRequestForFinishingOrFinished/" + model.RequestId
             };

            this.eventService.Add(aEvent);

            //Set request is finished and not approved
            this.requestToAcquireItemService.SetFinished(model.RequestId);
            this.requestToAcquireItemService.SetNotApproved(model.RequestId);

            return Redirect("/Orders/ItemOrder/GetAllRequestsForAproving");
        }

        //POST:Approve a request
        [RightCheck(Right = "Approve order for item")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(RequestWithSelectedOffersViewModel model)
        {
            //Add approved items and count of approved items to request
            var elements = model.SelectedOffers.Select(x => new { Key = x.Id, Value = x.SelectedCount }).ToList();
            var dictionary = new Dictionary<int, int>();

            foreach (var item in elements)
            {
                dictionary.Add(item.Key, item.Value);
            }
            this.requestToAcquireItemService.AddApprovedCountItems(dictionary, model.Request.Id);

            //Set request is approved
            this.requestToAcquireItemService.SetApproved(model.Request.Id);

            //Add event that request is approved
            this.eventService.AddForUserGroup(
              new Event
              {
                  Content = "There is new approved request for items !",
                  Date = DateTime.Now,
                  EventRelocationUrl = "/Orders/ItemOrder/ViewApprovedRequests"
              },
              "Give items for item order",
                this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()));

            //Create order packing slip
            var id = this.packingSlipService.Add(new PackingSlip
            {
                ToUserId = model.Request.From.Id
            });

            this.requestToAcquireItemService.AddPackingSlip(model.Request.Id, id);

            return Redirect("/Orders/ItemOrder/GetAllRequestsForAproving");
        }

        //GET: View aproved requests
        [HttpGet]
        [RightCheck(Right = "Give items for item order")]
        public ActionResult ViewApprovedRequests()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var requests = this.requestToAcquireItemService.GetAll()
                .Where(x => x.IsApproved)
                .Where(x => !x.Finished)
                .Where(x => !x.AreItemsGave);

            if (!this.IsMegaAdmin())
            {
                requests = requests.Where(x => x.From.Site.OrganisationId == userOrg);
            }

            var viewModel = requests.ToList().ConvertAll(
                  x => new ListItemAcquisitionRequestViewModel
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
        [RightCheck(Right = "Give items for item order")]
        public ActionResult ViewApprovedRequest(int id)
        {
            var request = this.requestToAcquireItemService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var viewModel = new RequestWithSelectedOffersViewModel();

            //Set request data to viewmodel
            viewModel.Request = new ViewItemAcquisitionRequestViewModel
            {
                DateOfSend = request.DateOfSend,
                From = new ShortUserDetails
                {
                    FullName = (request.From.FirstName + request.From.SecondName + request.From.LastName) == "" ?
                        request.From.Email : request.From.FirstName + " " + request.From.SecondName + " " + request.From.LastName,
                    Id = request.From.Id
                },
                Id = request.Id,
                Items = request.Items.ToList().ConvertAll(
                x => new ItemViewModel
                {
                    Id = x.Id,
                    Model = x.Model,
                    Brand = x.Brand
                }),
                SiteName = request.ToUser != null ? request.ToUser.Site.Name : request.From.Site.Name,
                ForUser = request.ToUser != null ? (request.From.FirstName != null) ?
                    request.From.FirstName + " " + request.From.SecondName + " " + request.From.LastName :
                    request.From.Email : null
            };

            //Set selected offers to viewmodel
            viewModel.SelectedOffers = request.Items.ToList().ConvertAll(
                 x => new OfferItemViewModel
                 {
                     Brand = x.Brand,
                     Id = x.Id,
                     ItemModel = x.Model,
                     MotherId = x.OrganisationId,
                     MotherName = x.Organisation.Name,
                     Price = x.Price.Value,
                     Producer = x.Producer,
                     Quantity = x.Quantity,
                     IsSelected = false,
                     SelectedCount = request.CountSelectedItems.Where(y => y.Key == x.Id).First().Want,
                     Currency = x.Price.Currency.Code,

                 });

            //Set approved offers to viewmodel
            viewModel.ApprovedOffers = request.Items.ToList().ConvertAll(
               x => new OfferItemViewModel
               {
                   Brand = x.Brand,
                   Id = x.Id,
                   ItemModel = x.Model,
                   MotherId = x.OrganisationId,
                   MotherName = x.Organisation.Name,
                   Price = x.Price.Value,
                   Producer = x.Producer,
                   Quantity = x.Quantity,
                   IsSelected = false,
                   SelectedCount = request.CountSelectedItems.Where(y => y.Key == x.Id).First().Approved,
                   Currency = x.Price.Currency.Code,

               });

            //Set give items to viewmodel
            viewModel.GaveItems = request.Items.ToList().ConvertAll(
             x => new OfferItemViewModel
             {
                 Brand = x.Brand,
                 Id = x.Id,
                 ItemModel = x.Model,
                 MotherId = x.OrganisationId,
                 MotherName = x.Organisation.Name,
                 Price = x.Price.Value,
                 Producer = x.Producer,
                 Quantity = x.Quantity,
                 IsSelected = false,
                 SelectedCount = request.CountSelectedItems.Where(y => y.Key == x.Id).First().Approved,
                 Currency = x.Price.Currency.Code,
                 CurrencyCourse = this.exchangeService.GetRate(x.Price.CurrencyId, this.User.Identity.GetUserId())

             });

            if (request.Location != null)
            {
                viewModel.Request.LocationName = request.Location.Country + ", " + request.Location.Town
                    + ", " + request.Location.Street + ", " + request.Location.StreetNumber;
            }
            return View(viewModel);
        }

        //GET: Set items are not given
        [HttpGet]
        [RightCheck(Right = "Give items for item order")]
        public ActionResult NotGave(int id)
        {
            var request = this.requestToAcquireItemService.GetById(id);

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
            return View("Decline", viewModel);
        }

        //POST:Set items are not given
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RightCheck(Right = "Give items for item order")]
        public ActionResult NotGave(DeclineRequestViewModel model)
        {
            var request = this.requestToAcquireItemService.GetById(model.RequestId);
            var itemsMessage = "";

            foreach (var item in request.Items)
            {
                itemsMessage += item.Brand + " " + item.Model + ", ";
            }

            //Add event that items are not gave
            var aEvent = new Event
            {
                UserId = request.FromId,
                Content = "Your request for " + itemsMessage + " was approved. But the items cannot be gave ! " + model.Message,
                Date = DateTime.Now,
                EventRelocationUrl = "/Orders/ItemOrder/GetRequestForFinishingOrFinished/" + model.RequestId
            };

            this.eventService.Add(aEvent);

            //Set from user to packing slip
            this.packingSlipService.SetFromUser(
                  this.requestToAcquireItemService.GetById(model.RequestId).PackingSlipId.Value,
                  this.User.Identity.GetUserId());

            //Set request is finished and items are not given
            this.requestToAcquireItemService.SetFinished(model.RequestId);
            this.requestToAcquireItemService.SetItemsNotGave(model.RequestId);

            return Redirect("/Orders/ItemOrder/ViewApprovedRequests");
        }

        //GET: Give items 
        [HttpPost]
        [RightCheck(Right = "Give items for item order")]
        public ActionResult GiveItems(RequestWithSelectedOffersViewModel model)
        {
            //Check if is there a Count field which value is not permissible
            foreach (var item in model.GaveItems)
            {
                if (item.SelectedCount > model.ApprovedOffers.Where(x => x.Id == item.Id).FirstOrDefault().SelectedCount)
                {
                    this.ModelState.AddModelError("", ItemOrderTr.PermissableError);
                    return View("ViewApprovedRequest", model);
                }
            }

            var request = this.requestToAcquireItemService.GetById(model.Request.Id);

            //Check if all items are given
            bool allGiven = true;
            for (int i = 0; i < model.ApprovedOffers.Count; i++)
            {
                if (model.GaveItems[i].SelectedCount < model.ApprovedOffers[i].SelectedCount)
                {
                    allGiven = false;
                    break;
                }
            }
            //Create a new request for a items which are not given and these items must be given later
            if (!allGiven)
            {
                var newRequest = new RequestToAcquireItems();
                newRequest.DateOfSend = request.DateOfSend;
                newRequest.FromId = request.FromId;
                newRequest.IsApproved = true;
                newRequest.LocationId = request.LocationId;
                newRequest.ToUserId = request.ToUserId;

                var dictionaryWant = new Dictionary<int, int>();

                //Add approved offer to new request
                for (int i = 0; i < model.ApprovedOffers.Count; i++)
                {
                    if (model.ApprovedOffers[i].SelectedCount > model.GaveItems[i].SelectedCount)
                    {
                        dictionaryWant.Add(model.ApprovedOffers[i].Id, model.ApprovedOffers[i].SelectedCount - model.GaveItems[i].SelectedCount);
                    }
                }

                var id = this.requestToAcquireItemService.Add(newRequest);

                this.requestToAcquireItemService.AddItemsToRequest(id,
                dictionaryWant.Select(x => x.Key).ToList());

                //Create new packing slip for new request
                var psId = this.packingSlipService.Add(new PackingSlip
                {
                    ToUserId = request.PackingSlip.ToUserId,
                    FromUserId = request.PackingSlip.FromUserId,

                });

                //Add count of want items
                this.requestToAcquireItemService.AddWantCountItems(dictionaryWant, id);

                //ADd count of approved items
                this.requestToAcquireItemService.AddApprovedCountItems(dictionaryWant, id);

                //Add packing slip
                this.requestToAcquireItemService.AddPackingSlip(id, psId);

                //Add message for new request
                this.requestToAcquireItemService.AddMessage(request.Id, "You will receive other items later !");
            }

            //Set asset are given and their count to request
            this.requestToAcquireItemService.SetAssetsGave(model.Request.Id);

            var elements = model.GaveItems.Select(x => new { Key = x.Id, Value = x.SelectedCount }).ToList();
            var dictionary = new Dictionary<int, int>();
            foreach (var item in elements)
            {
                dictionary.Add(item.Key, item.Value);
            }
            this.requestToAcquireItemService.AddGaveCountItems(dictionary, model.Request.Id);

            foreach (var item in model.GaveItems)
            {
                for (int i = 0; i < item.SelectedCount; i++)
                {
                    this.itemService.ReduceItemQuantity(item.Id);
                }
            }

            //Add a new event that items are given
            var aEvent = new Event
            {
                UserId = model.Request.From.Id,
                Content = "You have a new request for finishing !",
                Date = DateTime.Now,
                EventRelocationUrl = "/Orders/ItemOrder/RequestForFinishing"
            };

            this.eventService.Add(aEvent);

            //Set items are given and set from who is given in packing slip
            this.packingSlipService.SetGiven(
                this.requestToAcquireItemService.GetById(model.Request.Id).PackingSlipId.Value, DateTime.Now);

            this.packingSlipService.SetFromUser(
                  this.requestToAcquireItemService.GetById(model.Request.Id).PackingSlipId.Value,
                  this.User.Identity.GetUserId());

            return Redirect("/Orders/ItemOrder/ViewApprovedRequests");
        }

        //GET: Get requests for finishing
        [HttpGet]
        [RightCheck(Right = "Create order for item")]
        public ActionResult RequestForFinishing()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var requests = this.requestToAcquireItemService.GetAll()
                .Where(x => x.IsApproved)
                .Where(x => !x.Finished)
                .Where(x => x.AreItemsGave);

            if (!this.IsMegaAdmin())
            {
                requests = requests.Where(x => x.FromId == this.User.Identity.GetUserId())
                .Where(x => x.From.Site.OrganisationId == userOrg);
            }

            var viewModel = requests.ToList().ConvertAll(
                x => new ListItemAcquisitionRequestViewModel
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
        public ActionResult GetRequestForFinishingOrFinished(int id)
        {
            var request = this.requestToAcquireItemService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            if (request.Finished)
            {
                if (request.FromId != this.User.Identity.GetUserId())
                {
                    return Redirect("/Home/NotAuthorized");
                }

            }

            var viewModel = new RequestWithSelectedOffersViewModel();

            //Set request data to viewmodel
            viewModel.Request = new ViewItemAcquisitionRequestViewModel
            {
                DateOfSend = request.DateOfSend,
                From = new ShortUserDetails
                {
                    FullName = (request.From.FirstName + request.From.SecondName + request.From.LastName) == "" ?
                        request.From.Email : request.From.FirstName + " " + request.From.SecondName + " " + request.From.LastName,
                    Id = request.From.Id
                },
                Id = request.Id,
                Items = request.Items.ToList().ConvertAll(
                x => new ItemViewModel
                {
                    Id = x.Id,
                    Model = x.Model,
                    Brand = x.Brand
                }),
                IsFinished = request.Finished,
                SiteName = request.ToUser != null ? request.ToUser.Site.Name : request.From.Site.Name,
                ForUser = request.ToUser != null ? (request.ToUser.FirstName != null) ?
                    request.ToUser.FirstName + " " + request.ToUser.SecondName + " " + request.ToUser.LastName :
                    request.ToUser.Email : null,
                PackingSlipId = request.PackingSlip != null ? request.PackingSlipId.Value : 0,

            };

            //Set approved offer to viewmodel
            viewModel.ApprovedOffers = request.Items.ToList().ConvertAll(
                 x => new OfferItemViewModel
                 {
                     Brand = x.Brand,
                     Id = x.Id,
                     ItemModel = x.Model,
                     MotherId = x.OrganisationId,
                     MotherName = x.Organisation.Name,
                     Price = x.Price.Value,
                     Producer = x.Producer,
                     IsSelected = false,
                     IsRotatingItem = x.RotatingItem,
                     SelectedCount = request.CountSelectedItems.Where(y => y.Key == x.Id).First().Approved,
                     Currency = x.Price.Currency.Code
                 });

            //Set selected offers to viewmodel
            viewModel.SelectedOffers = request.Items.ToList().ConvertAll(
                 x => new OfferItemViewModel
                 {
                     Brand = x.Brand,
                     Id = x.Id,
                     ItemModel = x.Model,
                     MotherId = x.OrganisationId,
                     MotherName = x.Organisation.Name,
                     Price = x.Price.Value,
                     Producer = x.Producer,
                     IsRotatingItem = x.RotatingItem,
                     IsSelected = false,
                     SelectedCount = request.CountSelectedItems.Where(y => y.Key == x.Id).First().Want,
                     Currency = x.Price.Currency.Code

                 });

            //Set givven items to viewmodel
            viewModel.GaveItems = request.Items.ToList().ConvertAll(
                x => new OfferItemViewModel
                {
                    Brand = x.Brand,
                    Id = x.Id,
                    ItemModel = x.Model,
                    MotherId = x.OrganisationId,
                    MotherName = x.Organisation.Name,
                    Price = x.Price.Value,
                    Producer = x.Producer,
                    IsRotatingItem = x.RotatingItem,
                    IsSelected = false,
                    SelectedCount = request.CountSelectedItems.Where(y => y.Key == x.Id).First().Give,
                    Currency = x.Price.Currency.Code,
                    CurrencyCourse = this.exchangeService.GetRate(x.Price.CurrencyId, this.User.Identity.GetUserId())

                });

            viewModel.IsApproved = request.IsApproved;
            viewModel.AreItemGave = request.AreItemsGave;
            if (request.Location != null)
            {
                viewModel.Request.LocationName = request.Location.Country + ", " + request.Location.Town
                    + ", " + request.Location.Street + ", " + request.Location.StreetNumber;
            }

            viewModel.Message = request.Message != null ? ItemOrderTr.RequestSpecialMessage : "";
            return View(viewModel);
        }

        //GET: Get yout history
        [HttpGet]
        [RightCheck(Right = "Create order for item")]
        public ActionResult HistoryRequests()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var requests = this.requestToAcquireItemService.GetAll()
                .Where(x => x.FromId == this.User.Identity.GetUserId())
                .Where(x => x.Finished)
                .Where(x => x.From.Site.OrganisationId == userOrg);

            var viewModel = requests.ToList().ConvertAll(
                x => new ListItemAcquisitionRequestViewModel
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
        [RightCheck(Right = "Approve order for item")]
        public ActionResult HistoryRequestAll(int id)
        {
            var request = this.requestToAcquireItemService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }


            var viewModel = new RequestWithSelectedOffersViewModel();

            viewModel.Request = new ViewItemAcquisitionRequestViewModel
            {
                DateOfSend = request.DateOfSend,
                From = new ShortUserDetails
                {
                    FullName = (request.From.FirstName + request.From.SecondName + request.From.LastName) == "" ?
                        request.From.Email : request.From.FirstName + " " + request.From.SecondName + " " + request.From.LastName,
                    Id = request.From.Id
                },
                Id = request.Id,
                Items = request.Items.ToList().ConvertAll(
                x => new ItemViewModel
                {
                    Id = x.Id,
                    Model = x.Model,
                    Brand = x.Brand
                }),
                IsFinished = request.Finished,
                SiteName = request.ToUser != null ? request.ToUser.Site.Name : request.From.Site.Name,
                ForUser = request.ToUser != null ? (request.From.FirstName != null) ?
                    request.From.FirstName + " " + request.From.SecondName + " " + request.From.LastName :
                    request.From.Email : null,
                PackingSlipId = request.PackingSlip != null ? request.PackingSlipId.Value : 0

            };

            viewModel.ApprovedOffers = request.Items.ToList().ConvertAll(
                 x => new OfferItemViewModel
                 {
                     Brand = x.Brand,
                     Id = x.Id,
                     ItemModel = x.Model,
                     MotherId = x.OrganisationId,
                     MotherName = x.Organisation.Name,
                     Price = x.Price.Value,
                     Producer = x.Producer,
                     IsSelected = false,
                     IsRotatingItem = x.RotatingItem,
                     Currency = x.Price.Currency.Code,
                     SelectedCount = request.CountSelectedItems.Where(y => y.Key == x.Id).First().Approved
                 });

            viewModel.SelectedOffers = request.Items.ToList().ConvertAll(
                 x => new OfferItemViewModel
                 {
                     Brand = x.Brand,
                     Id = x.Id,
                     ItemModel = x.Model,
                     MotherId = x.OrganisationId,
                     MotherName = x.Organisation.Name,
                     Price = x.Price.Value,
                     Producer = x.Producer,
                     IsRotatingItem = x.RotatingItem,
                     IsSelected = false,
                     Currency = x.Price.Currency.Code,
                     SelectedCount = request.CountSelectedItems.Where(y => y.Key == x.Id).First().Want

                 });

            viewModel.GaveItems = request.Items.ToList().ConvertAll(
                x => new OfferItemViewModel
                {
                    Brand = x.Brand,
                    Id = x.Id,
                    ItemModel = x.Model,
                    MotherId = x.OrganisationId,
                    MotherName = x.Organisation.Name,
                    Price = x.Price.Value,
                    Producer = x.Producer,
                    IsRotatingItem = x.RotatingItem,
                    IsSelected = false,
                    Currency = x.Price.Currency.Code,
                    SelectedCount = request.CountSelectedItems.Where(y => y.Key == x.Id).First().Give,
                    CurrencyCourse = this.exchangeService.GetRate(x.Price.CurrencyId, this.User.Identity.GetUserId())

                });

            viewModel.IsApproved = request.IsApproved;
            viewModel.AreItemGave = request.AreItemsGave;
            if (request.Location != null)
            {
                viewModel.Request.LocationName = request.Location.Country + ", " + request.Location.Town
                    + ", " + request.Location.Street + ", " + request.Location.StreetNumber;
            }
            return View("GetRequestForFinishingOrFinished", viewModel);
        }

        //GET: Get all history
        [HttpGet]
        [RightCheck(Right = "Approve order for item")]
        public ActionResult HistoryRequestsAll()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var requests = this.requestToAcquireItemService.GetAll()
                .Where(x => x.Finished);

            if (!this.IsMegaAdmin())
            {
                requests = requests.Where(x => x.From.Site.OrganisationId == userOrg);
            }

            var viewModel = requests.ToList().ConvertAll(
                x => new ListItemAcquisitionRequestViewModel
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

        public class ComparerItem
        {
            public string Brand { get; set; }

            public string Model { get; set; }

            public int Id { get; set; }
        }
    }
}