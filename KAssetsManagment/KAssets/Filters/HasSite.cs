using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KAssets.Services;

namespace KAssets.Filters
{
    public class HasSite : ActionFilterAttribute
    {
        public string Right { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated == true)
            {
                var userId = filterContext.HttpContext.User.Identity.GetUserId();
                var userService = new UserService();
                if (userId != null)
                {
                    var user = userService.GetById(userId);
                    if (user.Site == null)
                    {

                        filterContext.Result = new RedirectResult("/Invoices/Invoice/MemberOfSite");
                    }
                }
                base.OnActionExecuting(filterContext);
            }
        }
    }
}