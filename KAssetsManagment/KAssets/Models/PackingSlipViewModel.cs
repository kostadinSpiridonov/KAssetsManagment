using KAssets.Resources.Translation.PackingSlipTr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KAssets.Models
{
    public class PackingSlipViewModel
    {
        [Display(Name="From",
            ResourceType=typeof(PackingSlipTr))]
        public string FromName { get; set; }

        [Display(Name = "IsGiven",
            ResourceType = typeof(PackingSlipTr))]
        public bool IsGiven { get; set; }

        [Display(Name = "DateOfGiven",
            ResourceType = typeof(PackingSlipTr))]
        public string DateOfGiven { get; set; }

        [Display(Name = "To",
            ResourceType = typeof(PackingSlipTr))]
        public string ToName { get; set; }

        [Display(Name = "IsReceived",
            ResourceType = typeof(PackingSlipTr))]
        public bool IsReceived { get; set; }

        [Display(Name = "DateOfReceived",
            ResourceType = typeof(PackingSlipTr))]
        public string DateOfReceived { get; set; }
    }
}