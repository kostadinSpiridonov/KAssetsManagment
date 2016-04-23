using KAssets.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace KAssets.Controllers
{
    public class BaseController : Controller
    {
        public IUserService userService;
        public IRightService rightService;
        public ILocationService locationService;
        public IOrganisationService organisationService;
        public ISiteService siteService;
        public IBillService billService;
        public IItemService itemService;
        public IRequestToAcquireItemsService requestToAcquireItemService;
        public IEventService eventService;
        public IAssetService assetService;
        public IAssetHistoryService assetHistoryService;
        public IRequestForScrappingService requestForScrappingService;
        public IRequestForRelocationService requestForRelocationService;
        public IRequestForRenovationService requestForRenovationService;
        public IProviderService providerService;
        public IPackingSlipService packingSlipService;
        public IRequestForAssetService requestForAssetService;
        public IRequestToProviderService requestToProviderService;
        public ICurrencyService currencyService;
        public IInvoiceService invoiceService;
        public IExchangeRateService exchangeService;
        public IAccidentService accidentService;
        public ISecurityGroupService securityGroupService;

        public BaseController()
        {
            this.userService = new UserService();
            this.rightService = new RightService();
            this.locationService = new LocationService();
            this.organisationService = new OrganisationService();
            this.siteService = new SiteService();
            this.billService = new BillService();
            this.itemService = new ItemService();
            this.requestToAcquireItemService = new RequestToAcquireAssestService();
            this.eventService = new EventService();
            this.assetService = new AssetService();
            this.assetHistoryService = new AssetHistoryService();
            this.requestForScrappingService = new RequestForScrappingService();
            this.requestForRelocationService = new RequestForRelocationService();
            this.requestForRenovationService = new RequestForRenovationService();
            this.providerService = new ProviderService();
            this.packingSlipService = new PackingSlipService();
            this.requestForAssetService = new RequestForAssetService();
            this.requestToProviderService = new RequestToProviderService();
            this.currencyService = new CurrencyService();
            this.invoiceService = new InvoiceService();
            this.exchangeService = new ExchangeRateService();
            this.accidentService = new AccidentService();
            this.securityGroupService = new SecurityGroupService();
        }

        private readonly HashSet<string> allowedLanguages = new HashSet<string>
        {
            "en",
            "bg"
        };

        private readonly string defaultLanguage = "en";

        //Add language cookie
        protected override IAsyncResult BeginExecute(System.Web.Routing.RequestContext requestContext, AsyncCallback callback, object state)
        {
            var cultureName = "";
            var cook = requestContext.HttpContext.Request.Cookies.Get("_lang");
            if (requestContext.HttpContext.Request.Cookies["_lang"] == null)
            {
                HttpCookie cookie = new HttpCookie("_lang");
                if (cookie.Value == null)
                {
                    cookie.Expires = DateTime.Now.AddDays(1);
                    cookie.Value = "en";
                    cookie.Path = "/";
                }
                requestContext.HttpContext.Request.Cookies.Add(cookie);
            }
           
           
            cultureName = requestContext.HttpContext.Request.Cookies["_lang"].Value.ToString();

            if (!allowedLanguages.Contains(cultureName))
            {
                cultureName = defaultLanguage;
            }


            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cultureName);
            return base.BeginExecute(requestContext, callback, state);
        }

        //Remove language cookie
        protected override void EndExecute(IAsyncResult asyncResult)
        {
            Response.Cookies.Add(Request.Cookies["_lang"]);
            base.EndExecute(asyncResult);
        }

        protected bool IsMegaAdmin()
        {
            return StaticFunctions.IsHasRihgt("Admin right",this.User.Identity.GetUserId());
        }
    }
}