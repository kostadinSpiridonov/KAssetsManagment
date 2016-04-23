using KAssets.Areas.HelpModule.Models;
using KAssets.Models;
using KAssets.Resources.Translation;
using KAssets.Resources.Translation.AssetsTr;
using KAssets.Resources.Translation.ScrappingTr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KAssets.Areas.AssetsActions.Models
{
    public class SendRequestForScrappingViewModel
    {
        [Display(Name = "InventoryNumber",
            ResourceType = typeof(AssetTr))]
        [DenyHtml]
        public string InventoryNumber { get; set; }

        [Display(Name = "Location",
            ResourceType = typeof(Common))]
        public LocationViewModel Location { get; set; }

        [Display(Name = "Guarantee",
            ResourceType = typeof(AssetTr))]
        public int Guarantee { get; set; }


        [Display(Name = "DateOfManufacture",
            ResourceType = typeof(ScrappingTr))]
        public virtual DateTime DateOfManufacture { get; set; }

        [Display(Name = "Producer",
            ResourceType = typeof(AssetTr))]
        public string Producer { get; set; }

        [Display(Name = "Brand",
            ResourceType = typeof(AssetTr))]
        public string Brand { get; set; }


        [Display(Name = "Model",
            ResourceType = typeof(AssetTr))]
        public string ItemModel { get; set; }

        [Display(Name = "Price",
            ResourceType = typeof(AssetTr))]
        public double Price { get; set; }

        [Display(Name = "Site",
            ResourceType = typeof(Common))]
        public string SiteName { get; set; }

        [Display(Name = "From",
            ResourceType = typeof(ScrappingTr))]
        public string FromName { get; set; }

        [DenyHtml]
        public string FromId { get; set; }

        [Display(Name = "Type",
            ResourceType = typeof(AssetTr))]
        public string Type { get; set; }

        public int Id { get; set; }

        [Display(Name = "Currency",
            ResourceType = typeof(Common))]
        public string Currency { get; set; }
    }

    public class ScrappingHistoryRequest
    {
        [Display(Name = "InventoryNumber",
            ResourceType = typeof(AssetTr))]
        public string InventoryNumber { get; set; }

        [Display(Name = "Location",
            ResourceType = typeof(Common))]
        public LocationViewModel Location { get; set; }

        [Display(Name = "Guarantee",
            ResourceType = typeof(AssetTr))]
        public int Guarantee { get; set; }

        [Display(Name = "DateOfManufacture",
            ResourceType = typeof(ScrappingTr))]
        public virtual DateTime DateOfManufacture { get; set; }

        [Display(Name = "Producer",
            ResourceType = typeof(AssetTr))]
        public string Producer { get; set; }

        [Display(Name = "Brand",
            ResourceType = typeof(AssetTr))]
        public string Brand { get; set; }

        [Display(Name = "Model",
            ResourceType = typeof(AssetTr))]
        public string ItemModel { get; set; }

        [Display(Name = "Price",
            ResourceType = typeof(AssetTr))]
        public double Price { get; set; }

        [Display(Name = "Site",
            ResourceType = typeof(Common))]
        public string SiteName { get; set; }

        [Display(Name = "From",
            ResourceType = typeof(ScrappingTr))]
        public string FromName { get; set; }

        public string FromId { get; set; }

        [Display(Name = "Type",
            ResourceType = typeof(AssetTr))]
        public string Type { get; set; }

        public int Id { get; set; }

        [Display(Name = "IsApproved",
            ResourceType = typeof(ScrappingTr))]
        public bool IsApproved { get; set; }

        [Display(Name = "Currency",
            ResourceType = typeof(Common))]
        public string Currency { get; set; }
    }
}