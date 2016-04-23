using KAssets.Resources.Translation;
using KAssets.Resources.Translation.OrdersTr.AssetOrderTr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KAssets.Models
{

    public class DeclineRequestViewModel
    {
        public int RequestId { get; set; }

        [Required(ErrorMessageResourceName = "MessageIsRequired",
            ErrorMessageResourceType = typeof(AssetOrderTr))]
        [DenyHtml]
        [Display(Name = "Message",
            ResourceType = typeof(Common))]
        public string Message { get; set; }
    }
}