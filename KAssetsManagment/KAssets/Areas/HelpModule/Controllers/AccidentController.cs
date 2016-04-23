using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KAssets.Models;
using KAssets.Filters;
using KAssets.Resources.Translation.AccidentTr;
using KAssets.Controllers;
using KAssets.Areas.HelpModule.Models;

namespace KAssets.Areas.HelpModule.Controllers
{
    [Authorize]
    [HasSite]
    public class AccidentController : BaseController
    {
        //GET: Add a new accident
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        //POST: Add a new accident
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddAccidentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Add accident to database
            this.accidentService.Add(
                 new Accident
                 {
                     DateOfSend = DateTime.Now,
                     FromId = this.User.Identity.GetUserId(),
                     Message = model.Message
                 });

            //Add event that is added new accident
            this.eventService.AddForUserGroup(new Event
                {
                    Date = DateTime.Now,
                    Content = "You have new accident to answer !",
                    EventRelocationUrl = "/HelpModule/Accident/ToAnswer"
                }, 
                "Responding to incidents",
                this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()));

            return Redirect("/HelpModule/Accident/SuccessfullySend");
        }

        //GET: Successfully send page
        [HttpGet]
        public ActionResult SuccessfullySend()
        {
            return View();
        }

        // GET: Get accidents with answer
        [HttpGet]
        public ActionResult GetAnswers()
        {
            var accidents = this.accidentService.GetAll()
                      .Where(x => x.IsAnswered)
                      .Reverse();

            if (!this.IsMegaAdmin())
            {
                var userId = this.User.Identity.GetUserId();
                accidents = accidents.Where(x => x.FromId == userId);
            }

            var viewModel = accidents.ToList()
                .ConvertAll(x => new ListAccidentViewModel
                {
                    Date = x.ReplyingDate.ToString(),
                    Id = x.Id,
                    From = (x.From.FirstName + x.From.SecondName + x.From.LastName) == "" ?
                            x.From.Email : x.From.FirstName + " " + x.From.SecondName + " " + x.From.LastName,
                    IsSeen = x.IsSeenByUser
                });

            return View(viewModel);
        }

        //GET: View accident with answer
        [HttpGet]
        public ActionResult ViewAccidentWithAnswer(int id)
        {
            var accident = this.accidentService.GetById(id);

            if (!this.IsMegaAdmin())
            {
                if (accident.FromId != this.User.Identity.GetUserId())
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            var viewModel = new ViewAccidentViewModel
            {
                DateOfSend = accident.DateOfSend.ToString(),
                From = (accident.From.FirstName + accident.From.SecondName + accident.From.LastName) == "" ?
                            accident.From.Email : accident.From.FirstName + " " + accident.From.SecondName + " " + accident.From.LastName,
                Id = accident.Id,
                Message = accident.Message,
                Answer = accident.Answer,
                DateOfRaply = accident.ReplyingDate.ToString()
            };

            //Set accident is seen 
            this.accidentService.SetSeenByUser(id);

            return View(viewModel);
        }

        //GET: Get accidents to answer
        [HttpGet]
        [RightCheck(Right = "Responding to incidents")]
        public ActionResult ToAnswer()
        {
            var accidents = this.accidentService.GetAll()
                .Where(x => !x.IsAnswered);

            if (!this.IsMegaAdmin())
            {
                accidents = accidents.Where(x => x.From.Site.OrganisationId ==
                    this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()));
            }

            var viewModel = accidents.ToList()
                .ConvertAll(x => new ListAccidentViewModel
                {
                    Date = x.DateOfSend.ToString(),
                    From = (x.From.FirstName + x.From.SecondName + x.From.LastName) == "" ?
                            x.From.Email : x.From.FirstName + " " + x.From.SecondName + " " + x.From.LastName,
                    Id = x.Id
                });

            return View(viewModel);
        }

        //GET: View accidetn to answer
        [HttpGet]
        [RightCheck(Right = "Responding to incidents")]
        public ActionResult ViewAccidentToAnswer(int id)
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
                DateOfSend = accident.DateOfSend.ToString(),
                From = (accident.From.FirstName + accident.From.SecondName + accident.From.LastName) == "" ?
                            accident.From.Email : accident.From.FirstName + " " + accident.From.SecondName + " " + accident.From.LastName,
                Id = accident.Id,
                Message = accident.Message
            };

            return View(viewModel);
        }

        //POST: Set answer to certain accident
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RightCheck(Right = "Responding to incidents")]
        public ActionResult SetAnswer(SetAnswerViewModel model)
        {
            //Verify if the accident is from user organisation
            if (!this.IsMegaAdmin())
            {
                var accident = this.accidentService.GetById(model.Id);
                if (accident.From.Site.OrganisationId != this.userService.GetUserOrganisationId(this.User.Identity.GetUserId()))
                {
                    return Redirect("/Home/NotAuthorized");
                }
            }

            if (!ModelState.IsValid)
            {
                this.TempData["DellError"] = AccidentTr.AnswerFieldIsRequired;
                return Redirect("/HelpModule/Accident/ViewAccidentToAnswer/" + model.Id);
            }

            this.accidentService.SetAnswer(model.Id, model.Answer, DateTime.Now);

            //Add a new event that is added an answer to certain accident
            this.eventService.Add(new Event
                {
                    Content = "You have new answer from accident !",
                    Date = DateTime.Now,
                    UserId = this.accidentService.GetById(model.Id).FromId,
                    EventRelocationUrl = "/HelpModule/Accident/GetAnswers"
                });

            return Redirect("/HelpModule/Accident/SuccessfullySend");
        }
    }
}