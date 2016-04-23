using KAssets.Filters;
using KAssets.Models;
using KAssets.Resources.Translation.ProviderTr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KAssets.Resources.Translation;
using KAssets.Areas.Admin.Models;
using KAssets.Controllers;
using KAssets.Areas.HelpModule.Models;
using KAssets.Resources.Translation.BillTr;

namespace KAssets.Areas.HelpModule.Controllers
{
    [Authorize]
    [HasSite]
    [RightCheck(Right = "Manage providers")]
    public class ProviderController : BaseController
    {
        // GET: Get all providers
        [HttpGet]
        public ActionResult GetAll()
        {
            var userORg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var providers = this.providerService.GetAll()
                .Where(x => x.Status == "Active");

            if (!this.IsMegaAdmin())
            {
                providers = providers.Where(x => x.OrganisationId == userORg);
            }

            var viewModel = providers.ToList().ConvertAll(
                x => new ProviderViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.Email,
                    Organisation = x.Organisation.Name
                });

            return View(viewModel);
        }

        //GET: Get all deleted providers
        [HttpGet]
        public ActionResult GetAllDeleted()
        {
            var userORg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var providers = this.providerService.GetAll()
                .Where(x => x.Status == "Deleted");

            if (!this.IsMegaAdmin())
            {
                providers = providers.Where(x => x.OrganisationId == userORg);
            }

            var viewModel = providers.ToList().ConvertAll(
                x => new ProviderViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.Email,
                    Organisation = x.Organisation.Name
                });

            return View(viewModel);
        }

        //GET: Get provider details
        [HttpGet]
        public ActionResult Details(int id)
        {
            var provider = this.providerService.GetById(id);

            //Verify if asset is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (provider.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var viewModel = new ProviderDetailsViewModel
            {
                Id = provider.Id,
                Email = provider.Email,
                Name = provider.Name,
                Phone = provider.Phone,
                Status = provider.Status,
                Bulstat = provider.Bulstat,
                Address = provider.Address
            };
            viewModel.Bill = new BillViewModel
            {
                IBAN = provider.Bill.IBAN,
                Id = provider.Bill.Id,
                Currency = provider.Bill.Currency.Code
            };

            return View(viewModel);
        }

        //GET: Add a provider
        [HttpGet]
        public ActionResult Add()
        {
            var viewModel = new AddProviderViewModel();
            viewModel.Bill = new AddBillProviderViewModel();


            if (!this.IsMegaAdmin())
            {
                viewModel.SeletedOrganisationId = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
                viewModel.Bill.Currency = this.currencyService.GetAll()
                .Where(x => x.OrganisationId == this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                .ToList()
                .ConvertAll(x =>
                new CurrencyViewModel
                {
                    Code = x.Code,
                    Id = x.Id,
                    //IsBase = x.IsBase
                })
                .ToList();
            }
            else
            {
                viewModel.Bill.Currency = new List<CurrencyViewModel>();

                viewModel.Organisations = this.organisationService.GetAll().ToList()
                    .ConvertAll(x => new OrganisationViewModel
                    {
                        Id = x.Id,
                        Name = x.Name
                    });
            }

            return View(viewModel);
        }

        //POST: Add a provider
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddProviderViewModel model)
        {
            if(model.Bill.IBAN!=null)
            {
                if (!StaticFunctions.ValidateBankAccount(model.Bill.IBAN))
                {
                    this.ModelState.AddModelError("", BillTr.IBANIsNotCorrect);
                }
            }

            if (!ModelState.IsValid || model.SeletedOrganisationId == 0 || model.Bill.SelectedCurrency == 0 )
            {
                if (model.SeletedOrganisationId == 0 || model.Bill.SelectedCurrency == 0)
                {
                    this.ModelState.AddModelError("", Common.Error);
                }
                if (!this.IsMegaAdmin())
                {
                    model.SeletedOrganisationId = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
                    model.Bill.Currency = this.currencyService.GetAll()
                       .Where(x => x.OrganisationId == this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                       .ToList()
                       .ConvertAll(x =>
                       new CurrencyViewModel
                       {
                           Code = x.Code,
                           Id = x.Id,
                           //IsBase = x.IsBase
                       })
                       .ToList();
                }
                else
                {
                    model.Organisations = this.organisationService.GetAll().ToList()
                        .ConvertAll(x => new OrganisationViewModel
                        {
                            Id = x.Id,
                            Name = x.Name
                        });

                    model.Bill.Currency = this.currencyService.GetAll()
                      .Where(x => x.OrganisationId == model.SeletedOrganisationId)
                      .ToList()
                      .ConvertAll(x =>
                      new CurrencyViewModel
                      {
                          Code = x.Code,
                          Id = x.Id,
                          //IsBase = x.IsBase
                      })
                      .ToList();
                }

                return View(model);
            }

            var userORg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            if (this.IsMegaAdmin())
            {
                userORg = model.SeletedOrganisationId;
            }
            //Check is there another provider with the same email
            if (this.providerService.Exist(model.Email, userORg))
            {
                this.ModelState.AddModelError("", ProviderTr.ExistSameEmail);

                if (!this.IsMegaAdmin())
                {
                    model.SeletedOrganisationId = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
                    model.Bill.Currency = this.currencyService.GetAll()
                       .Where(x => x.OrganisationId == this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                       .ToList()
                       .ConvertAll(x =>
                       new CurrencyViewModel
                       {
                           Code = x.Code,
                           Id = x.Id,
                           //IsBase = x.IsBase
                       })
                       .ToList();
                }
                else
                {
                    model.Organisations = this.organisationService.GetAll().ToList()
                        .ConvertAll(x => new OrganisationViewModel
                        {
                            Id = x.Id,
                            Name = x.Name
                        });

                    model.Bill.Currency = this.currencyService.GetAll()
                      .Where(x => x.OrganisationId == model.SeletedOrganisationId)
                      .ToList()
                      .ConvertAll(x =>
                      new CurrencyViewModel
                      {
                          Code = x.Code,
                          Id = x.Id,
                          //IsBase = x.IsBase
                      })
                      .ToList();
                }
                return View(model);
            }

            var billId = this.billService.Add(new Bill
            {
                IBAN = model.Bill.IBAN,
                CurrencyId = model.Bill.SelectedCurrency
            });

            var provider = new Provider
            {
                Status = "Active",
                Phone = model.Phone,
                Name = model.Name,
                Email = model.Email,
                BillId = billId,
                Bulstat = model.Bulstat,
                Address = model.Address,
                OrganisationId = model.SeletedOrganisationId
            };

            this.providerService.Add(provider);

            return Redirect("/HelpModule/Provider/GetAll");
        }

        //POST:Delete provider
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var provider = this.providerService.GetById(id);

            //Verify if asset is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (provider.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            this.providerService.Delete(id);

            return Redirect("/HelpModule/Provider/GetAll");
        }

        //GET: Edit a provider
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var provider = this.providerService.GetById(id);

            //Verify if asset is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (provider.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }


            var viewModel = new EditProviderViewModel
            {
                Email = provider.Email,
                Id = provider.Id,
                Name = provider.Name,
                Phone = provider.Phone,
                Status = provider.Status,
                Bulstat = provider.Bulstat,
                Address = provider.Address
            };

            viewModel.Bill = new AddBillProviderViewModel();
            viewModel.Bill.SelectedCurrency = provider.Bill.CurrencyId;
            viewModel.Bill.Id = provider.Bill.Id;
            viewModel.Bill.IBAN = provider.Bill.IBAN;


            if (!this.IsMegaAdmin())
            {
                viewModel.SeletedOrganisationId = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
                viewModel.Bill.Currency = this.currencyService.GetAll()
                .Where(x => x.OrganisationId == this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                .ToList()
                .ConvertAll(x =>
                new CurrencyViewModel
                {
                    Code = x.Code,
                    Id = x.Id
                })
                .ToList();
            }
            else
            {
                viewModel.Bill.Currency = this.currencyService.GetAll()
                .Where(x => x.OrganisationId == provider.OrganisationId)
                .ToList()
                .ConvertAll(x =>
                new CurrencyViewModel
                {
                    Code = x.Code,
                    Id = x.Id
                })
                .ToList();
                viewModel.SeletedOrganisationId = provider.OrganisationId;
            }

            return View(viewModel);
        }

        //POST:Edit a provider
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditProviderViewModel model)
        {
            if (model.Bill.IBAN != null)
            {
                if (!StaticFunctions.ValidateBankAccount(model.Bill.IBAN))
                {
                    this.ModelState.AddModelError("", BillTr.IBANIsNotCorrect);
                }
            }


            if (!ModelState.IsValid || model.SeletedOrganisationId == 0 || model.Bill.SelectedCurrency == 0 )
            {
              
                if (model.SeletedOrganisationId == 0 || model.Bill.SelectedCurrency == 0)
                {
                    this.ModelState.AddModelError("", Common.Error);
                }
                if (!this.IsMegaAdmin())
                {
                    model.SeletedOrganisationId = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
                    model.Bill.Currency = this.currencyService.GetAll()
                    .Where(x => x.OrganisationId == this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                    .ToList()
                    .ConvertAll(x =>
                    new CurrencyViewModel
                    {
                        Code = x.Code,
                        Id = x.Id
                    })
                    .ToList();
                }
                else
                {
                    model.SeletedOrganisationId = model.SeletedOrganisationId;

                    model.Bill.Currency = this.currencyService.GetAll()
                      .Where(x => x.OrganisationId == model.SeletedOrganisationId)
                      .ToList()
                      .ConvertAll(x =>
                      new CurrencyViewModel
                      {
                          Code = x.Code,
                          Id = x.Id
                      })
                      .ToList();
                }

                return View(model);
            }

            var userORg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            if (this.IsMegaAdmin())
            {
                userORg = model.SeletedOrganisationId;
            }
            //Check is there another provider with the same email
            if (this.providerService.ExistUpdate(model.Email, model.Id, userORg))
            {
                this.ModelState.AddModelError("", ProviderTr.ExistSameEmail);

                model.Bill.Currency = this.currencyService.GetAll().ToList()
                 .ConvertAll(x =>
                 new CurrencyViewModel
                 {
                     Code = x.Code,
                     Id = x.Id,
                     // IsBase = x.IsBase
                 })
                .ToList();

                return View(model);
            }

            var provider = new Provider
            {
                Id = model.Id,
                Status = model.Status,
                Phone = model.Phone,
                Name = model.Name,
                Email = model.Email,
                Bulstat = model.Bulstat,
                Address = model.Address
            };

            this.providerService.Update(provider);

            this.billService.Update(new Bill
            {
                IBAN = model.Bill.IBAN,
                Id = model.Bill.Id,
                CurrencyId = model.Bill.SelectedCurrency
            });
            return Redirect("/HelpModule/Provider/GetAll");
        }
    }
}