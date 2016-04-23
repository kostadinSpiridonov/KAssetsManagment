using KAssets.Areas.Orders.Models;
using KAssets.Resources.Translation;
using KAssets.Resources.Translation.InvoiceTr;
using KAssets.Resources.Translation.OrdersTr.ProviderOrder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KAssets.Models
{
    public class EmailViewModel
    {
        [Display(Name = "From",
            ResourceType = typeof(InvoiceTr))]
        public string From { get; set; }

        [Display(
         Name = "Subject",
         ResourceType = typeof(ProviderOrderTr))]
        public string Subject { get; set; }

        [Display(Name = "DateOfSend",
            ResourceType = typeof(InvoiceTr))]
        public string DateOfSend { get; set; }

        public bool Seen { get; set; }

        public long UId { get; set; }
    }

    public class EmailDetailsViewModel
    {
        [Display(Name = "From",
            ResourceType = typeof(InvoiceTr))]
        public string From { get; set; }

        [Display(Name = "To",
            ResourceType = typeof(Common))]
        public string To { get; set; }

        [Display(
         Name = "Subject",
         ResourceType = typeof(ProviderOrderTr))]
        public string Subject { get; set; }

        [Display(Name = "DateOfSend",
            ResourceType = typeof(InvoiceTr))]
        public string DateOfSend { get; set; }

        [Display(
         Name = "Content",
         ResourceType = typeof(Common))]
        public string Content { get; set; }

        public long UId { get; set; }

        public AddProviderToRequestViewModel Request { get; set; }

        [Display(Name = "Items",
            ResourceType = typeof(Common))]
        public AddItemsFromRequest Items { get; set; }
    }
}