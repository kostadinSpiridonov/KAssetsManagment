using KAssets.Resources.Translation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KAssets.Models
{
    public class AssetEventViewModel
    {
        [Display(
            Name="Content",
            ResourceType=typeof(Common))]
        public string Content { get; set; }

        [Display(
            Name = "Date",
            ResourceType = typeof(Common))]
        public string Date { get; set; }

        public bool IsSeen { get; set; }

        public string RelocationUrl { get; set; }

        public int Id { get; set; }
    }
}