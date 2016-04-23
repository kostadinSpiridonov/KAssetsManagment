using KAssets.Filters;
using KAssets.Models;
using KAssets.Resources.Translation.ExchangeRateTr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KAssets.Areas.Admin.Models;
using KAssets.Controllers;
using KAssets.Areas.HelpModule.Models;

namespace KAssets.Areas.HelpModule.Controllers
{
    [RightCheck(Right = "Manage exchange rates")]
    [Authorize]
    [HasSite]
    public class ExchangeRateController : BaseController
    {
        // GET: Get all exchange rates
        [HttpGet]
        public ActionResult Index()
        {
            var rates = this.exchangeService.GetAll();

            if (!this.IsMegaAdmin())
            {
                var userORg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
                rates = rates.Where(x => x.OrganisationId == userORg).ToList();
            }

            var viewModel = rates.ToList().
                ConvertAll(x =>
                new ExchangeRateViewModel
                {
                    From = x.From.Code,
                    To = x.To.Code,
                    Rate = x.ExchangeRateValue,
                    Id = x.Id,
                    Organisation = x.Organisation.Name
                });

            return View(viewModel);
        }

        //GET: Add a new exchange rate
        [HttpGet]
        public ActionResult Add()
        {
            var viewModel = new AddExchangeRateViewModel();

            if (!this.IsMegaAdmin())
            {
                viewModel.SeletedOrganisationId = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            }
            else
            {
                viewModel.Organisations = this.organisationService.GetAll().ToList()
                    .ConvertAll(x => new OrganisationViewModel
                    {
                        Id = x.Id,
                        Name = x.Name
                    });
            }
            return View(viewModel);
        }

        //POST: Add a new exchange rate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddExchangeRateViewModel model)
        {
            if ((!ModelState.IsValid) || model.To == 0 || model.From == 0)
            {
                if (model.To == 0)
                {
                    this.ModelState.AddModelError("To", ExchangeRateTr.ToCurrencyIsRequired);
                }

                if (model.From == 0)
                {
                    this.ModelState.AddModelError("From", ExchangeRateTr.FromCurrencyIsRequired);
                }

                if (!this.IsMegaAdmin())
                {
                    model.SeletedOrganisationId = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
                }
                else
                {
                    model.Organisations = this.organisationService.GetAll().ToList()
                        .ConvertAll(x => new OrganisationViewModel
                        {
                            Id = x.Id,
                            Name = x.Name
                        });
                }
                return View(model);
            }

            var userORg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            if(this.IsMegaAdmin())
            {
                userORg = model.SeletedOrganisationId;
            }
            //Check is there a exchange rate for the currenciess
            if (this.exchangeService.Exist(model.From, model.To, userORg))
            {
                this.ModelState.AddModelError("", ExchangeRateTr.Exist);
                if (!this.IsMegaAdmin())
                {
                    model.SeletedOrganisationId = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
                }
                else
                {
                    model.Organisations = this.organisationService.GetAll().ToList()
                        .ConvertAll(x => new OrganisationViewModel
                        {
                            Id = x.Id,
                            Name = x.Name
                        });
                }
                return View(model);
            }

            //Add a new exchange rate
            this.exchangeService.Add(new ExchangeRate
                {
                    ExchangeRateValue = model.Rate,
                    FromId = model.From,
                    ToId = model.To,
                    OrganisationId = model.SeletedOrganisationId
                });

            return Redirect("/HelpModule/ExchangeRate/Index");
        }

        //GET: Get all currencies
        [HttpGet]
        public PartialViewResult ChooseCurrency(int? id)
        {
            var currencies = this.currencyService.GetAll();
            if (id.HasValue)
            {
                currencies = currencies.Where(x => x.OrganisationId == id).ToList();
            }

            if (!this.IsMegaAdmin())
            {
                var userORg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
                currencies = currencies.Where(x => x.OrganisationId == userORg).ToList();
            }

            var viewModel = currencies.ToList().ConvertAll(x =>
                new CurrencyViewModel
                {
                    Code = x.Code,
                    Description = x.Description,
                    Id = x.Id
                });

            if (!id.HasValue)
            {
                if (this.IsMegaAdmin())
                {
                    viewModel = new List<CurrencyViewModel>();
                }
            }
            return PartialView(viewModel);
        }

        //GET: Edit a exchange rate
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var exRate = this.exchangeService.GetById(id);

            //Verify if asset is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (exRate.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var viewModel = new EditExchangeRateViewModel
            {
                Id = exRate.Id,
                Rate = exRate.ExchangeRateValue,
                To = exRate.ToId.Value,
                From = exRate.FromId.Value,
                SeletedOrganisationId=exRate.OrganisationId
            };

            return View(viewModel);
        }

        //POST: Edit a exchange rate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditExchangeRateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var exRate = this.exchangeService.GetById(model.Id);

            var userORg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            //Check if is there a same exchange rate
            if (model.From != exRate.FromId ||
                model.To != exRate.ToId)
            {
                if (this.IsMegaAdmin())
                {
                    userORg = model.SeletedOrganisationId;
                }
                if (this.exchangeService.Exist(model.From, model.To, userORg))
                {
                    this.ModelState.AddModelError("", ExchangeRateTr.Exist);

                    return View(model);
                }
            }

            this.exchangeService.Update(new ExchangeRate
            {
                ExchangeRateValue = model.Rate,
                FromId = model.From,
                ToId = model.To,
                Id = model.Id
            });

            return Redirect("/HelpModule/ExchangeRate/Index");
        }
    }
}