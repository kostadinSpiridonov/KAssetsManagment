using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KAssets.Models;
using KAssets.Filters;
using System.Linq.Expressions;
using KAssets.Resources.Translation.AssetsTr;
using KAssets.Resources.Translation.OrdersTr.ItemOrderTr;
using KAssets.Controllers;
using KAssets.Areas.AssetsActions.Models;
using KAssets.Areas.Orders.Models;
using KAssets.Areas.Admin.Models;
using KAssets.Areas.HelpModule.Models;
using KAssets.Areas.Items.Models;

namespace KAssets.Areas.AssetsActions.Controllers
{
    [Authorize]
    [HasSite]
    public class AssetController : BaseController
    {
        //GET: Get all assets
        [HttpGet]
        [RightCheck(Right = "Manage assets")]
        public ActionResult GetAll()
        {
            var assets = this.assetService.GetAll();
            var userSite = this.userService.GetById(this.User.Identity.GetUserId()).Site;
            if (!this.IsMegaAdmin())
            {
                assets = assets.Where(x => x.Site.OrganisationId == userSite.OrganisationId).ToList();
            }

            var viewModel = assets.ToList().ConvertAll(x =>
                new AssetViewModel
                {
                    AssetModel = x.Model,
                    Brand = x.Brand,
                    InventoryNumber = x.InventoryNumber,
                    IsInYourSite = this.IsMegaAdmin() ? true : ((userSite.Id == x.SiteId) ? true : false),
                    SiteName = x.Site.Name
                }).OrderBy(x => x.IsInYourSite).Reverse();

            return View(viewModel);
        }

        //GET: Get all sites for a organisation
        [HttpGet]
        public ActionResult ChooseSites()
        {
            var sites = this.siteService.GetAll();

            if (!this.IsMegaAdmin())
            {
                var userOrganisation = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
                sites = sites.Where(x => x.OrganisationId == userOrganisation).ToList();
            }

            var convertedSites = sites.ToList().ConvertAll(
                           x => new SiteViewModel
                           {
                               Id = x.Id,
                               Name = x.Name
                           });

            var viewModel = new ShowSitesViewModel
            {
                Sites = convertedSites
            };

            return PartialView(viewModel);
        }

        //GEt: Details asset
        [HttpGet]
        [RightCheck(Right = "Manage assets")]
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

        //GEt: Details asset
        [HttpGet]
        public ActionResult DetailsPartial(string id)
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

            if (asset.User != null)
            {
                viewModel.UserName = (asset.User.FirstName != null) ?
                    asset.User.FirstName + " " + asset.User.SecondName + " " + asset.User.LastName :
                    asset.User.Email;
            }

            viewModel.SiteName = asset.Site.Name;

            return PartialView(viewModel);
        }

        //GET: Create asset from request for items
        [HttpGet]
        public ActionResult CreateAssetsFromItemOrder(RequestWithSelectedOffersViewModel model)
        {
            var request = this.requestToAcquireItemService.GetById(model.Request.Id);

            //Set items are received in request' packing slip
            this.packingSlipService.SetReceived(request.PackingSlipId.Value, DateTime.Now);

            //Finish request if there aren't items which must be changed to assets
            if (model.GaveItems.Where(x => x.CreateAsset).Count() < 1)
            {
                this.requestToAcquireItemService.SetFinished(model.Request.Id);
                return Redirect("/Home/Index");
            }

            var items = request.Items;

            var viewModel = new List<AddAssetViewModel>();

            //Add gave items to viewmodel
            foreach (var item in model.GaveItems)
            {
                if (item.CreateAsset)
                {
                    var real = items.Where(x => x.Id == item.Id).FirstOrDefault();
                    for (int i = 0; i < request.CountSelectedItems.Where(x => x.Key == real.Id).FirstOrDefault().Give; i++)
                    {
                        viewModel.Add(new AddAssetViewModel
                        {
                            RequestId = model.Request.Id,
                            ItemId = item.Id,
                            Item = new ItemDetailsViewModel
                            {
                                Brand = real.Brand,
                                DateOfManufacture = real.DateOfManufacture,
                                ItemModel = real.Model,
                                Price = real.Price.Value,
                                Producer = real.Producer,
                                Currency = real.Price.Currency.Code,
                            }

                        });

                    }
                }
            }
            return View(viewModel);
        }

        //POST: Create asset from request for items
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateAssetsFromItemOrder(List<AddAssetViewModel> model)
        {
            //Check are there assets with same names
            for (int i = 0; i < model.Count; i++)
            {
                if (this.assetService.Exist(model[i].InventoryNumber))
                {
                    this.ModelState.AddModelError("[" + i + "].InventoryNumber", AssetTr.ExistAssetWithIN);
                    return View(model);
                }
            }

            //Check are there assets from model with same inventory numbers
            for (int i = 0; i < model.Count; i++)
            {
                for (int j = i + 1; j < model.Count; j++)
                {
                    if (model[i].InventoryNumber == model[j].InventoryNumber)
                    {
                        this.ModelState.AddModelError("", ItemOrderTr.TwoAssets);
                        return View(model);
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var request = this.requestToAcquireItemService.GetById(model.First().RequestId);

            //Add a new assets to database
            foreach (var item in model)
            {
                var asset = new Asset
                {
                    Guarantee = item.Guarantee,
                    InventoryNumber = item.InventoryNumber,
                    ItemId = item.ItemId,
                    Type = item.Type,
                    Status = "Active",
                    Price = new Price { Value = item.Item.Price, CurrencyId = this.itemService.GetById(item.ItemId).Price.CurrencyId },
                    Model = item.Item.ItemModel,
                    Producer = item.Item.Producer,
                    Brand = item.Item.Brand
                };

                if (request.LocationId != null)
                {
                    asset.LocationId = request.LocationId;
                }

                if (request.ToUserId != null)
                {
                    asset.UserId = request.ToUserId;

                    asset.SiteId = request.ToUser.SiteId;
                }
                else
                {
                    asset.SiteId = request.From.SiteId;
                }

                this.assetService.Add(asset);

                //Create asset history
                this.assetHistoryService.Add(
                    new AssetHistory
                    {
                        AssetId = asset.InventoryNumber
                    });

                //Set the first row of history
                this.assetHistoryService.AddHistoryRow(new HistoryRow
                {
                    Content = "The asset was acquired.",
                    Date = DateTime.Now
                }, item.InventoryNumber);

                //Set request is finished
                this.requestToAcquireItemService.SetFinished(request.Id);

                //Add event that items were received
                this.eventService.AddForUserGroup(
                    new Event
                    {
                        Content = "The items of request with number " + request.Id + "was received",
                        Date = DateTime.Now
                    }, "Approve order for item",
                this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()));

            }

            return Redirect("/Home/Index/");
        }

        //GET: Edit asset
        [HttpGet]
        [RightCheck(Right = "Manage assets")]
        public ActionResult Edit(string id)
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

            var viewModel = new EditAssetViewModel
            {
                Brand = asset.Brand,
                Guarantee = asset.Guarantee,
                InventoryNumber = asset.InventoryNumber,
                ItemModel = asset.Model,
                Price = asset.Price.Value,
                Producer = asset.Producer,
                Type = asset.Type,
                SelectedCurrency = asset.Price.CurrencyId

            };

            var user = this.userService.GetById(this.User.Identity.GetUserId());
            if (!this.IsMegaAdmin())
            {
                viewModel.Currency = this.currencyService.GetAll()
                    .Where(x => x.OrganisationId == user.Site.OrganisationId).ToList()
                   .ConvertAll(x =>
                   new CurrencyViewModel
                   {
                       Code = x.Code,
                       Id = x.Id
                   }).ToList();
            }
            else
            {
                viewModel.Currency = this.currencyService.GetAll().ToList()
                    .Where(x => x.OrganisationId == asset.Site.OrganisationId).ToList()
               .ConvertAll(x =>
               new CurrencyViewModel
               {
                   Code = x.Code,
                   Id = x.Id
               }).ToList();
            }

            viewModel.SiteName = asset.Site.Name;

            return View(viewModel);
        }

        //POST: Edit asset
        [HttpPost]
        [RightCheck(Right = "Manage assets")]
        public ActionResult Edit(EditAssetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //Add all locations for choosing to viewmodel
                model.Locations = this.locationService.GetAll().ToList().
                    ConvertAll(x => new DropDownLocationViewModel
                    {
                        Code = x.Code,
                        Location = x.Country + ", " + x.Town + ", " + x.Street + ", " + x.StreetNumber.Value
                    });

                var user = this.userService.GetById(this.User.Identity.GetUserId());
                if (!this.IsMegaAdmin())
                {
                    model.Currency = this.currencyService.GetAll()
                        .Where(x => x.OrganisationId == user.Site.OrganisationId).ToList()
                       .ConvertAll(x =>
                       new CurrencyViewModel
                       {
                           Code = x.Code,
                           Id = x.Id
                       }).ToList();
                }
                else
                {
                    var asset = this.assetService.GetById(model.InventoryNumber);
                    model.Currency = this.currencyService.GetAll().ToList()
                    .Where(x => x.OrganisationId == asset.Site.OrganisationId).ToList()
                   .ConvertAll(x =>
                   new CurrencyViewModel
                   {
                       Code = x.Code,
                       Id = x.Id
                   }).ToList();
                }

                return View(model);
            }

            this.assetService.Update(new Asset
                {
                    InventoryNumber = model.InventoryNumber,
                    Producer = model.Producer,
                    Brand = model.Brand,
                    Model = model.ItemModel,
                    Price = new Price
                    {
                        Value = model.Price,
                        CurrencyId = model.SelectedCurrency
                    },
                    Type = model.Type

                });

            //Add a new row to asset' history
            this.assetHistoryService.AddHistoryRow(new HistoryRow
            {
                Content = "The asset information was updated.",
                Date = DateTime.Now
            }, model.InventoryNumber);

            return Redirect("/AssetsActions/Asset/GetAll");
        }

        //GET: Asset is not active
        [HttpGet]
        public ActionResult AssetIsNotActive()
        {
            return View();
        }

        //GET: Add asset
        [HttpGet]
        [RightCheck(Right = "Manage assets")]
        public ActionResult Add()
        {
            var viewModel = new AddAssetFullViewModel
            {
                InventoryNumber = "0",
                Guarantee = 0,
                Price = 1
            };

            var user = this.userService.GetById(this.User.Identity.GetUserId());
            if (!this.IsMegaAdmin())
            {
                viewModel.Currency = this.currencyService.GetAll()
                    .Where(x => x.OrganisationId == user.Site.OrganisationId).ToList()
                   .ConvertAll(x =>
                   new CurrencyViewModel
                   {
                       Code = x.Code,
                       Id = x.Id
                   }).ToList();
            }
            else
            {
                viewModel.Currency = new List<CurrencyViewModel>();
            }

            return View(viewModel);
        }

        //POST: Edit asset
        [HttpPost]
        [RightCheck(Right = "Manage assets")]
        public ActionResult Add(AddAssetFullViewModel model)
        {
            var exist = assetService.Exist(model.InventoryNumber);
            if (!ModelState.IsValid || model.SiteId == 0 || exist)
            {
                if (exist)
                {
                    this.ModelState.AddModelError("", AssetTr.ExistAssetWithIN);
                }

                if (model.SiteId == 0)
                {
                    this.ModelState.AddModelError("", AssetTr.SiteIsRequired);
                }

                //Add all currencies to viewmodel
                var user = this.userService.GetById(this.User.Identity.GetUserId());
                if (!this.IsMegaAdmin())
                {
                    model.Currency = this.currencyService.GetAll()
                        .Where(x => x.OrganisationId == user.Site.OrganisationId).ToList()
                       .ConvertAll(x =>
                       new CurrencyViewModel
                       {
                           Code = x.Code,
                           Id = x.Id
                       }).ToList();
                }
                else
                {
                    var site = this.siteService.GetById(model.SiteId);
                    if (site != null)
                    {
                        model.Currency = this.currencyService.GetAll().ToList()
                        .Where(x => x.OrganisationId == site.OrganisationId).ToList()
                       .ConvertAll(x =>
                       new CurrencyViewModel
                       {
                           Code = x.Code,
                           Id = x.Id
                       }).ToList();
                    }
                    else
                    {
                        model.Currency = new List<CurrencyViewModel>();
                    }
                }
                return View(model);
            }
            var asset = new Asset
                {
                    Brand = model.Brand,
                    Guarantee = model.Guarantee,
                    InventoryNumber = model.InventoryNumber,
                    Model = model.ItemModel,
                    Price = new Price { Value = model.Price, CurrencyId = model.SelectedCurrency },
                    Producer = model.Producer,
                    Status = "Active",
                    Type = model.Type,
                };

            if (model.LocationId != null)
            {
                asset.LocationId = model.LocationId;
            }

            if (model.UserId != null)
            {
                asset.UserId = model.UserId;

                asset.SiteId = this.userService.GetById(model.UserId).SiteId;
            }
            else
            {
                asset.SiteId = model.SiteId;
            }

            this.assetService.Add(asset);

            //Create asset history
            this.assetHistoryService.Add(
                new AssetHistory
                {
                    AssetId = asset.InventoryNumber
                });

            //Add a first history row
            this.assetHistoryService.AddHistoryRow(new HistoryRow
            {
                Content = "The asset was acquired.",
                Date = DateTime.Now
            }, asset.InventoryNumber);

            return Redirect("/AssetsActions/Asset/GetAll");
        }

        //GET: Get organisation currencies
        [HttpGet]
        [RightCheck(Right = "Manage assets")]
        public JsonResult OrganisationCurrencies(int id)
        {
            var organisationId = this.siteService.GetById(id).OrganisationId;
            var currencies = this.currencyService.GetAll()
                .Where(x => x.OrganisationId == organisationId)
                .ToList()
                .ConvertAll(c =>
                new CurrencyViewModel
                {
                    Code = c.Code,
                    Id = c.Id
                }).ToList();

            return Json(currencies, JsonRequestBehavior.AllowGet);
        }




    }
}