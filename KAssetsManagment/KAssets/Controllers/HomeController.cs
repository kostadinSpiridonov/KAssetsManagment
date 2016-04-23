using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KAssets.Resources;
using KAssets.Models;
using System.Net.Http;

namespace KAssets.Controllers
{
    public class HomeController : BaseController
    {
        //GET:Home page
        [Authorize]
        public ActionResult Index()
        {
            var viewModel = new HomePageViewModel();

            viewModel.SystemAccidents = new List<DataTypeMorrisArea<int, string>>();
            viewModel.ScrappedAssets = new List<DataTypeMorrisArea<int, string>>();
            viewModel.IssuedInvoices = new List<DataTypeMorrisArea<int, string>>();
            viewModel.RenovatedAssets = new List<DataTypeMorrisArea<int, string>>();
            viewModel.AquiredAssets = new List<DataTypeMorrisArea<int, string>>();

            if (this.userService.GetById(this.User.Identity.GetUserId()).Site == null)
            {
                return View(viewModel);
            }

            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());

            //Get count of unseen event for user
            viewModel.Events = this.eventService.GetAllForUser(this.User.Identity.GetUserId())
                .Where(x => (!x.IsSeen)).Count();

            //Get employers count
            if (!this.IsMegaAdmin())
            {
                viewModel.Employess = this.userService.GetAll()
                    .Where(x => x.Status == "Active")
                    .Where(x => x.Site != null)
                    .Where(x => x.Site.OrganisationId == userOrg).Count();
            }
            else
            {
                viewModel.Employess = this.userService.GetAll()
                    .Where(x => x.Status == "Active")
                    .Where(x => x.Site != null).Count();
            }

            //Get assets count
            if (!this.IsMegaAdmin())
            {
                viewModel.Assets = this.assetService.GetAll()
                    .Where(x => x.Status == "Active")
                    .Where(x => x.Site.OrganisationId == userOrg).Count();
            }
            else
            {
                viewModel.Assets = this.assetService.GetAll()
                    .Where(x => x.Status == "Active").Count();
            }

            //Get items count
            if (!this.IsMegaAdmin())
            {
                viewModel.Items = this.itemService.GetAll()
                    .Where(x => x.Status == "Active")
                    .Where(x => x.OrganisationId == userOrg).Count();
            }
            else
            {
                viewModel.Items = this.itemService.GetAll()
                    .Where(x => x.Status == "Active").Count();
            }
            //Get accuired asset for last year by month (count, month)
            if (KAssets.Controllers.StaticFunctions.IsHasRihgt("Manage assets", User.Identity.GetUserId()))
            {
                viewModel.AquiredAssets = this.GetAquiredAssets();
            }

            //Get scrraped asset for last year by month (count,month)
            if (KAssets.Controllers.StaticFunctions.IsHasRihgt("Manage assets", User.Identity.GetUserId()))
            {
                viewModel.ScrappedAssets = this.GetScrappedAssets();
            }

            //Get renovated asset for last year by month (count,month)
            if (KAssets.Controllers.StaticFunctions.IsHasRihgt("Manage assets", User.Identity.GetUserId()))
            {
                viewModel.RenovatedAssets = this.GetRenovatedAssets();
            }

            //Get issued invoices for last year by month (count,month)
            if (KAssets.Controllers.StaticFunctions.IsHasRihgt("Create invoice", User.Identity.GetUserId())
                ||
                KAssets.Controllers.StaticFunctions.IsHasRihgt("Approve invoice", User.Identity.GetUserId())
                ||
                KAssets.Controllers.StaticFunctions.IsHasRihgt("Pay invoice", User.Identity.GetUserId()))
            {
                viewModel.IssuedInvoices = this.GetIssuedInvoices();
            }

            //Get system accidents for last year by month (count,month)
            if (KAssets.Controllers.StaticFunctions.IsHasRihgt("Responding to incidents", User.Identity.GetUserId()))
            {
                viewModel.SystemAccidents = this.GetSystemAccidents();
            }

            return View(viewModel);
        }

        //GET: Login page
        [HttpGet]
        public ActionResult IndexLogIn()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/Home/Index");
            }
            return View();
        }

        //GET: Not authorized page
        [HttpGet]
        public ActionResult NotAuthorized()
        {
            return View();
        }

        //Get aquired asset for last yers
        private List<DataTypeMorrisArea<int, string>> GetAquiredAssets()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());

            var assets = this.assetService.GetAll();
            if (!this.IsMegaAdmin())
            {
                assets = assets.Where(x => x.Site.OrganisationId == userOrg).ToList();
            }

            var year = DateTime.Now.Year.ToString();
            var aquiredAssets = new List<DataTypeMorrisArea<int, string>>();

            //Sorted asset by month and calculate count of asset for every month
            foreach (var item in assets)
            {
                //Get the creattion date of asset
                var date = item.History.Rows.Where(x => x.Content == "The asset was acquired.").FirstOrDefault().Date;

                if (date.Year == DateTime.Now.Year)
                {

                    if (aquiredAssets.Any(x => x.Date == date.Month.ToString()))
                    {
                        //Increase the count of asset for certain month
                        aquiredAssets.Where(x => x.Date == date.Month.ToString()).FirstOrDefault().Value++;
                    }
                    else
                    {
                        //Add a new asset to list for returned 
                        aquiredAssets.Add(new DataTypeMorrisArea<int, string>
                            {
                                Date = date.Month.ToString(),
                                Value = 1
                            });
                    }
                }
            }

            //Change the date string for the script
            foreach (var item in aquiredAssets)
            {
                if (item.Date.Count() == 1)
                {
                    item.Date = year + "-0" + item.Date;
                }
                else
                {
                    item.Date = year + "-" + item.Date;
                }
            }
            return aquiredAssets;
        }

        //Get scrapped asset for last yead
        private List<DataTypeMorrisArea<int, string>> GetScrappedAssets()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());

            var assets = this.assetService.GetAll();
            if (!this.IsMegaAdmin())
            {
                assets = assets.Where(x => x.Site.OrganisationId == userOrg).ToList();
            }

            var year = DateTime.Now.Year.ToString();
            var scrappedAssets = new List<DataTypeMorrisArea<int, string>>();

            //Sorted asset by month and calculate count of asset for every month
            foreach (var item in assets)
            {
                if (item.Status == "Scrapped")
                {
                    //Get the creattion date of asset
                    var date = item.History.Rows.Where(x => x.Content == "The asset was scrapped.").FirstOrDefault().Date;

                    if (date.Year == DateTime.Now.Year)
                    {
                        if (scrappedAssets.Any(x => x.Date == date.Month.ToString()))
                        {
                            //Increase the count of asset for certain month
                            scrappedAssets.Where(x => x.Date == date.Month.ToString()).FirstOrDefault().Value++;
                        }
                        else
                        {
                            //Add a new asset to list for returned 
                            scrappedAssets.Add(new DataTypeMorrisArea<int, string>
                            {
                                Date = date.Month.ToString(),
                                Value = 1
                            });
                        }
                    }
                }
            }

            //Change the date string for the script
            foreach (var item in scrappedAssets)
            {
                if (item.Date.Count() == 1)
                {
                    item.Date = year + "-0" + item.Date;
                }
                else
                {
                    item.Date = year + "-" + item.Date;
                }
            }

            return scrappedAssets;
        }

        //Get renovated asset for last year
        private List<DataTypeMorrisArea<int, string>> GetRenovatedAssets()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());

            var assets = this.assetService.GetAll();

            if (!this.IsMegaAdmin())
            {
                assets = assets.Where(x => x.Site.OrganisationId == userOrg).ToList();
            }

            var year = DateTime.Now.Year.ToString();
            var renovatedAssets = new List<DataTypeMorrisArea<int, string>>();

            //Sorted asset by month and calculate count of asset for every month
            foreach (var item in assets)
            {
                if (item.History.Rows.Any(x => x.Content == "The asset was renovated."))
                {
                    //Get the creattion date of asset
                    var date = item.History.Rows.Where(x => x.Content == "The asset was renovated.").FirstOrDefault().Date;

                    if (date.Year == DateTime.Now.Year)
                    {
                        if (renovatedAssets.Any(x => x.Date == date.Month.ToString()))
                        {
                            //Increase the count of asset for certain month
                            renovatedAssets.Where(x => x.Date == date.Month.ToString()).FirstOrDefault().Value++;
                        }
                        else
                        {
                            //Add a new asset to list for returned 
                            renovatedAssets.Add(new DataTypeMorrisArea<int, string>
                            {
                                Date = date.Month.ToString(),
                                Value = 1
                            });
                        }
                    }
                }
            }

            //Change the date string for the script
            foreach (var item in renovatedAssets)
            {
                if (item.Date.Count() == 1)
                {
                    item.Date = year + "-0" + item.Date;
                }
                else
                {
                    item.Date = year + "-" + item.Date;
                }
            }

            return renovatedAssets;
        }

        //Get issued invoices for last year
        private List<DataTypeMorrisArea<int, string>> GetIssuedInvoices()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());

            var year = DateTime.Now.Year.ToString();
            var invoices = this.invoiceService.GetAll();

            if (!this.IsMegaAdmin())
            {
                invoices = invoices.Where(x => x.CompiledUser.Site.OrganisationId == userOrg).ToList();
            }

            var issuedIvoices = new List<DataTypeMorrisArea<int, string>>();

            //Sorted asset by month and calculate count of asset for every month
            foreach (var item in invoices)
            {
                //Get the creattion date of asset
                var date = item.DateOfCreation;

                if (date.Year == DateTime.Now.Year)
                {
                    if (issuedIvoices.Any(x => x.Date == date.Month.ToString()))
                    {
                        //Increase the count of asset for certain month
                        issuedIvoices.Where(x => x.Date == date.Month.ToString()).FirstOrDefault().Value++;
                    }
                    else
                    {
                        //Add a new asset to list for returned 
                        issuedIvoices.Add(new DataTypeMorrisArea<int, string>
                        {
                            Date = date.Month.ToString(),
                            Value = 1
                        });
                    }
                }
            }

            //Change the date string for the script
            foreach (var item in issuedIvoices)
            {
                if (item.Date.Count() == 1)
                {
                    item.Date = year + "-0" + item.Date;
                }
                else
                {
                    item.Date = year + "-" + item.Date;
                }
            }

            return issuedIvoices;
        }

        //Get system accidents for last year
        private List<DataTypeMorrisArea<int, string>> GetSystemAccidents()
        {
            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());

            var year = DateTime.Now.Year.ToString();
            var accidents = this.accidentService.GetAll();

            if (!this.IsMegaAdmin())
            {
                accidents = accidents.Where(x => x.From.Site.OrganisationId == userOrg).ToList();
            }

            var systemAccidents = new List<DataTypeMorrisArea<int, string>>();

            //Sorted asset by month and calculate count of asset for every month
            foreach (var item in accidents)
            {
                var date = item.DateOfSend;
                if (date.Year == DateTime.Now.Year)
                {
                    if (systemAccidents.Any(x => x.Date == date.Month.ToString()))
                    {
                        //Increase the count of asset for certain month
                        systemAccidents.Where(x => x.Date == date.Month.ToString()).FirstOrDefault().Value++;
                    }
                    else
                    {
                        //Add a new asset to list for returned 
                        systemAccidents.Add(new DataTypeMorrisArea<int, string>
                        {
                            Date = date.Month.ToString(),
                            Value = 1
                        });
                    }
                }
            }

            //Change the date string for the script
            foreach (var item in systemAccidents)
            {
                if (item.Date.Count() == 1)
                {
                    item.Date = year + "-0" + item.Date;
                }
                else
                {
                    item.Date = year + "-" + item.Date;
                }
            }

            return systemAccidents;
        }

        //GET: Get count notifications
        [HttpGet]
        [Authorize]
        public JsonResult CountNewThings()
        {
            var viewModel = new CountNewThingsViewModel();

            if (this.userService.GetById(this.User.Identity.GetUserId()).Site == null)
            {
                return Json(viewModel, JsonRequestBehavior.AllowGet);
            }

            var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());

            //Get count of request for approving in item order
            if (StaticFunctions.IsHasRihgt("Approve order for item", User.Identity.GetUserId()))
            {
                if (!this.IsMegaAdmin())
                {
                    viewModel.ItemOrderRequestsForApproving = this.requestToAcquireItemService.GetAll()
                    .Where(x => !x.IsApproved)
                    .Where(x => !x.AreItemsGave)
                    .Where(x => !x.Finished)
                    .Where(x => x.From.Site.OrganisationId == userOrg)
                    .Count();
                }
                else
                {
                    viewModel.ItemOrderRequestsForApproving = this.requestToAcquireItemService.GetAll()
                   .Where(x => !x.IsApproved)
                   .Where(x => !x.AreItemsGave)
                   .Where(x => !x.Finished)
                   .Count();
                }
            }

            //Get count of approved requests in item ored
            if (StaticFunctions.IsHasRihgt("Give items for item order", User.Identity.GetUserId()))
            {
                if (!this.IsMegaAdmin())
                {
                    viewModel.ItemOrderApprovedRequests = this.requestToAcquireItemService.GetAll()
                    .Where(x => x.IsApproved)
                    .Where(x => !x.Finished)
                    .Where(x => !x.AreItemsGave)
                    .Where(x => x.From.Site.OrganisationId == userOrg)
                    .Count();
                }
                else
                {
                    viewModel.ItemOrderApprovedRequests = this.requestToAcquireItemService.GetAll()
                  .Where(x => x.IsApproved)
                  .Where(x => !x.Finished)
                  .Where(x => !x.AreItemsGave)
                  .Count();
                }
            }

            //Get count of request for finishing in item order
            if (StaticFunctions.IsHasRihgt("Create order for item", User.Identity.GetUserId()))
            {
                if (!this.IsMegaAdmin())
                {
                    viewModel.ItemOrderRequestsForFinishing = this.requestToAcquireItemService.GetAll()
                    .Where(x => x.FromId == this.User.Identity.GetUserId())
                    .Where(x => x.IsApproved)
                    .Where(x => !x.Finished)
                    .Where(x => x.AreItemsGave)
                    .Where(x => x.From.Site.OrganisationId == userOrg)
                    .Count();
                }
                else
                {
                    viewModel.ItemOrderRequestsForFinishing = this.requestToAcquireItemService.GetAll()
                    .Where(x => x.FromId == this.User.Identity.GetUserId())
                    .Where(x => x.IsApproved)
                    .Where(x => !x.Finished)
                    .Where(x => x.AreItemsGave)
                    .Count();
                }
            }

            //Get count of rquest for approving in asset order
            if (StaticFunctions.IsHasRihgt("Approve orders for asset", User.Identity.GetUserId()))
            {
                if (!this.IsMegaAdmin())
                {
                    viewModel.AssetOrderRequestsForApproving = this.requestForAssetService.GetAll()
                    .Where(x => !x.IsApproved)
                    .Where(x => !x.AreAssetsGave)
                    .Where(x => !x.Finished)
                    .Where(x => x.From.Site.OrganisationId == userOrg)
                    .Count();
                }
                else
                {
                    viewModel.AssetOrderRequestsForApproving = this.requestForAssetService.GetAll()
                  .Where(x => !x.IsApproved)
                  .Where(x => !x.AreAssetsGave)
                  .Where(x => !x.Finished)
                  .Count();
                }
            }

            //Get count of approved requests in asset order
            if (StaticFunctions.IsHasRihgt("Give assets for asset orders", User.Identity.GetUserId()))
            {
                if (!this.IsMegaAdmin())
                {
                    viewModel.AssetOrderApprovedRequests = this.requestForAssetService.GetAll()
                   .Where(x => x.IsApproved)
                   .Where(x => !x.Finished)
                   .Where(x => !x.AreAssetsGave)
                   .Where(x => x.From.Site.OrganisationId == userOrg)
                   .Count();
                }
                else
                {
                    viewModel.AssetOrderApprovedRequests = this.requestForAssetService.GetAll()
                   .Where(x => x.IsApproved)
                   .Where(x => !x.Finished)
                   .Where(x => !x.AreAssetsGave)
                   .Count();
                }
            }

            //Get count of requeet for finishing in asset order
            if (StaticFunctions.IsHasRihgt("Create order for asset", User.Identity.GetUserId()))
            {
                if (!this.IsMegaAdmin())
                {
                    viewModel.AssetOrderRequestsForFinishing = this.requestForAssetService.GetAll()
                    .Where(x => x.FromId == this.User.Identity.GetUserId())
                    .Where(x => x.IsApproved)
                    .Where(x => !x.Finished)
                    .Where(x => x.AreAssetsGave)
                    .Where(x => x.From.Site.OrganisationId == userOrg)
                    .Count();
                }
                else
                {
                    viewModel.AssetOrderRequestsForFinishing = this.requestForAssetService.GetAll()
                     .Where(x => x.FromId == this.User.Identity.GetUserId())
                     .Where(x => x.IsApproved)
                     .Where(x => !x.Finished)
                     .Where(x => x.AreAssetsGave)
                     .Count();
                }
            }

            //Get count of request for approving in provider order
            if (StaticFunctions.IsHasRihgt("Approve request to provider", User.Identity.GetUserId()))
            {
                if (!this.IsMegaAdmin())
                {
                    viewModel.ProviderOrderRequestsForApproving = this.requestToProviderService.GetAll()
                    .Where(x => !x.IsSeenByApprover)
                    .Where(x => !x.IsFinished)
                    .Where(x => x.SendOffers.Count != 0)
                    .Where(x => x.From.Site.OrganisationId == userOrg)
                    .Count();
                }
                else
                {
                    viewModel.ProviderOrderRequestsForApproving = this.requestToProviderService.GetAll()
                 .Where(x => !x.IsSeenByApprover)
                 .Where(x => !x.IsFinished)
                 .Where(x => x.SendOffers.Count != 0)
                 .Count();
                }
            }

            //Get count of approved requests in provider order
            if (StaticFunctions.IsHasRihgt("Send request to provider", User.Identity.GetUserId()))
            {
                if (!this.IsMegaAdmin())
                {
                    viewModel.ProviderOrderApprovedRequests = this.requestToProviderService.GetAll()
                    .Where(x => x.IsSeenByApprover)
                    .Where(x => x.IsApproved)
                    .Where(x => !x.IsFinished)
                    .Where(x => x.FromId == this.User.Identity.GetUserId())
                    .Where(x => x.From.Site.OrganisationId == userOrg)
                    .Count();
                }
                else
                {
                    viewModel.ProviderOrderApprovedRequests = this.requestToProviderService.GetAll()
                    .Where(x => x.IsSeenByApprover)
                    .Where(x => x.IsApproved)
                    .Where(x => !x.IsFinished)
                    .Where(x => x.FromId == this.User.Identity.GetUserId())
                    .Count();
                }
            }

            //Get count of scrapping request for approving
            if (StaticFunctions.IsHasRihgt("Approve request for scrapping", User.Identity.GetUserId()))
            {
                if (!this.IsMegaAdmin())
                {
                    viewModel.ScrappingRequests = this.requestForScrappingService.GetAll()
                    .Where(x => !x.IsApproved)
                    .Where(x => !x.IsFinished)
                    .Where(x => x.From.Site.OrganisationId == userOrg)
                    .Count();
                }
                else
                {
                    viewModel.ScrappingRequests = this.requestForScrappingService.GetAll()
                    .Where(x => !x.IsApproved)
                    .Where(x => !x.IsFinished)
                    .Count();
                }
            }

            //Get count of relocation request for approving
            if (StaticFunctions.IsHasRihgt("Approve request for relocation", User.Identity.GetUserId()))
            {
                if (!this.IsMegaAdmin())
                {
                    viewModel.RelocationRequestsForApproving = this.requestForRelocationService.GetAll()
                    .Where(x => !x.SeenFromApprover)
                    .Where(x => x.From.Site.OrganisationId == userOrg)
                    .Where(x => !x.IsFinished)
                    .Count();
                }
                else
                {
                    viewModel.RelocationRequestsForApproving = this.requestForRelocationService.GetAll()
                   .Where(x => !x.SeenFromApprover)
                   .Where(x => !x.IsFinished)
                   .Count();
                }
            }

            //Get count of your relocation requests for issue
            //if (StaticFunctions.IsHasRihgt("Send request for relocation", User.Identity.GetUserId()))
            //{
            if (!this.IsMegaAdmin())
            {
                viewModel.RelocationsForIssue = this.requestForRelocationService.GetAll()
                .Where(x => !x.IsFinished)
                .Where(x => x.IsApproved)
                .Where(x => x.SeenFromApprover)
                .Where(x => x.PackingSlip.FromUser == null)
                .Where(x => x.OldUserId != null)
                .Where(x => x.OldUserId == this.User.Identity.GetUserId())
                .Count();
            }
            else
            {
                viewModel.RelocationsForIssue = this.requestForRelocationService.GetAll()
               .Where(x => !x.IsFinished)
               .Where(x => x.IsApproved)
               .Where(x => x.SeenFromApprover)
               .Where(x => x.PackingSlip.FromUser == null)
               .Where(x => x.OldUserId != null)
                .Where(x => x.OldUserId == this.User.Identity.GetUserId())
               .Count();
            }
            //}

            //Get count of all relocation requests for isuue
            if (StaticFunctions.IsHasRihgt("Approve request for relocation", User.Identity.GetUserId()))
            {
                if (!this.IsMegaAdmin())
                {
                    viewModel.RelocationForIssueAll = this.requestForRelocationService.GetAll()
                    .Where(x => !x.IsFinished)
                    .Where(x => x.IsApproved)
                    .Where(x => x.SeenFromApprover)
                    .Where(x => x.PackingSlip.FromUser == null)
                    .Where(x => x.From.Site.OrganisationId == userOrg)
                    .Where(x => x.OldUserId == null)
                    .Count();
                }
                else
                {
                    viewModel.RelocationForIssueAll = this.requestForRelocationService.GetAll()
                    .Where(x => !x.IsFinished)
                    .Where(x => x.IsApproved)
                    .Where(x => x.SeenFromApprover)
                    .Where(x => x.PackingSlip.FromUser == null)
                    .Where(x => x.From.Site.OrganisationId == userOrg)
                    .Where(x => x.OldUserId == null)
                    .Count();
                }
            }

            //Get count of your relocation requests for receive
            //if (StaticFunctions.IsHasRihgt("Send request for relocation", User.Identity.GetUserId()))
            //{
            if (!this.IsMegaAdmin())
            {
                viewModel.RelocationReceive = this.requestForRelocationService.GetAll()
                .Where(x => !x.IsFinished)
                .Where(x => x.IsApproved)
                .Where(x => x.SeenFromApprover)
                .Where(x => x.PackingSlip.FromUser != null)
                .Where(x => x.ToUserId != null)
                .Where(x => x.ToUserId == this.User.Identity.GetUserId())
                .Where(x => x.From.Site.OrganisationId == userOrg)
                .Count();
            }
            else
            {
                viewModel.RelocationReceive = this.requestForRelocationService.GetAll()
               .Where(x => !x.IsFinished)
               .Where(x => x.IsApproved)
               .Where(x => x.SeenFromApprover)
               .Where(x => x.PackingSlip.FromUser != null)
               .Where(x => x.ToUserId != null)
               .Where(x => x.ToUserId == this.User.Identity.GetUserId())
               .Count();
            }
            //}


            //Get count of all relocation requests for receive
            if (StaticFunctions.IsHasRihgt("Approve request for relocation", User.Identity.GetUserId()))
            {
                if (!this.IsMegaAdmin())
                {
                    viewModel.RelocationReceiveAll = this.requestForRelocationService.GetAll()
                    .Where(x => !x.IsFinished)
                    .Where(x => x.IsApproved)
                    .Where(x => x.SeenFromApprover)
                    .Where(x => x.PackingSlip.FromUser != null)
                    .Where(x => x.From.Site.OrganisationId == userOrg)
                    .Where(x => x.ToUserId == null)
                    .Count();
                }
                else
                {
                    viewModel.RelocationReceiveAll = this.requestForRelocationService.GetAll()
                      .Where(x => !x.IsFinished)
                      .Where(x => x.IsApproved)
                      .Where(x => x.SeenFromApprover)
                      .Where(x => x.PackingSlip.FromUser != null)
                      .Where(x => x.ToUserId == null)
                      .Count();
                }
            }

            //Get count of renovation requests for approving
            if (StaticFunctions.IsHasRihgt("Approve request for renovation", User.Identity.GetUserId()))
            {
                if (!this.IsMegaAdmin())
                {
                    viewModel.RenovationRequestsForApproving = this.requestForRenovationService.GetAll()
                    .Where(x => !x.IsApproved)
                    .Where(x => !x.IsAssetGave)
                    .Where(x => !x.IsAssetRenovated)
                    .Where(x => !x.IsFinished)
                    .Where(x => !x.IsSeenFromApprover)
                    .Where(x => x.From.Site.OrganisationId == userOrg)
                    .Count();
                }
                else
                {
                    viewModel.RenovationRequestsForApproving = this.requestForRenovationService.GetAll()
                  .Where(x => !x.IsApproved)
                  .Where(x => !x.IsAssetGave)
                  .Where(x => !x.IsAssetRenovated)
                  .Where(x => !x.IsFinished)
                  .Where(x => !x.IsSeenFromApprover)
                  .Count();
                }
            }

            //Get count of renovation approved requests
            if (StaticFunctions.IsHasRihgt("Send request for renovation", User.Identity.GetUserId()))
            {
                if (!this.IsMegaAdmin())
                {
                    viewModel.RenovationApprovedRequests = this.requestForRenovationService.GetAll()
                    .Where(x => !x.IsFinished)
                    .Where(x => x.IsSeenFromApprover)
                    .Where(x => x.IsApproved)
                    .Where(x => !x.IsAssetGave)
                    .Where(x => !x.IsAssetRenovated)
                    .Where(x => x.FromId == this.User.Identity.GetUserId())
                    .Where(x => x.From.Site.OrganisationId == userOrg)
                    .Count();
                }
                else
                {
                    viewModel.RenovationApprovedRequests = this.requestForRenovationService.GetAll()
                   .Where(x => !x.IsFinished)
                   .Where(x => x.IsSeenFromApprover)
                   .Where(x => x.IsApproved)
                   .Where(x => !x.IsAssetGave)
                   .Where(x => !x.IsAssetRenovated)
                   .Count();
                }
            }

            //Get count of asset for renovating
            if (StaticFunctions.IsHasRihgt("Renovate assets", User.Identity.GetUserId()))
            {
                if (!this.IsMegaAdmin())
                {
                    viewModel.RenovationAssetForRenovating = this.requestForRenovationService.GetAll()
                    .Where(x => !x.IsFinished)
                    .Where(x => x.IsSeenFromApprover)
                    .Where(x => x.IsApproved)
                    .Where(x => x.IsAssetGave)
                    .Where(x => !x.IsAssetRenovated)
                    .Where(x => !x.AssetIsReturned)
                    .Where(x => x.From.Site.OrganisationId == userOrg)
                    .Count();
                }
                else
                {
                    viewModel.RenovationAssetForRenovating = this.requestForRenovationService.GetAll()
                    .Where(x => !x.IsFinished)
                    .Where(x => x.IsSeenFromApprover)
                    .Where(x => x.IsApproved)
                    .Where(x => x.IsAssetGave)
                    .Where(x => !x.IsAssetRenovated)
                    .Where(x => !x.AssetIsReturned)
                    .Count();
                }
            }

            //Get count of returned assets
            if (StaticFunctions.IsHasRihgt("Send request for renovation", User.Identity.GetUserId()))
            {
                if (!this.IsMegaAdmin())
                {
                    viewModel.RenovationReturnedAssets = this.requestForRenovationService.GetAll()
                    .Where(x => !x.IsFinished)
                    .Where(x => x.IsSeenFromApprover)
                    .Where(x => x.IsAssetGave)
                    .Where(x => x.FromId == this.User.Identity.GetUserId())
                    .Where(x => x.AssetIsReturned)
                    .Where(x => x.From.Site.OrganisationId == userOrg)
                    .Count();
                }
                else
                {
                    viewModel.RenovationReturnedAssets = this.requestForRenovationService.GetAll()
                  .Where(x => !x.IsFinished)
                  .Where(x => x.IsSeenFromApprover)
                  .Where(x => x.IsAssetGave)
                  .Where(x => x.FromId == this.User.Identity.GetUserId())
                  .Where(x => x.AssetIsReturned)
                  .Count();
                }
            }

            //Get count of invoices for approving
            if (StaticFunctions.IsHasRihgt("Approve invoice", User.Identity.GetUserId()))
            {
                if (!this.IsMegaAdmin())
                {
                    viewModel.InvoicesForApproving = this.invoiceService.GetAll()
                    .Where(x => !x.IsSeenByApproved)
                    .Where(x => x.CompiledUser.Site.OrganisationId == userOrg)
                    .Count();
                }
                else
                {
                    viewModel.InvoicesForApproving = this.invoiceService.GetAll()
                    .Where(x => !x.IsSeenByApproved)
                    .Count();
                }
            }

            //Get count of invoices for paying
            if (StaticFunctions.IsHasRihgt("Pay invoice", User.Identity.GetUserId()))
            {
                if (!this.IsMegaAdmin())
                {
                    viewModel.InvoicesForPaid = this.invoiceService.GetAll()
                    .Where(x => (!x.IsPaid) && (x.IsApproved))
                    .Where(x => x.CompiledUser.Site.OrganisationId == userOrg)
                    .Count();
                }
                else
                {
                    viewModel.InvoicesForPaid = this.invoiceService.GetAll()
                  .Where(x => (!x.IsPaid) && (x.IsApproved))
                  .Count();
                }
            }

            //Get count of answers from accidents
            if (!this.IsMegaAdmin())
            {
                viewModel.AccidnetsAnswers = this.accidentService.GetAll()
                    .Where(x => x.IsAnswered)
                    .Where(x => x.FromId == User.Identity.GetUserId())
                    .Where(x => !x.IsSeenByUser)
                    .Where(x => x.From.Site.OrganisationId == userOrg)
                    .Count();
            }
            else
            {
                viewModel.AccidnetsAnswers = this.accidentService.GetAll()
                  .Where(x => x.IsAnswered)
                  .Where(x => x.FromId == User.Identity.GetUserId())
                  .Where(x => !x.IsSeenByUser)
                  .Count();
            }

            //Get count of accidents for answering 
            if (StaticFunctions.IsHasRihgt("Responding to incidents", User.Identity.GetUserId()))
            {
                if (!this.IsMegaAdmin())
                {
                    viewModel.AccidentsForAnswering = this.accidentService.GetAll()
                    .Where(x => x.From.Site.OrganisationId == userOrg)
                    .Where(x => !x.IsAnswered)
                    .Count();
                }
                else
                {
                    viewModel.AccidentsForAnswering = this.accidentService.GetAll()
                      .Where(x => !x.IsAnswered)
                      .Count();
                }
            }
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }


    }
}