using KAssets.Areas.Reports.Models;
using KAssets.Controllers;
using KAssets.Filters;
using KAssets.Models;
using System;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KAssets.Areas.Items.Models;
using KAssets.Areas.Invoices.Models;

namespace KAssets.Areas.Reports.Controllers
{
    [Authorize]
    [HasSite]
    [RightCheck(Right = "Report for invoices by date")]
    public class InvoicesByDateReportController : BaseController
    {
        // GET: Create report
        [HttpGet]
        public ActionResult Create()
        {
            var viewModel = new CreateInvoicesByDateReportViewModel
            {
                Paid = true,
                Approved = true,
                Finished = true
            };

            return View(viewModel);
        }

        //POST: Get result
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Result(CreateInvoicesByDateReportViewModel model)
        {
            if (model.DateOfApproving == null && model.DateOfCreation == null && model.DateOfPayment == null&&model.PaymentPeriod==null)
            {
                this.ModelState.AddModelError("", @KAssets.Resources.Translation.ReportsArea.Reports.MustChooseAtLeastOneDate);
                return View("Create", model);
            }

            if (!model.DateOfApproving.HasValue)
            {
                model.DateOfApproving = DateTime.MinValue;
            }
            if (!model.DateOfCreation.HasValue)
            {
                model.DateOfCreation = DateTime.MinValue;
            }
            if (!model.DateOfPayment.HasValue)
            {
                model.DateOfPayment = DateTime.MinValue;
            }
            if (!model.PaymentPeriod.HasValue)
            {
                model.PaymentPeriod = DateTime.MinValue;
            }


            var invoices = this.invoiceService.GetAll()
                    .Where(x => x.IsPaid == model.Paid)
                    .Where(x => x.IsApproved == model.Approved)
                    .Where(x => x.Finished == model.Finished);

            if (!this.IsMegaAdmin())
            {
                var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
                invoices = invoices.Where(x => x.CompiledUser.Site.OrganisationId == userOrg);
            }


            if (model.OrOrAnd)
            {
                invoices = invoices.Where(x =>
                    (
                       (model.DateOfApproving.HasValue ? (x.DateOfApproving.ToShortDateString() == model.DateOfApproving.Value.ToShortDateString()) : (false))
                       ||
                       (model.DateOfCreation.HasValue ? (x.DateOfCreation.ToShortDateString() == model.DateOfCreation.Value.ToShortDateString()) : (false))
                       ||
                       (model.PaymentPeriod.HasValue ? (x.PaymentPeriod.ToShortDateString() == model.PaymentPeriod.Value.ToShortDateString()) : (false))
                       ||
                       (model.DateOfPayment.HasValue ? (x.DateOfPayment.ToShortDateString() == model.DateOfPayment.Value.ToShortDateString()) : (false))
                    )
                   );
            }
            else
            {
                invoices = invoices.Where(x =>
                    (
                     (model.DateOfApproving.HasValue ? (x.DateOfApproving.ToShortDateString() == model.DateOfApproving.Value.ToShortDateString()) : (true))
                      &&
                      (model.DateOfCreation.HasValue ? (x.DateOfCreation.ToShortDateString() == model.DateOfCreation.Value.ToShortDateString()) : (true))
                      &&
                      (model.PaymentPeriod.HasValue ? (x.PaymentPeriod.ToShortDateString() == model.PaymentPeriod.Value.ToShortDateString()) : (true))
                      &&
                      (model.DateOfPayment.HasValue ? (x.DateOfPayment.ToShortDateString() == model.DateOfPayment.Value.ToShortDateString()) : (true))
                    )
                  );
            }

            var viewModel = invoices.ToList()
              .ConvertAll(x =>
              new InvoiceListViewModel
              {
                  Id = x.Id,
                  InvoiceNumber = x.Number,
                  RecipientMOL = x.RecipientMOL
              });

            return View(viewModel);
        }

        //GET: View certain result
        [HttpGet]
        public ActionResult ViewCertainResult(int id)
        {
            var invoice = this.invoiceService.GetById(id);

            if (!this.IsMegaAdmin())
            {
                //Verify if asset is from user organisation
                if (invoice.CompiledUser.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var viewModel = new InvoiceFullViewModel
            {
                CompiledUser = (invoice.CompiledUser.FirstName != null && invoice.CompiledUser.SecondName != null && invoice.CompiledUser.LastName != null) ?
                      invoice.CompiledUser.FirstName + " " + invoice.CompiledUser.SecondName + " " + invoice.CompiledUser.LastName : invoice.CompiledUser.Email,
                DateOfIssue = invoice.DateOfCreation == DateTime.MinValue ? " - " : invoice.DateOfCreation.ToString(),
                Id = invoice.Id,
                Items = invoice.Items.ToList()
                        .ConvertAll(x =>
                        new ItemViewModel
                        {
                            Brand = x.Brand,
                            Model = x.Model,
                            Quantity = x.Quantity,
                            Price = x.Price.Value,
                            Currency = x.Price.Currency.Code
                        }),
                Number = invoice.Number,
                PaymentPeriod = invoice.PaymentPeriod == DateTime.MinValue ? " - " : invoice.PaymentPeriod.ToShortDateString(),
                // PerformerFirmName = invoice.PerformerFirmName,
                // PerformerMOL = invoice.PerformerMOL,
                Price = invoice.Price.Value.FormattedTo2() + " " + invoice.Price.Currency.Code,
                ProviderName = invoice.Provider.Name,
                ProviderEmail = invoice.Provider.Email,
                ProviderAddress = invoice.Provider.Address,
                ProviderBulstat = invoice.Provider.Bulstat,
                BillToAddress = invoice.CompiledUser.Site.Organisation.Address,
                BillToOrganisation = invoice.CompiledUser.Site.Organisation.Name,
                BillToSite = invoice.CompiledUser.Site.Name,
                RecipientMOL = invoice.RecipientMOL,
                DateOfApproving = invoice.DateOfApproving == DateTime.MinValue ? " - " : invoice.DateOfApproving.ToString(),
                DayOfPayment = invoice.DateOfPayment == DateTime.MinValue ? " - " : invoice.DateOfPayment.ToString()
            };
            if (invoice.RequestToProvider != null)
            {
                viewModel.PODateOfSend = invoice.RequestToProvider.DateOfSend == DateTime.MinValue ? " - " : invoice.RequestToProvider.DateOfSend.ToString();
                viewModel.POFrom = (invoice.RequestToProvider.From.FirstName != null && invoice.RequestToProvider.From.SecondName != null && invoice.RequestToProvider.From.LastName != null) ?
                     invoice.RequestToProvider.From.FirstName + " " + invoice.RequestToProvider.From.SecondName + " " + invoice.RequestToProvider.From.LastName : invoice.RequestToProvider.From.Email;
                viewModel.POProvider = invoice.RequestToProvider.Provider.Name;
            }

            return View(viewModel);
        }
    }
}