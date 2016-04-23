using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KAssets.Controllers
{
    public class PackingSlipController : BaseController
    {
        // GET: get packing slip details
        [HttpGet]
        public ActionResult GetPartialDetails(int id)
        {
            var packingSlip = this.packingSlipService.GetById(id);

            var viewModel = new PackingSlipViewModel
            {
                DateOfGiven = packingSlip.DateOfGiven.ToString(),
                DateOfReceived = packingSlip.DateOfReceived.ToString(),
                FromName = packingSlip.FromUser != null ? (packingSlip.FromUser.FirstName != null) ?
                    packingSlip.FromUser.FirstName + " " + packingSlip.FromUser.SecondName + " " + packingSlip.FromUser.LastName :
                    packingSlip.FromUser.Email : null,
                ToName = packingSlip.ToUser != null ? (packingSlip.ToUser.FirstName != null) ?
                                   packingSlip.ToUser.FirstName + " " + packingSlip.ToUser.SecondName + " " + packingSlip.ToUser.LastName :
                                   packingSlip.ToUser.Email : null,
                IsGiven = packingSlip.Given,
                IsReceived = packingSlip.Received
            };
            return View(viewModel);
        }
    }
}