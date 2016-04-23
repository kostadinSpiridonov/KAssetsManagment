using KAssets.Filters;
using KAssets.Models;
using KAssets.Resources.Translation.BillTr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KAssets.Resources.Translation.OrganisationTr;
using KAssets.Resources.Translation.CurrencyTr;
using KAssets.Resources.Translation;
using KAssets.Areas.HelpModule.Models;

namespace KAssets.Controllers
{
    [Authorize]
    public class BillController : BaseController
    {
        // GET: Get bill by id
        [HttpGet]
        public ActionResult GetBill(int id)
        {
            var bill = this.billService.GetBill(id);
            var viewModel = new BillViewModel
            {
                IBAN = bill.IBAN,
                Id = bill.Id,
                OrganisationId = id,
                Currency = bill.Currency.Code
            };

            return View(viewModel);
        }

        //GET: Add a new bill
        [RightCheck(Right = "Low admin")]
        [HttpGet]
        public ActionResult Add(int id)
        {
            var viewModel = new AddBillViewModel
            {
                OrganisationId = id
            };
            viewModel.Currency = this.currencyService.GetAll()
                .Where(x => x.OrganisationId == id).ToList()
               .ConvertAll(x =>
               new CurrencyViewModel
               {
                   Code = x.Code,
                   Id = x.Id
               }).ToList();


            return View(viewModel);
        }

        //POST: Add a new bill
        [RightCheck(Right = "Low admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddBillViewModel model)
        {
            if (!ModelState.IsValid||(model.SelectedCurrency==0))
            {
                this.ModelState.AddModelError("", Common.Error);
                model.Currency = this.currencyService.GetAll()
                    .Where(x => x.OrganisationId == model.Id).ToList()
                   .ConvertAll(x =>
                   new CurrencyViewModel
                   {
                       Code = x.Code,
                       Id = x.Id
                   }).ToList();


                return View(model);
            }
            //Validate iban
            if (!StaticFunctions.ValidateBankAccount(model.IBAN))
            {
                model.Currency = this.currencyService.GetAll()
                    .Where(x => x.OrganisationId == model.Id).ToList()
                   .ConvertAll(x =>
                   new CurrencyViewModel
                   {
                       Code = x.Code,
                       Id = x.Id
                   }).ToList();

                this.ModelState.AddModelError("", BillTr.IBANIsNotCorrect);
                return View(model);
            }



            //Add bill
            var billId = this.billService.Add(new Bill
                {
                    IBAN = model.IBAN,
                    CurrencyId = model.SelectedCurrency
                });

            this.billService.AddBillToOrganisation(billId, model.OrganisationId);

            return Redirect("/Admin/Organisation/Details/" + model.OrganisationId);
        }

        //GET: Edit a bill
        [RightCheck(Right = "Low admin")]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var bill = this.billService.GetBill(id);
            var viewModel = new EditBillViewModel
            {
                OrganisationId = id,
                SelectedCurrency = bill.CurrencyId,
                BillId = bill.Id,
                IBAN = bill.IBAN
            };
            viewModel.Currency = this.currencyService.GetAll()
                .Where(x => x.OrganisationId == id).ToList()
               .ConvertAll(x =>
               new CurrencyViewModel
               {
                   Code = x.Code,
                   Id = x.Id
               }).ToList();


            return View(viewModel);
        }

        //POST: Edit a bill
        [RightCheck(Right = "Low admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditBillViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Currency = this.currencyService.GetAll()
                    .Where(x => x.OrganisationId == model.OrganisationId).ToList()
                   .ConvertAll(x =>
                   new CurrencyViewModel
                   {
                       Code = x.Code,
                       Id = x.Id
                   }).ToList();

                return View(model);
            }

            //Validate iban
            if (!StaticFunctions.ValidateBankAccount(model.IBAN))
            {
                model.Currency = this.currencyService.GetAll()
                    .Where(x => x.OrganisationId == model.OrganisationId).ToList()
                   .ConvertAll(x =>
                   new CurrencyViewModel
                   {
                       Code = x.Code,
                       Id = x.Id
                   }).ToList();

                this.ModelState.AddModelError("", BillTr.IBANIsNotCorrect);
                return View(model);
            }

            //Update bill data
            this.billService.Update(new Bill
            {
                IBAN = model.IBAN,
                Id = model.BillId,
                CurrencyId = model.SelectedCurrency
            });

            return Redirect("/Admin/Organisation/Details/" + model.OrganisationId);
        }

        //Verify whether a organisation has a bill
        public bool HasOrganisationBill(int id)
        {
            return this.billService.HasOrganisationBill(id);
        }
    }
}