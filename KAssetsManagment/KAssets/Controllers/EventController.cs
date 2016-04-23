using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KAssets.Models;

namespace KAssets.Controllers
{
    [Authorize]
    public class EventController : BaseController
    {
        // GET: Get events for user
        [HttpGet]
        public ActionResult GetAllForUser()
        {
            var events = this.eventService.GetAllForUser(
                this.User.Identity.GetUserId()).Reverse();

            var viewModel = events.ToList().ConvertAll(
                x => new AssetEventViewModel
                {
                    Content=StaticFunctions.TranslateDynamicEvent(x.Content),
                    Date=x.Date.ToString(),
                    IsSeen=x.IsSeen,
                    RelocationUrl=x.EventRelocationUrl
                });

            //Set event are seen
            this.eventService.SetSeenForUser(this.User.Identity.GetUserId());

            return View(viewModel);
        }
    
        //GET: Get new events for user
        [HttpGet]
        public JsonResult GetNewEventsForUser()
        {
            var events = this.eventService.GetAllForUser(
                  this.User.Identity.GetUserId())
                  .Where(x=>(!x.IsSeen));

            var viewModel = events.ToList().ConvertAll(
                x => new AssetEventViewModel
                {
                    Content = StaticFunctions.TranslateDynamicEvent(x.Content),
                    RelocationUrl=x.EventRelocationUrl,
                    Id=x.Id
                });
            viewModel.Reverse();

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        //POST: Set evennt is seen
        [HttpPost]
        public JsonResult SetSeen(int id,string url)
        {
            this.eventService.SetSeen(id);
            return Json(true,JsonRequestBehavior.AllowGet);
        }
    }
}