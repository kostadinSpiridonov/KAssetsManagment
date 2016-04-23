using KAssets.Resources.Translation.InvoiceTr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KAssets.Areas.Reports.Models
{
    public class CreateInvoicesByDateReportViewModel
    {
        [Display(
        Name = "DateOfCreation",
        ResourceType = typeof(InvoiceTr))]
        public DateTime? DateOfCreation { get; set; }

        [Display(
        Name = "DateOfApproving",
        ResourceType = typeof(InvoiceTr))]
        public DateTime? DateOfApproving { get; set; }

        [Display(
        Name = "PaymentPeriod",
        ResourceType = typeof(InvoiceTr))]
        public DateTime? PaymentPeriod { get; set; }

        [Display(
        Name = "DateOfPayment",
        ResourceType = typeof(InvoiceTr))]
        public DateTime? DateOfPayment { get; set; }

        [Display(
        Name = "Paid",
        ResourceType = typeof(InvoiceTr))]
        public bool Paid { get; set; }

        [Display(
        Name = "Approved",
        ResourceType = typeof(InvoiceTr))]
        public bool Approved { get; set; }

        [Display(
        Name = "Finished",
        ResourceType = typeof(InvoiceTr))]
        public bool Finished { get; set; }

        [Display(
        Name = "OrOrAnd",
        ResourceType = typeof(InvoiceTr))]
        public bool OrOrAnd { get; set; }
    }
}