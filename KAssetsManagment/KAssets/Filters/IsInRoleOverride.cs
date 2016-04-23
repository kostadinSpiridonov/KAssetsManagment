using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KAssets.Services;

namespace KAssets.Filters
{
    public class RightCheck : ActionFilterAttribute
    {
        public string Right { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated == true)
            {
                var userId = filterContext.HttpContext.User.Identity.GetUserId();
                var service = new RightService();

                bool tran = false;
                if (!service.IsUserHasRightById(userId, Right))
                {
                    tran = true;
                }
                if(service.IsUserHasRightById(userId,"Admin right"))
                {
                    tran = false;
                }

                if(tran)
                {
                    filterContext.Result = new RedirectResult("/Home/NotAuthorized");
                }
            }
            else
            {
                filterContext.Result = new RedirectResult("/Account/NotAuthorized");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}