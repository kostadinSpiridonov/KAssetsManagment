using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KAssets.Filters;
using KAssets.Resources.Translation.InvoiceTr;
using KAssets.Areas.Orders.Models;
using KAssets.Areas.HelpModule.Models;
using KAssets.Areas.Items.Models;
using KAssets.Controllers;
using KAssets.Areas.Invoices.Models;

namespace KAssets.Areas.Invoices.Controllers
{
    [Authorize]
    public class InvoiceController : BaseController
    {
        //GET: Add invoice
        [HttpGet]
        [HasSite]
        [RightCheck(Right = "Create invoice")]
        public ActionResult Add()
        {
            var user = this.userService.GetById(this.User.Identity.GetUserId());

            if (user.Site == null)
            {
                return Redirect("/Invoices/Invoice/MemberOfSite");
            }

            //Generate default invoice number
            var number = (this.invoiceService.GetAll().Count + 1001).ToString();
            while (this.invoiceService.Exist(number, user.Site.OrganisationId))
            {
                number = (int.Parse(number) + 1).ToString();
            }

            //Set viewmodel data
            var viewModel = new AddInvoiceViewModel
            {
                InvoiceNumber = number,
                Currencies = this.currencyService.GetAll().ToList()
                .Where(x => x.OrganisationId == user.Site.OrganisationId)
                .ToList()
                .ConvertAll(x => new CurrencyViewModel
                {
                    Code = x.Code,
                    Id = x.Id
                }),
                BillToAddress = user.Site.Organisation.Address,
                BillToOrganisation = user.Site.Organisation.Name,
                BillToSite = user.Site.Name
            };

            //Set default invoiee currency by user organisation
            if (user.Site != null)
            {
                if (user.Site.Organisation.Bill != null)
                {
                    var billCurrencyId = user.Site.Organisation.Bill.CurrencyId;
                    var billRealItem = viewModel.Currencies.Where(x => x.Id == billCurrencyId).FirstOrDefault();

                    viewModel.Currencies.Remove(billRealItem);
                    viewModel.Currencies.Insert(0, billRealItem);
                }
            }
            return View(viewModel);
        }

        //POST: Add invoice
        [HttpPost]
        [HasSite]
        [RightCheck(Right = "Create invoice")]
        public ActionResult Add(AddInvoiceViewModel model)
        {
            var userOrg = this.userService.GetById(this.User.Identity.GetUserId()).Site.OrganisationId;
            //Check if exist a invoice with same invoice number
            if (this.invoiceService.Exist(model.InvoiceNumber, userOrg))
            {
                model.Currencies = this.currencyService.GetAll()
                .Where(x => x.OrganisationId == userOrg)
                .ToList()
                .ConvertAll(x => new CurrencyViewModel
                {
                    Code = x.Code,
                    Id = x.Id
                });

                this.ModelState.AddModelError("", InvoiceTr.SameNum);
                return View(model);
            }

            //Check the is the provider selected and are there selected items
            if (((model.ItemIds == null || model.ItemIds.Count == 0) && (model.ProviderId == 0)) || (!ModelState.IsValid))
            {
                model.Currencies = this.currencyService.GetAll()
                .Where(x => x.OrganisationId == userOrg)
                .ToList()
                .ConvertAll(x => new CurrencyViewModel
                {
                    Code = x.Code,
                    Id = x.Id
                });

                if (model.ProviderId == 0)
                {
                    ModelState.AddModelError("ProviderId", InvoiceTr.ProviderIsRequired);
                }

                if (model.ItemIds == null || model.ItemIds.Count == 0)
                {
                    ModelState.AddModelError("ItemIds", InvoiceTr.MustSelectItems);
                }
                return View(model);
            }

            //-------------------------------------------------------------------------------
            //Convert item to real id and calculate the price of invoice

            double price = 0;
            int flag = 0;
            if (model.ItemIds != null)
            {
                for (int i = 0; i < model.ItemIds.Count; i++)
                {
                    if (model.ItemIds[i].Last() == 'f')
                    {
                        model.ItemIds[i] = model.ItemIds[i].Remove(model.ItemIds[i].Count() - 1, 1);
                    }
                }


                foreach (var item in model.ItemIds)
                {
                    var realItem = this.itemService.GetById(int.Parse(item));

                    var currencyCourse = this.exchangeService.GetRate(realItem.Price.CurrencyId, model.SelectedCurrency);

                    price += realItem.Price.Value * currencyCourse * model.Items[flag];

                    flag++;
                }
            }

            if (model.PoId != null && model.PoId.HasValue)
            {
                var PO = this.requestToProviderService.GetById(model.PoId.Value);

                foreach (var item in PO.GiveItems)
                {
                    var currencyCourse = this.exchangeService.GetRate(item.Price.CurrencyId, model.SelectedCurrency);

                    price += item.Price.Value * currencyCourse * PO.CountItems.Where(x => x.Key == item.Id).FirstOrDefault().Give;

                }
            }

            //-------------------------------------------------------------------------------
            // Add a invoice to database

            var id = this.invoiceService.Add(new Invoice
                {
                    CompiledUserId = this.User.Identity.GetUserId(),
                    DateOfCreation = DateTime.Now,
                    Number = model.InvoiceNumber,
                    ProviderId = model.ProviderId,
                    RecipientMOL = model.RecipientMOL,
                    RequestToProviderId = model.PoId,
                    Price = new Price
                    {
                        CurrencyId = model.SelectedCurrency,
                        Value = price
                    },
                    PaymentPeriod = model.DateOfPayment
                });

            //Add items to invoice
            if (model.ItemIds != null)
            {
                this.invoiceService.AddItems(id, model.ItemIds.ConvertAll(x => int.Parse(x)), model.Items);
            }

            //-------------------------------------------------------------------------------
            //Add event that there is a new invoice

            this.eventService.AddForUserGroup(new Event
                {
                    Content = "You have a new invoice for approving !",
                    Date = DateTime.Now,
                    EventRelocationUrl = "/Invoices/Invoice/InvoicesForApproving"
                },
                "Approve invoice",
                this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()));

            return Redirect("/Home/Index");
        }

        //GET: Get assets
        [HttpGet]
        [HasSite]
        [RightCheck(Right = "Create invoice")]
        public ActionResult ChooseItems()
        {
            var userOrg = this.userService.GetById(this.User.Identity.GetUserId()).Site.OrganisationId;

            var items = this.itemService.GetAll().Where(x => x.Status == "Active");
            if (!this.IsMegaAdmin())
            {
                items = items.Where(x => x.OrganisationId == userOrg);
            }

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

        //GET: Get assets
        [HttpGet]
        [HasSite]
        [RightCheck(Right = "Create invoice")]
        public ActionResult ChooseProvider()
        {
            var providers = this.providerService.GetAll().Where(x => x.Status == "Active");
            var userOrg = this.userService.GetById(this.User.Identity.GetUserId()).Site.OrganisationId;
            if (!this.IsMegaAdmin())
            {
                providers = providers.Where(x => x.OrganisationId == userOrg);
            }

            var viewModel = providers.ToList().ConvertAll(
                x => new ProviderDetailsViewModel
                {
                    Id = x.Id,
                    Email = x.Email,
                    Name = x.Name,
                    Address = x.Address,
                    Bulstat = x.Bulstat
                });

            return PartialView(viewModel);
        }

        //GET: Get assets
        [HttpGet]
        [HasSite]
        [RightCheck(Right = "Create invoice")]
        public ActionResult ChooseProviderOrder()
        {
            var userOrg = this.userService.GetById(this.User.Identity.GetUserId()).Site.OrganisationId;

            var providerOrders = this.requestToProviderService.GetAll().Where(x => x.GiveItems != null && x.GiveItems.Count != 0);
            if (!this.IsMegaAdmin())
            {
                providerOrders = providerOrders.Where(x => x.From.Site.OrganisationId == userOrg);
            }

            var viewModel = providerOrders.ToList().ConvertAll(
                y => new RequestToProviderInvoiceModel
                {
                    DateOfSend = y.DateOfSend.ToString(),
                    FromName = (y.From.FirstName != null) ?
                        y.From.FirstName + " " + y.From.SecondName + " " + y.From.LastName :
                        y.From.Email,
                    ProviderEmail = y.Provider.Email,
                    Offers = y.GiveItems.ToList().ConvertAll(x => new ItemViewModel
                    {
                        Id = x.Id,
                        Brand = x.Brand,
                        Model = x.Model,
                        Price = x.Price.Value,
                        Currency = x.Price.Currency.Code,
                        CurrencyCourse = this.exchangeService.GetRate(x.Price.CurrencyId, this.User.Identity.GetUserId()),
                        Quantity = y.CountItems.Where(m => m.Key == x.Id).LastOrDefault().Give
                    }),
                    Id = y.Id,
                    ProviderId = y.ProviderId
                });

            return PartialView(viewModel);
        }

        //GET:Get invoices for approving
        [HttpGet]
        [HasSite]
        [RightCheck(Right = "Approve invoice")]
        public ActionResult InvoicesForApproving()
        {
            var userOrg = this.userService.GetById(this.User.Identity.GetUserId()).Site.OrganisationId;

            var invoices = this.invoiceService.GetAll()
                .Where(x => !x.IsSeenByApproved);

            if (!this.IsMegaAdmin())
            {
                invoices = invoices.Where(x => x.CompiledUser.Site.OrganisationId == userOrg);
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

        //GET: View invoice for approving
        [HttpGet]
        [HasSite]
        [RightCheck(Right = "Approve invoice")]
        public ActionResult InvoiceForApproving(int id)
        {
            var invoice = this.invoiceService.GetById(id);

            //Verify if asset is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (invoice.CompiledUser.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }


            var viewModel = new InvoiceFullViewModel
            {
                CompiledUser = (invoice.CompiledUser.FirstName != null && invoice.CompiledUser.SecondName != null && invoice.CompiledUser.LastName != null) ?
                      invoice.CompiledUser.FirstName + " " + invoice.CompiledUser.SecondName + " " + invoice.CompiledUser.LastName : invoice.CompiledUser.Email,
                DateOfIssue = invoice.DateOfCreation.ToString(),
                Id = invoice.Id,
                Items = invoice.Items.ToList()
                        .ConvertAll(x =>
                        new ItemViewModel
                        {
                            Brand = x.Brand,
                            Model = x.Model,
                            Quantity = invoice.CounteItems.Where(y => y.Key == x.Id).FirstOrDefault().Want,
                            Price = x.Price.Value,
                            Currency = x.Price.Currency.Code
                        }),
                Number = invoice.Number,
                PaymentPeriod = invoice.PaymentPeriod.ToShortDateString(),

                Price = invoice.Price.Value.FormattedTo2() + " " + invoice.Price.Currency.Code,
                ProviderEmail = invoice.Provider.Email,
                ProviderName = invoice.Provider.Name,
                ProviderAddress = invoice.Provider.Address,
                ProviderBulstat = invoice.Provider.Bulstat,
                BillToAddress = invoice.CompiledUser.Site.Organisation.Address,
                BillToOrganisation = invoice.CompiledUser.Site.Organisation.Name,
                BillToSite = invoice.CompiledUser.Site.Name,
                RecipientMOL = invoice.RecipientMOL
            };
            if (invoice.RequestToProvider != null)
            {
                viewModel.PODateOfSend = invoice.RequestToProvider.DateOfSend.ToString();
                viewModel.POFrom = (invoice.RequestToProvider.From.FirstName != null && invoice.RequestToProvider.From.SecondName != null && invoice.RequestToProvider.From.LastName != null) ?
                     invoice.RequestToProvider.From.FirstName + " " + invoice.RequestToProvider.From.SecondName + " " + invoice.RequestToProvider.From.LastName : invoice.RequestToProvider.From.Email;
                viewModel.POProvider = invoice.RequestToProvider.Provider.Name;

                var po = this.requestToProviderService.GetById(invoice.RequestToProviderId.Value);
                foreach (var item in po.GiveItems)
                {
                    viewModel.Items.Add(new ItemViewModel
                        {
                            Brand = item.Brand,
                            Model = item.Model,
                            Quantity = po.CountItems.Where(y => y.Key == item.Id).FirstOrDefault().Give,
                            Price = item.Price.Value,
                            Currency = item.Price.Currency.Code
                        });
                }
            }

            return View(viewModel);
        }

        //POST: Approve invoice
        [HttpPost]
        [HasSite]
        [ValidateAntiForgeryToken]
        [RightCheck(Right = "Approve invoice")]
        public ActionResult Approve(int id)
        {
            var invoice = this.invoiceService.GetById(id);

            //Verify if asset is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (invoice.CompiledUser.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            //Set invice is approved
            this.invoiceService.SetApproved(id);

            //Set invoice is seen by approver
            this.invoiceService.SetSeenByApprover(id);

            //Set date of approving
            this.invoiceService.SetDateOfApproving(DateTime.Now, id);

            //Add a event that invoice is approved
            this.eventService.AddForUserGroup(new Event
                {
                    Date = DateTime.Now,
                    Content = "You have a new invice for paying !",
                    EventRelocationUrl = "/Invoices/Invoice/GetInvoicesForPaid"
                },
                "Pay invoice",
                this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()));
            return Redirect("/Invoices/Invoice/InvoicesForApproving");
        }

        //GET: Decline a request
        [HttpGet]
        [RightCheck(Right = "Approve invoice")]
        public ActionResult Decline(int id)
        {
            var invoice = this.invoiceService.GetById(id);

            //Verify if asset is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (invoice.CompiledUser.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
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
        [HasSite]
        [ValidateAntiForgeryToken]
        [RightCheck(Right = "Approve invoice")]
        public ActionResult Decline(DeclineRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Set invoice is seen by approver
            this.invoiceService.SetSeenByApprover(model.RequestId);

            //Set invoice is finished
            this.invoiceService.SetFinished(model.RequestId);


            //Add a event that invoice is not approved
            this.eventService.Add(new Event
                {
                    Content = "Your invoice was not approved !",
                    Date = DateTime.Now,
                    EventRelocationUrl = "/Invoices/Invoice/ViewHistory/" + model.RequestId,
                    UserId = this.invoiceService.GetById(model.RequestId).CompiledUserId
                });

            return Redirect("/Invoices/Invoice/InvoicesForApproving");
        }

        //GET: Get invoices for paid
        [HttpGet]
        [HasSite]
        [RightCheck(Right = "Pay invoice")]
        public ActionResult GetInvoicesForPaid()
        {
            var userOrg = this.userService.GetById(this.User.Identity.GetUserId()).Site.OrganisationId;

            var invoices = this.invoiceService.GetAll()
                .Where(x => (!x.IsPaid) && (x.IsApproved));

            if (!this.IsMegaAdmin())
            {
                invoices = invoices.Where(x => x.CompiledUser.Site.OrganisationId == userOrg);
            }

            var viewModel = invoices.ToList().ConvertAll(x => new InvoiceListViewModel
                {
                    Id = x.Id,
                    InvoiceNumber = x.Number,
                    RecipientMOL = x.RecipientMOL
                });

            return View(viewModel);
        }

        //GET: View invoice for paid
        [HttpGet]
        [HasSite]
        [RightCheck(Right = "Pay invoice")]
        public ActionResult InvoiceForPaid(int id)
        {
            var invoice = this.invoiceService.GetById(id);

            //Verify if asset is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (invoice.CompiledUser.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var viewModel = new InvoiceFullViewModel
            {
                CompiledUser = (invoice.CompiledUser.FirstName != null && invoice.CompiledUser.SecondName != null && invoice.CompiledUser.LastName != null) ?
                      invoice.CompiledUser.FirstName + " " + invoice.CompiledUser.SecondName + " " + invoice.CompiledUser.LastName : invoice.CompiledUser.Email,
                DateOfIssue = invoice.DateOfCreation.ToString(),
                Id = invoice.Id,
                Items = invoice.Items.ToList()
                        .ConvertAll(x =>
                        new ItemViewModel
                        {
                            Brand = x.Brand,
                            Model = x.Model,
                            Quantity = invoice.CounteItems.Where(y => y.Key == x.Id).FirstOrDefault().Want,
                            Price = x.Price.Value,
                            Currency = x.Price.Currency.Code
                        }),
                Number = invoice.Number,
                PaymentPeriod = invoice.PaymentPeriod.ToShortDateString(),
                Price = invoice.Price.Value.FormattedTo2() + " " + invoice.Price.Currency.Code,
                ProviderEmail = invoice.Provider.Email,
                ProviderName = invoice.Provider.Name,
                ProviderAddress = invoice.Provider.Address,
                ProviderBulstat = invoice.Provider.Bulstat,
                BillToAddress = invoice.CompiledUser.Site.Organisation.Address,
                BillToOrganisation = invoice.CompiledUser.Site.Organisation.Name,
                BillToSite = invoice.CompiledUser.Site.Name,
                RecipientMOL = invoice.RecipientMOL,
                DateOfApproving = invoice.DateOfApproving.ToString()
            };
            if (invoice.RequestToProvider != null)
            {
                viewModel.PODateOfSend = invoice.RequestToProvider.DateOfSend.ToString();
                viewModel.POFrom = (invoice.RequestToProvider.From.FirstName != null && invoice.RequestToProvider.From.SecondName != null && invoice.RequestToProvider.From.LastName != null) ?
                     invoice.RequestToProvider.From.FirstName + " " + invoice.RequestToProvider.From.SecondName + " " + invoice.RequestToProvider.From.LastName : invoice.RequestToProvider.From.Email;
                viewModel.POProvider = invoice.RequestToProvider.Provider.Name;

                var po = this.requestToProviderService.GetById(invoice.RequestToProviderId.Value);
                foreach (var item in po.GiveItems)
                {
                    viewModel.Items.Add(new ItemViewModel
                    {
                        Brand = item.Brand,
                        Model = item.Model,
                        Quantity = po.CountItems.Where(y => y.Key == item.Id).FirstOrDefault().Give,
                        Price = item.Price.Value,
                        Currency = item.Price.Currency.Code
                    });
                }
            }
            return View(viewModel);
        }

        //POST: Pay invoice
        [HttpPost]
        [HasSite]
        [ValidateAntiForgeryToken]
        [RightCheck(Right = "Pay invoice")]
        public ActionResult Pay(int id)
        {
            var invoice = this.invoiceService.GetById(id);

            //Verify if asset is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (invoice.CompiledUser.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            //Set invoices is paid, date of payment and is finished
            this.invoiceService.SetPaid(id);
            this.invoiceService.SetDateOfPayment(DateTime.Now, id);
            this.invoiceService.SetFinished(id);

            //Add a event that invoice is paid
            this.eventService.Add(new Event
                {
                    Content = "Your invoice was payed !",
                    Date = DateTime.Now,
                    UserId = this.invoiceService.GetById(id).CompiledUserId,
                    EventRelocationUrl = "/Invoices/Invoice/ViewHistory/" + id
                });

            return Redirect("/Invoices/Invoice/GetInvoicesForPaid");
        }

        //GET: View history of your invoices
        [HttpGet]
        [HasSite]
        [RightCheck(Right = "Create invoice")]
        public ActionResult History()
        {
            var invoices = this.invoiceService.GetAll().Where(x => x.Finished)
                .Where(x => x.CompiledUserId == this.User.Identity.GetUserId());

            var viewModel = invoices.ToList().ConvertAll(x => new InvoiceListViewModel
            {
                Id = x.Id,
                InvoiceNumber = x.Number,
                //PerformerMOL = x.PerformerMOL,
                RecipientMOL = x.RecipientMOL
            });

            return View(viewModel);
        }

        //GET: View all history of invoices
        [HttpGet]
        [HasSite]
        [RightCheck(Right = "Create invoice")]
        public ActionResult HistoryAll()
        {
            var userOrg = this.userService.GetById(this.User.Identity.GetUserId()).Site.OrganisationId;
            var invoices = this.invoiceService.GetAll().Where(x => (x.Finished));

            if (!this.IsMegaAdmin())
            {
                invoices = invoices.Where(x => x.CompiledUser.Site.OrganisationId == userOrg);
            }

            var viewModel = invoices.ToList().ConvertAll(x => new InvoiceListViewModel
            {
                Id = x.Id,
                InvoiceNumber = x.Number,
                // PerformerMOL = x.PerformerMOL,
                RecipientMOL = x.RecipientMOL
            });

            return View(viewModel);
        }

        //GET: View your history invoice
        [HttpGet]
        [HasSite]
        [RightCheck(Right = "Approve invoice")]
        public ActionResult ViewHistory(int id)
        {
            var invoice = this.invoiceService.GetById(id);

            //Verify if asset is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (invoice.CompiledUser.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            if (invoice.CompiledUserId != this.User.Identity.GetUserId())
            {
                return Redirect("/Home/NotAuthorized");
            }

            var viewModel = new InvoiceFullViewModel
            {
                CompiledUser = (invoice.CompiledUser.FirstName != null && invoice.CompiledUser.SecondName != null && invoice.CompiledUser.LastName != null) ?
                      invoice.CompiledUser.FirstName + " " + invoice.CompiledUser.SecondName + " " + invoice.CompiledUser.LastName : invoice.CompiledUser.Email,
                DateOfIssue = invoice.DateOfCreation.ToString(),
                Id = invoice.Id,
                Items = invoice.Items.ToList()
                        .ConvertAll(x =>
                        new ItemViewModel
                        {
                            Brand = x.Brand,
                            Model = x.Model,
                            Quantity = invoice.CounteItems.Where(y => y.Key == x.Id).FirstOrDefault().Want,
                            Price = x.Price.Value,
                            Currency = x.Price.Currency.Code
                        }),
                Number = invoice.Number,
                PaymentPeriod = invoice.PaymentPeriod.ToShortDateString(),
                Price = invoice.Price.Value.FormattedTo2() + " " + invoice.Price.Currency.Code,
                ProviderName = invoice.Provider.Name,
                ProviderEmail = invoice.Provider.Email,
                ProviderAddress = invoice.Provider.Address,
                ProviderBulstat = invoice.Provider.Bulstat,
                BillToAddress = invoice.CompiledUser.Site.Organisation.Address,
                BillToOrganisation = invoice.CompiledUser.Site.Organisation.Name,
                BillToSite = invoice.CompiledUser.Site.Name,
                RecipientMOL = invoice.RecipientMOL,
                DateOfApproving = invoice.DateOfApproving.ToString(),
                DayOfPayment = invoice.DateOfPayment.ToString()
            };

            if (invoice.RequestToProvider != null)
            {
                viewModel.PODateOfSend = invoice.RequestToProvider.DateOfSend.ToString();
                viewModel.POFrom = (invoice.RequestToProvider.From.FirstName != null && invoice.RequestToProvider.From.SecondName != null && invoice.RequestToProvider.From.LastName != null) ?
                     invoice.RequestToProvider.From.FirstName + " " + invoice.RequestToProvider.From.SecondName + " " + invoice.RequestToProvider.From.LastName : invoice.RequestToProvider.From.Email;
                viewModel.POProvider = invoice.RequestToProvider.Provider.Name;

                var po = this.requestToProviderService.GetById(invoice.RequestToProviderId.Value);
                foreach (var item in po.GiveItems)
                {
                    viewModel.Items.Add(new ItemViewModel
                    {
                        Brand = item.Brand,
                        Model = item.Model,
                        Quantity = po.CountItems.Where(y => y.Key == item.Id).FirstOrDefault().Give,
                        Price = item.Price.Value,
                        Currency = item.Price.Currency.Code
                    });
                }
            }
            return View(viewModel);
        }

        //GET: View all history invoice
        [HttpGet]
        [HasSite]
        [RightCheck(Right = "Approve invoice")]
        public ActionResult ViewHistoryAll(int id)
        {
            var invoice = this.invoiceService.GetById(id);

            //Verify if asset is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (invoice.CompiledUser.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var viewModel = new InvoiceFullViewModel
            {
                CompiledUser = (invoice.CompiledUser.FirstName != null && invoice.CompiledUser.SecondName != null && invoice.CompiledUser.LastName != null) ?
                      invoice.CompiledUser.FirstName + " " + invoice.CompiledUser.SecondName + " " + invoice.CompiledUser.LastName : invoice.CompiledUser.Email,
                DateOfIssue = invoice.DateOfCreation.ToString(),
                Id = invoice.Id,
                Items = invoice.Items.ToList()
                        .ConvertAll(x =>
                        new ItemViewModel
                        {
                            Brand = x.Brand,
                            Model = x.Model,
                            Quantity = invoice.CounteItems.Where(y => y.Key == x.Id).FirstOrDefault().Want,
                            Price = x.Price.Value,
                            Currency = x.Price.Currency.Code
                        }),
                Number = invoice.Number,
                PaymentPeriod = invoice.PaymentPeriod.ToShortDateString(),
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
                DateOfApproving = invoice.DateOfApproving.ToString(),
                DayOfPayment = invoice.DateOfPayment.ToString()
            };
            if (invoice.RequestToProvider != null)
            {
                viewModel.PODateOfSend = invoice.RequestToProvider.DateOfSend.ToString();
                viewModel.POFrom = (invoice.RequestToProvider.From.FirstName != null && invoice.RequestToProvider.From.SecondName != null && invoice.RequestToProvider.From.LastName != null) ?
                     invoice.RequestToProvider.From.FirstName + " " + invoice.RequestToProvider.From.SecondName + " " + invoice.RequestToProvider.From.LastName : invoice.RequestToProvider.From.Email;
                viewModel.POProvider = invoice.RequestToProvider.Provider.Name;

                var po = this.requestToProviderService.GetById(invoice.RequestToProviderId.Value);
                foreach (var item in po.GiveItems)
                {
                    viewModel.Items.Add(new ItemViewModel
                    {
                        Brand = item.Brand,
                        Model = item.Model,
                        Quantity = po.CountItems.Where(y => y.Key == item.Id).FirstOrDefault().Give,
                        Price = item.Price.Value,
                        Currency = item.Price.Currency.Code
                    });
                }
            }
            return View("ViewHistory", viewModel);
        }

        //GET: You must be member of site notification
        [HttpGet]
        public ActionResult MemberOfSite()
        {
            return View();
        }
    }
}
