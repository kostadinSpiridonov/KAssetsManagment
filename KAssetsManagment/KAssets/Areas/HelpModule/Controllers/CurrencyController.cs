using KAssets.Filters;
using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KAssets.Resources.Translation.CurrencyTr;
using KAssets.Areas.Admin.Models;
using KAssets.Controllers;
using KAssets.Areas.HelpModule.Models;

namespace KAssets.Areas.HelpModule.Controllers
{
    [Authorize]
    [HasSite]
    public class CurrencyController : BaseController
    {
        // GET: Get all currencies
        [HttpGet]
        [RightCheck(Right = "Manage currency")]
        public ActionResult Index()
        {
            var currencies = this.currencyService.GetAll();
            if (!this.IsMegaAdmin())
            {
                var userORg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
                currencies = currencies.Where(x => x.OrganisationId == userORg).ToList();
            }

            var viewModel = currencies.ToList()
                .ConvertAll(x =>
                 new CurrencyViewModel
                 {
                     Code = x.Code,
                     Id = x.Id,
                     Description = x.Description,
                     Organisation = x.Organisation.Name
                 });

            return View(viewModel);
        }

        //GET: Add a new currency
        [HttpGet]
        [RightCheck(Right = "Manage currency")]
        public ActionResult Add()
        {
            var viewModel = new AddCurrencyViewModel();

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

        //POST: Add a new currency
        [HttpPost]
        [RightCheck(Right = "Manage currency")]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddCurrencyViewModel model)
        {
            if (!ModelState.IsValid)
            {
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

            if (this.IsMegaAdmin())
            {
                userORg = model.SeletedOrganisationId;
            }

            //Check is there a currency with the same code
            if (this.currencyService.Exist(model.Code, userORg))
            {
                this.ModelState.AddModelError("Code", CurrencyTr.SameCodeError);
                
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


            this.currencyService.Add(new Currency
                {
                    Code = model.Code,
                    Description = model.Description,
                    OrganisationId = model.SeletedOrganisationId
                });

            return Redirect("/HelpModule/Currency/Index");
        }

        //GET: Edit a currency
        [HttpGet]
        [RightCheck(Right = "Manage currency")]
        public ActionResult Edit(int id)
        {
            var currency = this.currencyService.GetById(id);

            //Verify if asset is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (currency.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var viewModel = new EditCurrencyViewModel
            {
                Code = currency.Code,
                Description = currency.Description,
                Id = currency.Id
            };

            return View(viewModel);
        }

        //POST: Edit a currency
        [HttpPost]
        [RightCheck(Right = "Manage currency")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditCurrencyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            if (this.IsMegaAdmin())
            {
                userOrg = this.currencyService.GetById(model.Id).OrganisationId;
            }

            //Check is there a currency with the same code
                if (this.currencyService.ExistUpdate(model.Code, model.Id, userOrg))
                {
                    this.ModelState.AddModelError("Code", CurrencyTr.SameCodeError);

                    return View(model);
                }

            this.currencyService.Update(new Currency
            {
                Description = model.Description,
                Code = model.Code,
                Id = model.Id
            });

            return Redirect("/HelpModule/Currency/Index");
        }

        //GET: Get the base currency of user' organisation
        [HttpGet]
        public JsonResult GetBaseCurrency()
        {
            //return user' organisation' currency or the first currency from database
            var user = this.userService.GetById(this.User.Identity.GetUserId());
            var baseCurrency = new Currency();

            if (user.Site != null)
            {
                var organisation = this.userService.GetById(this.User.Identity.GetUserId()).Site.Organisation;

                if (organisation.Bill != null)
                {
                    baseCurrency = organisation.Bill.Currency;
                }
                else
                {
                    baseCurrency = this.currencyService.GetAll().First();
                }
            }
            else
            {
                baseCurrency = this.currencyService.GetAll().First();
            }

            return Json(new { Notation = baseCurrency.Code }, JsonRequestBehavior.AllowGet);
        }

        //GET: Get the course between two currency
        [HttpGet]
        public JsonResult GetCourseBetween(string first, int second)
        {
            first = first.Trim();
            var rate = this.exchangeService.GetRateNotation(second, first);

            return Json(rate, JsonRequestBehavior.AllowGet);
        }
    }
}
