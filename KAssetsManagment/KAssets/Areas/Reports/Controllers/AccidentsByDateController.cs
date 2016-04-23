using KAssets.Areas.Reports.Models;
using KAssets.Controllers;
using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoreLinq;
using KAssets.Filters;
using Microsoft.AspNet.Identity;
using KAssets.Areas.HelpModule.Models;

namespace KAssets.Areas.Reports.Controllers
{
    [Authorize]
    [HasSite]
    [RightCheck(Right = "Report for accidents by date")]
    public class AccidentsByDateController : BaseController
    {
        // GET: Create report
        [HttpGet]
        public ActionResult CreateReport()
        {
            return View();
        }

        // GET: Result
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Result(CreateAccidentesByDateReport model)
        {
            if (model.DateOfReply == null && model.DateOfSend == null)
            {
                ModelState.AddModelError("", KAssets.Resources.Translation.ReportsArea.Reports.MustChooseAtLeastOneDate);
                return View("CreateReport", model);
            }

            if (!model.DateOfSend.HasValue)
            {
                model.DateOfSend = DateTime.MinValue;
            }

            if (!model.DateOfReply.HasValue)
            {
                model.DateOfReply = DateTime.MinValue;
            }

            var accidents = this.accidentService.GetAll();
            if (!this.IsMegaAdmin())
            {
                var userOrg = this.userService.GetUserOrganisationId(this.User.Identity.GetUserId());
                accidents = accidents.Where(x => x.From.Site.OrganisationId == userOrg).ToList();
            }

            if (model.ORorAND)
            {
                accidents = accidents.Where(x =>
                    (
                        (model.DateOfReply.HasValue ? (x.ReplyingDate.ToShortDateString() == model.DateOfReply.Value.ToShortDateString()) : (false))
                        ||
                        (model.DateOfSend.HasValue ? (x.DateOfSend.ToShortDateString() == model.DateOfSend.Value.ToShortDateString()) : (false))
                    )
                   ).ToList();
            }
            else
            {
                accidents = accidents.Where(x =>
                       (
                         (model.DateOfReply.HasValue ? (x.ReplyingDate.ToShortDateString() == model.DateOfReply.Value.ToShortDateString()) : (false))
                         &&
                         (model.DateOfSend.HasValue ? (x.DateOfSend.ToShortDateString() == model.DateOfSend.Value.ToShortDateString()) : (false))
                       )
                    ).ToList();
            }

            var viewModel = accidents.ToList()
                .ConvertAll(x => new ListAccidentViewModelReport
                {
                    DateOfReply = x.ReplyingDate == DateTime.MinValue ? " - " : x.ReplyingDate.ToString(),
                    DateOfSend = x.DateOfSend == DateTime.MinValue ? " - " : x.DateOfSend.ToString(),
                    Id = x.Id,
                    IsReply = x.IsAnswered,
                    From = (x.From.FirstName + x.From.SecondName + x.From.LastName) == "" ?
                                x.From.Email : x.From.FirstName + " " + x.From.SecondName + " " + x.From.LastName,
                });
            return View(viewModel);
        }

        //GET: View accident
        [HttpGet]
        public ActionResult ViewAccident(int id)
        {
            var accident = this.accidentService.GetById(id);

            //Verify if the accident is from user organisation
            if (!this.IsMegaAdmin())
            {
                if (accident.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }
            var viewModel = new ViewAccidentViewModel
            {
                DateOfSend = accident.DateOfSend == DateTime.MinValue ? " - " : accident.DateOfSend.ToString(),
                From = (accident.From.FirstName + accident.From.SecondName + accident.From.LastName) == "" ?
                            accident.From.Email : accident.From.FirstName + " " + accident.From.SecondName + " " + accident.From.LastName,
                Id = accident.Id,
                Message = accident.Message
            };

            if (accident.ReplyingDate != null)
            {
                viewModel.DateOfRaply = accident.ReplyingDate.ToString() == "1.1.0001 г. 0:00:00" ? "-" : accident.ReplyingDate.ToString();
            }
            return View(viewModel);
        }
    }
}