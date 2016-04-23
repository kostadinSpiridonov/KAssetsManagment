using KAssets.Filters;
using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KAssets.Resources.Translation.ItemTr;
using KAssets.Resources.Translation;
using KAssets.Areas.Admin.Models;
using KAssets.Areas.HelpModule.Models;
using KAssets.Controllers;
using KAssets.Areas.Items.Models;

namespace KAssets.Areas.Items.Controllers
{
    [Authorize]
    [HasSite]
    [RightCheck(Right = "Manage items")]
    public class ItemController : BaseController
    {
        [HttpGet]
        //GET: Get all items
        public ActionResult GetAll()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());

            var items = this.itemService.GetAll().Where(x => x.Status == "Active");

            if (!this.IsMegaAdmin())
            {
                items = items.Where(x => x.OrganisationId == userOrg);
            }

            var itemsViewModel = items.ToList().ConvertAll(
                x => new ItemViewModel
                {
                    Id = x.Id,
                    Brand = x.Brand,
                    Model = x.Model,
                    Quantity = x.Quantity,
                    OrganisationName = x.Organisation.Name
                }).OrderBy(x => x.IsInYourOrganisation).Reverse();


            return View(itemsViewModel);
        }

        [HttpGet]
        //GET: Get all deleted items
        public ActionResult GetAllDeleted()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var items = this.itemService.GetAll().Where(x => x.Status == "Deleted");

            if (!this.IsMegaAdmin())
            {
                items = items.Where(x => x.OrganisationId == userOrg);
            }

            var itemsViewModel = items.ToList().ConvertAll(
                x => new ItemViewModel
                {
                    Id = x.Id,
                    Brand = x.Brand,
                    Model = x.Model,
                    Quantity = x.Quantity,
                    OrganisationName = x.Organisation.Name
                }).OrderBy(x => x.IsInYourOrganisation).Reverse();


            return View(itemsViewModel);
        }

        //GET: Add item to organiation
        [HttpGet]
        public ActionResult Add()
        {
            var viewModel = new AddItemViewModel
            {
                Price = 1,
                Quantity = 1,
            };


            var user = this.userService.GetById(this.User.Identity.GetUserId());
            if (user.Site != null)
            {
                if (!this.IsMegaAdmin())
                {

                    viewModel.Organisations = new List<OrganisationViewModel>();
                    viewModel.Organisations.Add(new OrganisationViewModel
                        {
                            Id = user.Site.Organisation.Id,
                            Name = user.Site.Organisation.Name
                        });

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
                    viewModel.Organisations = this.organisationService.GetAll().ToList()
                   .ConvertAll(x => new OrganisationViewModel
                   {
                       Id = x.Id,
                       Name = x.Name
                   });

                    viewModel.Currency = new List<CurrencyViewModel>();
                }
            }
            else
            {
                viewModel.Organisations = new List<OrganisationViewModel>();

                viewModel.Currency = this.currencyService.GetAll().ToList()
                .ConvertAll(x =>
                new CurrencyViewModel
                {
                    Code = x.Code,
                    Id = x.Id
                }).ToList();
            }

            return View(viewModel);
        }

        //POST: Add item to organisation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddItemViewModel model)
        {
            //Check is there a item with the same id
            if (this.itemService.Exist(model.Id) || (!ModelState.IsValid)||(model.SelectedCurrency==0))
            {
                if(model.SelectedCurrency==0)
                {
                    this.ModelState.AddModelError("", Common.Error);
                }

                if (this.itemService.Exist(model.Id))
                {
                    this.ModelState.AddModelError("", ItemTr.Exist);
                }

                var user = this.userService.GetById(this.User.Identity.GetUserId());
                if (user.Site != null)
                {
                    if (!this.IsMegaAdmin())
                    {

                        model.Organisations = new List<OrganisationViewModel>();
                        model.Organisations.Add(new OrganisationViewModel
                        {
                            Id = user.Site.Organisation.Id,
                            Name = user.Site.Organisation.Name
                        });

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
                        model.Organisations = this.organisationService.GetAll().ToList()
                       .ConvertAll(x => new OrganisationViewModel
                       {
                           Id = x.Id,
                           Name = x.Name
                       });

                        model.Currency = this.currencyService.GetAll()
                            .Where(x=>x.OrganisationId==model.SeletedOrganisationId).ToList()
                       .ConvertAll(x =>
                       new CurrencyViewModel
                       {
                           Code = x.Code,
                           Id = x.Id
                       }).ToList();
                    }
                }
                else
                {
                    model.Organisations = this.organisationService.GetAll().ToList()
                    .ConvertAll(x => new OrganisationViewModel
                    {
                        Id = x.Id,
                        Name = x.Name
                    });

                    model.Currency = this.currencyService.GetAll()
                            .Where(x => x.OrganisationId == model.SeletedOrganisationId).ToList()
                    .ConvertAll(x =>
                    new CurrencyViewModel
                    {
                        Code = x.Code,
                        Id = x.Id
                    }).ToList();
                }

                return View(model);
            }


            var item = this.itemService.Add(new Item
            {
                Id = model.Id,
                Brand = model.Brand,
                DateOfManufacture = model.DateOfManufacture,
                Model = model.ItemModel,
                Price = new Price { Value = model.Price, CurrencyId = model.SelectedCurrency },
                Producer = model.Producer,
                Quantity = model.Quantity,
                RotatingItem = model.RotatingItem,
                OrganisationId = model.SeletedOrganisationId,
                Status = "Active"
            });

            return Redirect("/Items/Item/GetAll");
        }

        //GET: Edit a item
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var item = this.itemService.GetById(id);

            //Verify if asset is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (item.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var viewModel = new EditItemViewModel
            {
                Brand = item.Brand,
                DateOfManufacture = item.DateOfManufacture,
                Id = item.Id,
                ItemModel = item.Model,
                Price = item.Price.Value,
                Producer = item.Producer,
                Quantity = item.Quantity,
                RotatingItem = item.RotatingItem,
                SeletedOrganisationId = item.OrganisationId
            };
            if (!this.IsMegaAdmin())
            {
                var user = this.userService.GetById(this.User.Identity.GetUserId());

                viewModel.Organisations = new List<OrganisationViewModel>();
                viewModel.Organisations.Add(new OrganisationViewModel
                {
                    Id = user.Site.Organisation.Id,
                    Name = user.Site.Organisation.Name
                });

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
                viewModel.Organisations = this.organisationService.GetAll().ToList()
                       .ConvertAll(x => new OrganisationViewModel
                       {
                           Id = x.Id,
                           Name = x.Name
                       });


                viewModel.Currency = this.currencyService.GetAll()
                    .Where(x => x.OrganisationId == viewModel.SeletedOrganisationId).ToList()
               .ConvertAll(x =>
               new CurrencyViewModel
               {
                   Code = x.Code,
                   Id = x.Id
               }).ToList();
            }

            return View(viewModel);
        }

        //POST: Edit a item
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditItemViewModel model)
        {
            if (!ModelState.IsValid||model.SelectedCurrency==0)
            {
                if(model.SelectedCurrency==0)
                {
                    this.ModelState.AddModelError("", Common.Error);
                }
                var user = this.userService.GetById(this.User.Identity.GetUserId());
                if (user.Site != null)
                {
                    if (!this.IsMegaAdmin())
                    {

                        model.Organisations = new List<OrganisationViewModel>();
                        model.Organisations.Add(new OrganisationViewModel
                        {
                            Id = user.Site.Organisation.Id,
                            Name = user.Site.Organisation.Name
                        });

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
                        model.Organisations = this.organisationService.GetAll().ToList()
                       .ConvertAll(x => new OrganisationViewModel
                       {
                           Id = x.Id,
                           Name = x.Name
                       });


                        model.Currency = this.currencyService.GetAll()
                            .Where(x => x.OrganisationId == model.SeletedOrganisationId).ToList()
                       .ConvertAll(x =>
                       new CurrencyViewModel
                       {
                           Code = x.Code,
                           Id = x.Id
                       }).ToList();
                    }
                }
                else
                {
                    model.Organisations = this.organisationService.GetAll().ToList()
                    .ConvertAll(x => new OrganisationViewModel
                    {
                        Id = x.Id,
                        Name = x.Name
                    });


                    model.Currency = this.currencyService.GetAll()
                        .Where(x => x.OrganisationId == model.SeletedOrganisationId).ToList()
                   .ConvertAll(x =>
                   new CurrencyViewModel
                   {
                       Code = x.Code,
                       Id = x.Id
                   }).ToList();
                }

                return View(model);
            }

            this.itemService.Update(new Item
            {
                Brand = model.Brand,
                DateOfManufacture = model.DateOfManufacture,
                Id = model.Id,
                Model = model.ItemModel,
                Price = new Price { Value = model.Price, CurrencyId = model.SelectedCurrency },
                Producer = model.Producer,
                Quantity = model.Quantity,
                RotatingItem = model.RotatingItem,
                OrganisationId = model.SeletedOrganisationId
            });

            return Redirect("/Items/Item/Details/" + model.Id);
        }

        //GET:Get the details for a item
        [HttpGet]
        public ActionResult Details(ItemDetailsViewModel model)
        {
            var item = this.itemService.GetById(model.Id);

            //Verify if asset is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (item.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var viewModel = new ItemDetailsViewModel
            {
                Brand = item.Brand,
                DateOfManufacture = item.DateOfManufacture.Date,
                Id = item.Id,
                ItemModel = item.Model,
                Price = item.Price.Value,
                Producer = item.Producer,
                Quantity = item.Quantity,
                RotatingItem = item.RotatingItem,
                OrganisationName = item.Organisation.Name,
                Status = item.Status,
                Currency = item.Price.Currency.Code

            };

            return View(viewModel);
        }

        //POST:Delete a item
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var item = this.itemService.GetById(id);
            //Verify if asset is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (item.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var requests = this.requestToAcquireItemService.GetAll().Where(x => !x.Finished)
                .Where(x => x.Items.Select(y => y.Id).Contains(id));

            //Check is item used somewhere
            if (requests.Count() > 0)
            {
                TempData["DelError"] = ItemTr.DeleteError;
                return Redirect("/Items/Item/GetAll");
            }
            var orgId = this.itemService.GetById(id).OrganisationId;
            this.itemService.Delete(id);

            return Redirect("/Items/Item/GetAll");
        }


        //GET: Get organisation currencies
        [HttpGet]
        [RightCheck(Right = "Manage assets")]
        public JsonResult OrganisationCurrencies(int id)
        {
            var currencies = this.currencyService.GetAll()
                .Where(x => x.OrganisationId == id)
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