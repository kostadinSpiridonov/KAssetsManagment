using KAssets.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ImapX;
using ImapX.Authentication;
using System.Security.Authentication;
using ImapX.Collections;
using Newtonsoft.Json;
using KAssets.Filters;
using KAssets.Resources.Translation.OrdersTr.ProviderOrder;
using KAssets.Resources.Translation.ItemTr;
using KAssets.Controllers;
using KAssets.Areas.Orders.Models;
using KAssets.Areas.Admin.Models;
using KAssets.Areas.HelpModule.Models;
using KAssets.Areas.Items.Models;

namespace KAssets.Areas.Orders.Controllers
{
    [Authorize]
    [HasSite]
    public class ProviderOrderController : BaseController
    {
        //GET: Create request 
        [HttpGet]
        [RightCheck(Right = "Send request to provider")]
        public ActionResult CreateRequest()
        {
            return View();
        }

        //GET: Choose providers
        [HttpGet]
        [RightCheck(Right = "Send request to provider")]
        public ActionResult ChooseProviders()
        {
            var users = this.providerService.GetAll().Where(x => x.Status == "Active");

            if (!this.IsMegaAdmin())
            {
                var userSite = this.userService.GetById(this.User.Identity.GetUserId()).Site;
                users = users.Where(x => x.OrganisationId == userSite.OrganisationId);
            }

            var viewModel = users.ToList().ConvertAll(
                x => new ProviderDetailsViewModel
                {
                    Id = x.Id,
                    Email = x.Email,
                    Name = x.Name,
                    Phone = x.Phone
                });

            return PartialView(viewModel);
        }

        //POST: Create request
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RightCheck(Right = "Send request to provider")]
        public ActionResult CreateRequest(CreateProviderRequestViewModel model)
        {
            //Check are there selected items
            if (model.ItemsAndCount == null || model.ItemsAndCount.Count == 0)
            {
                this.ModelState.AddModelError("ItemsAndCount", ProviderOrderTr.PleaseSelectitems);
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var organisation = this.organisationService.GetById(
                this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()));

            var credentialUserName = organisation.EmailClient;
            var sentFrom = organisation.EmailClient;
            var pwd = KAssets.Controllers.StaticFunctions.RSAALg.Decryption(organisation.EmailClientPassword);

            // Configure the client:
            System.Net.Mail.SmtpClient client =
                new System.Net.Mail.SmtpClient("smtp-mail.outlook.com");

            client.Port = 587;
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            // Creatte the credentials:
            System.Net.NetworkCredential credentials =
                new System.Net.NetworkCredential(credentialUserName, pwd);

            client.EnableSsl = true;
            client.Credentials = credentials;

            //Add a request to database
            var id = this.requestToProviderService.Add(
                new RequestToProvider
                {
                    DateOfSend = DateTime.Now,
                    FromId = this.User.Identity.GetUserId(),
                    ProviderId = model.Provider,
                    SentSubject = model.Subject,
                    SentBody = model.Content
                });

            //Add want items and their count
            this.requestToProviderService.AddWantItems(id, model.ItemsAndCount.Select(x => x.Id).ToList());
            this.requestToProviderService.AddCountWantItems(id, model.ItemsAndCount.ToDictionary(x => x.Id, y => y.Count));

            // Create the message:
            var to = this.providerService.GetById(model.Provider).Email;
            var mail =
                new System.Net.Mail.MailMessage(sentFrom, to);

            mail.Body += "Order id: ";
            mail.Body += id;
            mail.Body += "\r\n";
            mail.Body += "\r\n";
            mail.Subject = model.Subject;
            mail.Body += model.Content;
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            if (model.ItemsAndCount != null)
            {
                foreach (var item in model.ItemsAndCount)
                {
                    var itemFull = this.itemService.GetById(item.Id);
                    mail.Body += "\r\n";
                    mail.Body += "Brand: " + itemFull.Brand;
                    mail.Body += "\r\n";
                    mail.Body += "Model: " + itemFull.Model;
                    mail.Body += "\r\n";
                    mail.Body += "Producer: " + itemFull.Producer;
                    mail.Body += "\r\n";
                    mail.Body += "Count: " + item.Count;
                    mail.Body += "\r\n";
                }
            }

            // Send:
            client.Send(mail);

            return Redirect("/Orders/ProviderOrder/SuccessSend");
        }

        //GET: Get success notification page for sending
        [HttpGet]
        [RightCheck(Right = "Send request to provider")]
        public ActionResult SuccessSend()
        {
            return View();
        }

        //GET: Inbox
        [HttpGet]
        [RightCheck(Right = "Send request to provider")]
        public ActionResult ProviderAnswers()
        {
            try
            {
                //Create imap client
                var client = new ImapClient();

                client.Port = 993;
                client.SslProtocol = SslProtocols.Default;
                client.Host = "imap-mail.outlook.com";
                client.ValidateServerCertificate = true;
                client.UseSsl = true;

                var viewModel = new List<EmailViewModel>();

                //Connect
                if (client.Connect())
                {
                    var organisation = this.organisationService.GetById(
               this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()));

                    var sentFrom = organisation.EmailClient;
                    var pwd = KAssets.Controllers.StaticFunctions.RSAALg.Decryption(organisation.EmailClientPassword);

                    //Login
                    if (client.Login(sentFrom, pwd))
                    {
                        //Download the messages
                        client.Folders.Inbox.Messages.Download();
                        MessageCollection messages = client.Folders.Inbox.Messages;

                        //Add messages to viewmodel
                        foreach (var mess in messages)
                        {
                            viewModel.Add(new EmailViewModel
                                {
                                    DateOfSend = mess.Date.Value.ToString(),
                                    From = mess.From.Address,
                                    Seen = mess.Seen,
                                    Subject = mess.Subject,
                                    UId = mess.UId
                                });
                        }

                    }
                }
                else
                {
                    ModelState.AddModelError("", "Connection Failed. Please refresh");
                }

                client.Disconnect();
                return View(viewModel);
            }
            catch
            {
                return Redirect("/Orders/ProviderOrder/ProviderAnswers");
            }
        }

        //GET: View inbox message
        [HttpGet]
        [RightCheck(Right = "Send request to provider")]
        public ActionResult ViewProviderAnswer(long id)
        {
            var userOrg = this.userService.GetById(this.User.Identity.GetUserId()).Site.Organisation;
            try
            {
                //Create imap clien
                var client = new ImapClient();

                client.Port = 993;
                client.SslProtocol = SslProtocols.Default;
                client.Host = "imap-mail.outlook.com";
                client.ValidateServerCertificate = true;
                client.UseSsl = true;

                var viewModel = new EmailDetailsViewModel();
                viewModel.Request = new AddProviderToRequestViewModel();
                viewModel.Items = new AddItemsFromRequest();

                //Add request offers to viewmodel
                viewModel.Request.Offers = new List<ProviderOfferViewModel> 
                { 
                    new ProviderOfferViewModel
                    {
                        Price = 1,
                        Quantity = 1, 
                        Currency = this.currencyService.GetAll()
                        .Where(x=>x.OrganisationId==userOrg.Id)
                        .ToList()
                       .ConvertAll(x =>
                       new CurrencyViewModel
                       {
                           Code = x.Code,
                           Id = x.Id,
                       }).ToList()
                  } 
                };

                //Add items to viewmodel
                viewModel.Items.Items = new List<AddItemViewModel> { 
                   new AddItemViewModel {
                    Organisations = new List<OrganisationViewModel>{
                        new OrganisationViewModel
                    {
                        Id = userOrg.Id,
                        Name = userOrg.Name
                    }},
                    Price = 1,
                    Quantity = 1,
                    Currency = this.currencyService.GetAll()
                        .Where(x=>x.OrganisationId==userOrg.Id)
                        .ToList()
                       .ConvertAll(x =>
                       new CurrencyViewModel
                       {
                           Code = x.Code,
                           Id = x.Id,
                       }).ToList()
                }};

                //Connect
                if (client.Connect())
                {
                    var organisation = this.organisationService.GetById(
                      this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()));

                    var sentFrom = organisation.EmailClient;
                    var pwd = KAssets.Controllers.StaticFunctions.RSAALg.Decryption(organisation.EmailClientPassword);

                    //Login
                    if (client.Login(sentFrom, pwd))
                    {
                        //Download the messages
                        client.Folders.Inbox.Messages.Download();

                        //Select the message
                        MessageCollection messages = client.Folders.Inbox.Messages;
                        var message = messages.Where(x => x.UId == id).First();

                        message.Seen = true;

                        message.Body.Download();
                        viewModel.Content = message.Body.Html;
                        viewModel.DateOfSend = message.Date.ToString();
                        viewModel.From = message.From.Address;
                        viewModel.Subject = message.Subject;
                        viewModel.To = message.To.First().Address;
                        viewModel.UId = id;

                        viewModel.Request.ProviderEmail = message.From.Address;
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Connection Failed. Please refresh !");
                }
                client.Disconnect();

                //Add ofers to viewmodel
                if (TempData["Values"] != null)
                {
                    viewModel.Request.Offers = new List<ProviderOfferViewModel>();
                    var keys = JsonConvert.DeserializeObject<List<ProviderOfferViewModel>>(TempData["Values"].ToString());
                    for (int i = 0; i < keys.Count; i++)
                    {
                        viewModel.Request.Offers.Add(keys[i]);
                        viewModel.Request.Offers[i].Currency = this.currencyService.GetAll()
                        .Where(x => x.OrganisationId == userOrg.Id)
                        .ToList()
                           .ConvertAll(x =>
                           new CurrencyViewModel
                           {
                               Code = x.Code,
                               Id = x.Id,
                           }).ToList();

                    }
                }

                //Add items to viewmodel
                if (TempData["ValuesItems"] != null)
                {
                    viewModel.Items.Items = new List<AddItemViewModel>();
                    var keys = JsonConvert.DeserializeObject<List<AddItemViewModel>>(TempData["ValuesItems"].ToString());
                    for (int i = 0; i < keys.Count; i++)
                    {
                        keys[i].Organisations = new List<OrganisationViewModel>{
                        new OrganisationViewModel
                        {
                            Id = userOrg.Id,
                            Name = userOrg.Name
                        }};
                        viewModel.Items.Items.Add(keys[i]);
                        viewModel.Items.Items[i].Currency = this.currencyService.GetAll()
                        .Where(x => x.OrganisationId == userOrg.Id)
                        .ToList()
                          .ConvertAll(x =>
                          new CurrencyViewModel
                          {
                              Code = x.Code,
                              Id = x.Id,
                          }).ToList();
                    }
                }

                //Add model errors to ModelState
                if (TempData["Keys"] != null)
                {
                    var keys = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(TempData["Keys"].ToString());
                    for (int i = 0; i < keys.Count; i++)
                    {
                        foreach (var item in keys.ElementAt(i).Value)
                        {
                            this.ModelState.AddModelError(keys.ElementAt(i).Key, item);
                        }

                    }
                }

                TempData["Keys"] = null;
                TempData["Values"] = null;
                return View(viewModel);
            }
            catch
            {
                return Redirect("/Orders/ProviderOrder/ViewProviderAnswer/" + id);
            }
        }

        //POST: Add offfer from provider
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RightCheck(Right = "Send request to provider")]
        public ActionResult AddProviderOffer(AddProviderToRequestViewModel req)
        {
            var userOrg = this.userService.GetById(this.User.Identity.GetUserId()).Site.OrganisationId;
            if (userOrg != this.requestToProviderService.GetById(req.PoId).From.Site.OrganisationId)
            {
                return Redirect("/Home/NotAuthorized");
            }

            if (!ModelState.IsValid)
            {
                var errorList = ModelState.ToDictionary(
                                    kvp => kvp.Key,
                                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                                );

                //Put the errors and model values to tempdata
                TempData["Keys"] = JsonConvert.SerializeObject(errorList);
                TempData["Values"] = JsonConvert.SerializeObject(req.Offers);

                var url = this.HttpContext.Request.UrlReferrer.ToString();
                TempData["Show"] = 1;
                return Redirect(url);
            }

            //Check is there a request with this id
            if (!this.requestToProviderService.GetAll().Any(x => x.Id == req.PoId))
            {
                this.ModelState.AddModelError("PoId", ProviderOrderTr.EixstReq);
                var errorList = ModelState.ToDictionary(
                                    kvp => kvp.Key,
                                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                                );

                //Put the errors and model values to tempdata
                TempData["Keys"] = JsonConvert.SerializeObject(errorList);
                TempData["Values"] = JsonConvert.SerializeObject(req.Offers);

                var url = this.HttpContext.Request.UrlReferrer.ToString();
                TempData["Show"] = 1;
                return Redirect(url);
            }
            //Check are there a offers for this request
            var po = this.requestToProviderService.GetById(req.PoId);
            if (po.WantItems != null && po.SendOffers.Count != 0)
            {
                this.ModelState.AddModelError("PoId", ProviderOrderTr.AddedOffers);
                var errorList = ModelState.ToDictionary(
                                    kvp => kvp.Key,
                                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                                );

                //Put the errors and model values to tempdata
                TempData["Keys"] = JsonConvert.SerializeObject(errorList);
                TempData["Values"] = JsonConvert.SerializeObject(req.Offers);

                var url = this.HttpContext.Request.UrlReferrer.ToString();
                TempData["Show"] = 1;
                return Redirect(url);
            }


            //Add offers to the request
            foreach (var item in req.Offers)
            {
                this.requestToProviderService.AddOffer(new ProviderItemOffer
                    {
                        Brand = item.Brand,
                        Model = item.ItemModel,
                        Price = new Price { Value = item.Price, CurrencyId = item.SelectedCurrency },
                        Quantity = item.Quantity,
                        Producer = item.Producer

                    }, po.Id);
            }

            //Add a event that are added offers to request
            var aEvent = new KAssets.Models.Event
            {
                Content = "There are new request to provider for approving! ",
                Date = DateTime.Now,
                EventRelocationUrl = "/Orders/ProviderOrder/GetRequestsForApproving"
            };

            this.eventService.AddForUserGroup(aEvent, "Approve request to provider",
                this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()));

            return Redirect("/Home/Index");
        }

        //GET: Success send notification
        [HttpGet]
        [RightCheck(Right = "Approve request to provider")]
        public ActionResult SuccessSendApproving()
        {
            return View();
        }

        //GET: Get requests for approving
        [HttpGet]
        [RightCheck(Right = "Approve request to provider")]
        public ActionResult GetRequestsForApproving()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var requests = this.requestToProviderService.GetAll()
                .Where(x => !x.IsSeenByApprover)
                .Where(x => !x.IsFinished)
                .Where(x => x.SendOffers.Count != 0);

            if (!this.IsMegaAdmin())
            {
                requests = requests.Where(x => x.From.Site.OrganisationId == userOrg);
            }

            var viewModel = requests.ToList().ConvertAll(
                x => new RequestToProviderViewModel
                {
                    Id = x.Id,
                    FromUser = (x.From.FirstName != null) ?
                   x.From.FirstName + " " + x.From.SecondName + " " + x.From.LastName :
                   x.From.Email,
                    DateOfSend = x.DateOfSend.ToString(),
                    Provider = x.Provider.Email
                });
            return View(viewModel);

        }

        //GET: View request for approving
        [HttpGet]
        [RightCheck(Right = "Approve request to provider")]
        public ActionResult ViewRequestForApproving(int id)
        {
            var request = this.requestToProviderService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            //Add request data to viewmodel
            var viewModel = new RequestToProviderFullViewModel
            {
                DateOfSend = request.DateOfSend.ToString(),
                FromName = (request.From.FirstName != null) ?
                    request.From.FirstName + " " + request.From.SecondName + " " + request.From.LastName :
                    request.From.Email,
                ProviderEmail = request.Provider.Email,
                Offers = request.SendOffers.ToList().ConvertAll(x => new ProviderOfferViewModel
                {
                    Brand = x.Brand,
                    ItemModel = x.Model,
                    Price = x.Price.Value,
                    Producer = x.Producer,
                    Quantity = x.Quantity,
                    CurrencyNotaion = x.Price.Currency.Code,
                    Id = x.Id
                }),
                Id = id
            };
            viewModel.Offers.OrderBy(x => x.Brand).OrderBy(x => x.ItemModel).OrderBy(x => x.Producer);

            var baseCurrencyId = this.currencyService.GetAll().First().Id;

            //Select the the best offers
            foreach (var item in viewModel.Offers)
            {
                int flag = 0;
                foreach (var item2 in viewModel.Offers)
                {
                    if (item2.ItemModel.Trim().ToLower() == item.ItemModel.Trim().ToLower() &&
                    item2.Brand.Trim().ToLower() == item.Brand.Trim().ToLower())
                    {
                        var price2 = this.exchangeService.GetRateNotation(baseCurrencyId, item2.CurrencyNotaion) * item2.Price;
                        var price = this.exchangeService.GetRateNotation(baseCurrencyId, item.CurrencyNotaion) * item.Price;

                        if (price2 < price)
                        {
                            item2.IsSelected = true;
                            item.IsSelected = false;
                        }
                        flag++;
                    }
                }
                if (flag == 1)
                {
                    item.IsSelected = true;
                }
            }

            return View(viewModel);
        }


        //GET: Decline a request
        [HttpGet]
        [RightCheck(Right = "Approve request to provider")]
        public ActionResult Decline(int id)
        {
            var request = this.requestToProviderService.GetById(id);

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
        [RightCheck(Right = "Approve request to provider")]
        [ValidateAntiForgeryToken]
        public ActionResult Decline(DeclineRequestViewModel model)
        {
            var request = this.requestToProviderService.GetById(model.RequestId);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            //Add event that request is not approved
            var aEvent = new KAssets.Models.Event
            {
                UserId = request.FromId,
                Content = "Your request to provider was not approved. " + model.Message,
                Date = DateTime.Now,
                EventRelocationUrl = "/Orders/ProviderOrder/ViewHistoryRequest/" + model.RequestId
            };

            this.eventService.Add(aEvent);
            this.requestToProviderService.SetFinished(model.RequestId);

            return Redirect("/Orders/ProviderOrder/GetRequestsForApproving");
        }

        //POST: Approve a request for relocation
        [HttpPost]
        [RightCheck(Right = "Approve request to provider")]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveRequest(RequestToProviderFullViewModel model)
        {
            var request = this.requestToProviderService.GetById(model.Id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            //Set request is approved
            this.requestToProviderService.SetApproved(model.Id);

            //Set request is seen by approved
            this.requestToProviderService.SetIsSeenByApproved(model.Id);

            //Add approved offers to request
            var ids = model.Offers.Where(x => x.IsSelected == true).Select(x => x.Id).ToList();
            this.requestToProviderService.SetApprovedOffer(model.Id, ids);


            var organisation = this.organisationService.GetById(
                this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()));

            var credentialUserName = organisation.EmailClient;
            var sentFrom = organisation.EmailClient;
            var pwd = KAssets.Controllers.StaticFunctions.RSAALg.Decryption(organisation.EmailClientPassword);

            // Configure the client:
            System.Net.Mail.SmtpClient client =
                new System.Net.Mail.SmtpClient("smtp-mail.outlook.com");

            client.Port = 587;
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            // Creatte the credentials:
            System.Net.NetworkCredential credentials =
                new System.Net.NetworkCredential(credentialUserName, pwd);

            client.EnableSsl = true;
            client.Credentials = credentials;

            // Create the message:
            var to = request.Provider.Email;
            var mail =
                new System.Net.Mail.MailMessage(sentFrom, to);

            request = this.requestToProviderService.GetById(model.Id);

            mail.Subject = "The company wants: ";

            var body = "Order id: " + model.Id;
            foreach (var item in request.SendOffers.Where(x => x.IsApproved))
            {


                body += " " + "Brand: " + item.Brand + "\r\n Model: " + item.Model + "\r\n Producer: " +
                  item.Producer + "\r\n  Price: " + item.Price.Value + " " + item.Price.Currency.Code + "\r\n Quantity: " + item.Quantity + "\r\n \r\n";
            }
            mail.Body = body;
            // Send:
            client.Send(mail);

            //Add event that request is approved
            var aEvent = new KAssets.Models.Event
            {
                UserId = request.FromId,
                Content = "Your request to provider was approved. ",
                Date = DateTime.Now,
                EventRelocationUrl = "/Orders/ProviderOrder/GetApprovedRequests"
            };

            this.eventService.Add(aEvent);

            return Redirect("/Orders/ProviderOrder/GetRequestsForApproving");
        }

        //GET: Get approved
        [HttpGet]
        [RightCheck(Right = "Send request to provider")]
        public ActionResult GetApprovedRequests()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var requests = this.requestToProviderService.GetAll()
                .Where(x => x.IsSeenByApprover)
                .Where(x => x.IsApproved)
                .Where(x => !x.IsFinished);

            if (!this.IsMegaAdmin())
            {
                requests = requests.Where(x => x.FromId == this.User.Identity.GetUserId())
                .Where(x => x.From.Site.OrganisationId == userOrg);
            }

            var viewModel = requests.ToList().ConvertAll(
                x => new RequestToProviderViewModel
                {
                    Id = x.Id,
                    FromUser = (x.From.FirstName != null) ?
                   x.From.FirstName + " " + x.From.SecondName + " " + x.From.LastName :
                   x.From.Email,
                    DateOfSend = x.DateOfSend.ToString(),
                    Provider = x.Provider.Email
                });
            return View(viewModel);

        }

        //GET: View request for approving
        [HttpGet]
        [RightCheck(Right = "Send request to provider")]
        public ActionResult ViewApprovedRequest(int id)
        {
            var request = this.requestToProviderService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            //Create email for provider
            var sentEmail = "";
            foreach (var item in request.SendOffers.Where(x => x.IsApproved))
            {

                sentEmail += " " + "Brand: " + item.Brand + "<br/>   Model: " + item.Model + "<br/>   Producer: " +
                  item.Producer + "<br/>    Price: " + item.Price.Value + " " + item.Price.Currency.Code + "<br/>   Quantity: " + item.Quantity + "<br/>  <br/>  ";
            }

            //Set viewmodel data
            var viewModel = new RequestToProviderFullViewModel
            {
                DateOfSend = request.DateOfSend.ToString(),
                FromName = (request.From.FirstName != null) ?
                    request.From.FirstName + " " + request.From.SecondName + " " + request.From.LastName :
                    request.From.Email,
                ProviderEmail = request.Provider.Email,
                Offers = request.SendOffers.ToList().ConvertAll(x => new ProviderOfferViewModel
                {
                    Brand = x.Brand,
                    ItemModel = x.Model,
                    Price = x.Price.Value,
                    Producer = x.Producer,
                    Quantity = x.Quantity,
                    IsSelected = x.IsApproved,
                    CurrencyNotaion = x.Price.Currency.Code
                }),
                WantItems = request.WantItems.ToList().ConvertAll(x => new ProviderOfferViewModel
                {
                    Brand = x.Brand,
                    ItemModel = x.Model,
                    Price = x.Price.Value,
                    Producer = x.Producer,
                    Quantity = request.CountItems.Where(y => y.Key == x.Id).FirstOrDefault().Want,
                    CurrencyNotaion = x.Price.Currency.Code
                }),
                Id = id,
                SentEmail = sentEmail
            };

            return View(viewModel);
        }

        //POST: Add items from provider answer
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RightCheck(Right = "Send request to provider")]
        public ActionResult AddItems(AddItemsFromRequest model)
        {
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.ToDictionary(
                                    kvp => kvp.Key,
                                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                                );

                //Add errors and values to tempdata
                TempData["Keys"] = JsonConvert.SerializeObject(errorList);
                TempData["ValuesItems"] = JsonConvert.SerializeObject(model.Items);

                var url = this.HttpContext.Request.UrlReferrer.ToString();
                TempData["Show"] = 2;
                return Redirect(url);
            }

            var userOrg = this.userService.GetById(this.User.Identity.GetUserId()).Site.OrganisationId;
            if (userOrg != this.requestToProviderService.GetById(model.PoId).From.Site.OrganisationId)
            {
                return Redirect("/Home/NotAuthorized");
            }

            bool valid = true;

            for (int i = 0; i < model.Items.Count; i++)
            {
                for (int j = i + 1; j < model.Items.Count; j++)
                {
                    if (model.Items[i].Id == model.Items[j].Id)
                    {
                        valid = false;
                    }
                }
            }

            if ((!ModelState.IsValid) || (!valid))
            {
                var errorList = ModelState.ToDictionary(
                                    kvp => kvp.Key,
                                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                                );

                //Add errors and values to tempdata
                TempData["Keys"] = JsonConvert.SerializeObject(errorList);
                TempData["ValuesItems"] = JsonConvert.SerializeObject(model.Items);

                var url = this.HttpContext.Request.UrlReferrer.ToString();
                TempData["Show"] = 2;
                return Redirect(url);
            }

            bool hasExist = false;

            //Check is there a item with the same id
            foreach (var item in model.Items)
            {
                if (this.itemService.Exist(item.Id))
                {
                    this.ModelState.AddModelError("", ItemTr.Exist);
                    hasExist = true;
                }
            }

            //Check is there a order with this id or is it finished
            if (!this.requestToProviderService.GetAll().Any(x => x.Id == model.PoId))
            {
                this.ModelState.AddModelError("PoId", ProviderOrderTr.EixstReq);
                hasExist = true;
            }
            else
            {
                if (this.requestToProviderService.GetById(model.PoId).IsFinished)
                {

                    this.ModelState.AddModelError("PoId", ProviderOrderTr.OrderFinished);
                    hasExist = true;
                }
            }

            //Check are there added items
            var po = this.requestToProviderService.GetById(model.PoId);
            if (po.GiveItems != null && po.GiveItems.Count != 0)
            {
                this.ModelState.AddModelError("PoId", ProviderOrderTr.AddedOffers);
                hasExist = true;
            }


            if ((!ModelState.IsValid) || hasExist)
            {
                var errorList = ModelState.ToDictionary(
                                    kvp => kvp.Key,
                                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                                );

                //Add errors and values to tempdata
                TempData["Keys"] = JsonConvert.SerializeObject(errorList);
                TempData["ValuesItems"] = JsonConvert.SerializeObject(model.Items);

                var url = this.HttpContext.Request.UrlReferrer.ToString();
                TempData["Show"] = 2;
                return Redirect(url);
            }

            //Add items to database
            foreach (var item in model.Items)
            {
                this.itemService.Add(new KAssets.Models.Item
               {
                   Id = item.Id,
                   Brand = item.Brand,
                   DateOfManufacture = item.DateOfManufacture,
                   Model = item.ItemModel,
                   Price = new Price { Value = item.Price, CurrencyId = item.SelectedCurrency },
                   Producer = item.Producer,
                   Quantity = item.Quantity,
                   RotatingItem = item.RotatingItem,
                   OrganisationId = item.SeletedOrganisationId,
                   Status = "Active"
               });
            }

            //Add items to request and set request is finished
            this.requestToProviderService.AddGiveItems(model.PoId, model.Items.Select(x => x.Id).ToList());
            this.requestToProviderService.SetFinished(model.PoId);

            return Redirect("/Home/Index");
        }

        //GET: Your history 
        [HttpGet]
        [RightCheck(Right = "Send request to provider")]
        public ActionResult HistoryRequests()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var requests = this.requestToProviderService.GetAll().Where(x => x.IsFinished)
                .Where(x => x.FromId == this.User.Identity.GetUserId())
                .Where(x => x.From.Site.OrganisationId == userOrg);

            var viewModel = new List<ViewSentRequestToProviderViewModel>();

            foreach (var item in requests)
            {
                viewModel.Add(new ViewSentRequestToProviderViewModel
                {
                    DateOfSend = item.DateOfSend.ToString(),
                    FromName = (item.From.FirstName != null) ?
                                item.From.FirstName + " " + item.From.SecondName + " " + item.From.LastName :
                                item.From.Email,
                    Id = item.Id,
                    EmailSubject = item.SentSubject
                });
            }

            return View(viewModel);
        }

        //GET: View your history request
        [HttpGet]
        [RightCheck(Right = "Send request to provider")]
        public ActionResult ViewHistoryRequest(int id)
        {
            var request = this.requestToProviderService.GetById(id);

            //Verify if request is from user organisation
            if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
            {
                return Redirect("/Home/NotAuthorized");
            }

            if (request.FromId != this.User.Identity.GetUserId())
            {
                return Redirect("/Home/NotAuthorized");
            }

            var sentEmail = "";
            foreach (var item in request.SendOffers.Where(x => x.IsApproved))
            {
                sentEmail += " " + "Brand: " + item.Brand + "<br/>   Model: " + item.Model + "<br/>   Producer: " +
                  item.Producer + "<br/>    Price: " + item.Price.Value + " " + item.Price.Currency.Code + "<br/>   Quantity: " + item.Quantity + "<br/>  <br/>  ";
            }

            var viewModel = new RequestToProviderFullViewModel
            {
                DateOfSend = request.DateOfSend.ToString(),
                FromName = (request.From.FirstName != null) ?
                    request.From.FirstName + " " + request.From.SecondName + " " + request.From.LastName :
                    request.From.Email,
                ProviderEmail = request.Provider.Email,
                Offers = request.SendOffers.ToList().ConvertAll(x => new ProviderOfferViewModel
                {
                    Brand = x.Brand,
                    ItemModel = x.Model,
                    Price = x.Price.Value,
                    Producer = x.Producer,
                    Quantity = x.Quantity,
                    IsSelected = x.IsApproved,
                    CurrencyNotaion = x.Price.Currency.Code
                }),
                WantItems = request.WantItems.ToList().ConvertAll(x => new ProviderOfferViewModel
                {
                    Brand = x.Brand,
                    ItemModel = x.Model,
                    Price = x.Price.Value,
                    Producer = x.Producer,
                    Quantity = request.CountItems.Where(y => y.Key == x.Id).FirstOrDefault().Want,
                    CurrencyNotaion = x.Price.Currency.Code
                }),
                GiveItems = request.GiveItems.ToList().ConvertAll(x => new ProviderOfferViewModel
                {
                    Brand = x.Brand,
                    ItemModel = x.Model,
                    Price = x.Price.Value,
                    Producer = x.Producer,
                    Quantity = request.CountItems.Where(y => y.Key == x.Id).FirstOrDefault().Give,
                    CurrencyNotaion = x.Price.Currency.Code
                }),
                Id = id,
                SentEmail = sentEmail
            };

            return View(viewModel);
        }

        //GET: All history
        [HttpGet]
        [RightCheck(Right = "Approve request to provider")]
        public ActionResult HistoryRequestsAll()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
            var requests = this.requestToProviderService.GetAll().Where(x => x.IsFinished);
            if (!this.IsMegaAdmin())
            {
                requests = requests.Where(x => x.From.Site.OrganisationId == userOrg);
            }

            var viewModel = new List<ViewSentRequestToProviderViewModel>();

            foreach (var item in requests)
            {
                viewModel.Add(new ViewSentRequestToProviderViewModel
                {
                    DateOfSend = item.DateOfSend.ToString(),
                    FromName = (item.From.FirstName != null) ?
                                item.From.FirstName + " " + item.From.SecondName + " " + item.From.LastName :
                                item.From.Email,
                    Id = item.Id,
                    EmailSubject = item.SentSubject
                });
            }

            return View(viewModel);
        }

        //GET: View all history request
        [HttpGet]
        [RightCheck(Right = "Approve request to provider")]
        public ActionResult ViewHistoryRequestAll(int id)
        {
            var request = this.requestToProviderService.GetById(id);

            //Verify if request is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (request.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var sentEmail = "";
            foreach (var item in request.SendOffers.Where(x => x.IsApproved))
            {
                sentEmail += " " + "Brand: " + item.Brand + "<br/>   Model: " + item.Model + "<br/>   Producer: " +
                  item.Producer + "<br/>    Price: " + item.Price.Value + " " + item.Price.Currency.Code + "<br/>   Quantity: " + item.Quantity + "<br/>  <br/>  ";
            }

            var viewModel = new RequestToProviderFullViewModel
            {
                DateOfSend = request.DateOfSend.ToString(),
                FromName = (request.From.FirstName != null) ?
                    request.From.FirstName + " " + request.From.SecondName + " " + request.From.LastName :
                    request.From.Email,
                ProviderEmail = request.Provider.Email,
                Offers = request.SendOffers.ToList().ConvertAll(x => new ProviderOfferViewModel
                {
                    Brand = x.Brand,
                    ItemModel = x.Model,
                    Price = x.Price.Value,
                    Producer = x.Producer,
                    Quantity = x.Quantity,
                    IsSelected = x.IsApproved,
                    CurrencyNotaion = x.Price.Currency.Code
                }),
                WantItems = request.WantItems.ToList().ConvertAll(x => new ProviderOfferViewModel
                {
                    Brand = x.Brand,
                    ItemModel = x.Model,
                    Price = x.Price.Value,
                    Producer = x.Producer,
                    Quantity = request.CountItems.Where(y => y.Key == x.Id).FirstOrDefault().Want,
                    CurrencyNotaion = x.Price.Currency.Code
                }),
                GiveItems = request.GiveItems.ToList().ConvertAll(x => new ProviderOfferViewModel
                {
                    Brand = x.Brand,
                    ItemModel = x.Model,
                    Price = x.Price.Value,
                    Producer = x.Producer,
                    Quantity = request.CountItems.Where(y => y.Key == x.Id).FirstOrDefault().Give,
                    CurrencyNotaion = x.Price.Currency.Code
                }),
                Id = id,
                SentEmail = sentEmail
            };

            return View("ViewHistoryRequest", viewModel);
        }
    }

}