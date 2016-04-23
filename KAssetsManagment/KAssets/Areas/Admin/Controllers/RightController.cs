using KAssets.Areas.Admin.Models;
using KAssets.Controllers;
using KAssets.Filters;
using KAssets.Models;
using KAssets.Resources;
using KAssets.Resources.Translation.AdminArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KAssets.Areas.Admin.Controllers
{
    [RightCheck(Right = "Low admin")]
    public class RightController : BaseController
    {
        // GET: Get all rights
        [HttpGet]
        public ActionResult GetAll()
        {
            var rights = this.rightService.GetAll();

            var viewModel = rights.ToList().ConvertAll(
                x =>
                new RightViewModel
                {
                    Id = x.Id,
                    Name = Rights.ResourceManager.GetString("r"+x.Code.ToString()),
                    Desciption = Rights.ResourceManager.GetString("n" + x.Code.ToString()),
                });
            return View(viewModel);
        }
    }
}