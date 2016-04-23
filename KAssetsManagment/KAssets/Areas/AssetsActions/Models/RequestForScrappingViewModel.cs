using KAssets.Resources.Translation.ScrappingTr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KAssets.Areas.AssetsActions.Models
{
    public class RequestForScrappingViewModel
    {
        [Display(Name="DateOfSend",
            ResourceType=typeof(ScrappingTr))]
        public string DateOfSend { get; set; }

        [Display(Name = "From",
            ResourceType = typeof(ScrappingTr))]
        public string FromName { get; set; }

        [Display(Name = "AssetInventoryNumber",
            ResourceType = typeof(ScrappingTr))]
        public string AssetInvNumber { get; set; }

        public int Id { get; set; }
    }
}