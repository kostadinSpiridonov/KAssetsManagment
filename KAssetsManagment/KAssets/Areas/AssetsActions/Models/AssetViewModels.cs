using KAssets.Areas.HelpModule.Models;
using KAssets.Models;
using KAssets.Resources.Translation;
using KAssets.Resources.Translation.AssetsTr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KAssets.Areas.AssetsActions.Models
{
    public class AssetViewModel
    {
        [Display(
         Name = "InventoryNumber",
         ResourceType = typeof(AssetTr))]
        [DenyHtml]
        public string InventoryNumber { get; set; }

        [Display(
         Name = "Brand",
         ResourceType = typeof(AssetTr))]
        public string Brand { get; set; }

        [Display(
         Name = "Model",
         ResourceType = typeof(AssetTr))]
        [DenyHtml]
        public string AssetModel { get; set; }

        public bool IsInYourSite { get; set; }

        [Display(
         Name = "SiteName",
         ResourceType = typeof(AssetTr))]
        [DenyHtml]
        public string SiteName { get; set; }

        [Display(
         Name = "Price",
         ResourceType = typeof(AssetTr))]
        public double Price { get; set; }

        [Display(
         Name = "Currency",
         ResourceType = typeof(Common))]
        public string Currency { get; set; }

        public double CurrencyCourse { get; set; }
    }

    public class AssetDetailsViewModel
    {
        [Display(
         Name = "InventoryNumber",
         ResourceType = typeof(AssetTr))]
        public string InventoryNumber { get; set; }

        [Display(
         Name = "Type",
         ResourceType = typeof(AssetTr))]
        public TypesOfAsset Type { get; set; }

        [Display(
         Name = "Location",
         ResourceType = typeof(Common))]
        public LocationViewModel Location { get; set; }

        [Display(
         Name = "Guarantee",
         ResourceType = typeof(AssetTr))]
        public int Guarantee { get; set; }

        [Display(
         Name = "DateOfManufacture",
         ResourceType = typeof(AssetTr))]
        public virtual DateTime DateOfManufacture { get; set; }

        [Display(
         Name = "Producer",
         ResourceType = typeof(AssetTr))]
        public string Producer { get; set; }

        [Display(
         Name = "Brand",
         ResourceType = typeof(AssetTr))]
        public string Brand { get; set; }

        [Display(
         Name = "Model",
         ResourceType = typeof(AssetTr))]
        public string ItemModel { get; set; }

        [Display(
         Name = "Price",
         ResourceType = typeof(AssetTr))]
        public double Price { get; set; }

        [Display(
         Name = "SiteName",
         ResourceType = typeof(AssetTr))]
        public string SiteName { get; set; }

        [Display(
         Name = "User",
         ResourceType = typeof(Common))]
        public string UserName { get; set; }

        [Display(
         Name = "Status",
         ResourceType = typeof(Common))]
        public string Status { get; set; }


        [Display(
         Name = "Currency",
         ResourceType = typeof(Common))]
        public string Currency { get; set; }

    }

    public class EditAssetViewModel
    {
        [Required]
        [Display(
            Name = "InventoryNumber",
            ResourceType = typeof(AssetTr))]
        [DenyHtml]
        public string InventoryNumber { get; set; }

        [Required
        (ErrorMessageResourceName = "TypeIsRequired",
         ErrorMessageResourceType = typeof(AssetTr))]
        [Display(
        Name = "Type",
        ResourceType = typeof(AssetTr))]
        public TypesOfAsset Type { get; set; }


        [Display(Name = "Location")]
        [DenyHtml]
        public string SelectedLocation { get; set; }

        public List<DropDownLocationViewModel> Locations { get; set; }

        [Required
      (ErrorMessageResourceName = "Guarantee",
       ErrorMessageResourceType = typeof(AssetTr))]
        [Range(0, double.MaxValue,
            ErrorMessageResourceName = "GuaranteeRangeError",
            ErrorMessageResourceType = typeof(AssetTr))]
        [Display(
        Name = "Guarantee",
        ResourceType = typeof(AssetTr))]
        public int Guarantee { get; set; }

        [Required]
        [Display(Name = "Date of manufacture")]
        public DateTime DateOfManufacture { get; set; }

        [Required
        (ErrorMessageResourceName = "ProducerIsRequired",
         ErrorMessageResourceType = typeof(AssetTr))]
        [DenyHtml]
        [Display(
        Name = "Producer",
        ResourceType = typeof(AssetTr))]
        public string Producer { get; set; }

        [Required
          (ErrorMessageResourceName = "BrandIsRequired",
           ErrorMessageResourceType = typeof(AssetTr))]
        [DenyHtml]
        [Display(
        Name = "Brand",
        ResourceType = typeof(AssetTr))]
        public string Brand { get; set; }

        [Required
          (ErrorMessageResourceName = "ModelIsRequired",
           ErrorMessageResourceType = typeof(AssetTr))]
        [DenyHtml]
        [Display(
        Name = "Model",
        ResourceType = typeof(AssetTr))]
        public string ItemModel { get; set; }

        [Required
        (ErrorMessageResourceName = "PriceIsRequired",
         ErrorMessageResourceType = typeof(AssetTr))]
        [Range(0.00000001, double.MaxValue,
            ErrorMessageResourceName = "PriceRangeError",
            ErrorMessageResourceType = typeof(AssetTr))]
        [Display(
        Name = "Price",
        ResourceType = typeof(AssetTr))]
        public double Price { get; set; }

        [Required]
        [Display(
            Name = "Site",
            ResourceType = typeof(Common))]
        [DenyHtml]
        public string SiteName { get; set; }

        [Display(
        Name = "Currency",
        ResourceType = typeof(Common))]
        public List<CurrencyViewModel> Currency { get; set; }

        [Required
        (ErrorMessageResourceName = "CurrencyIsRequired",
         ErrorMessageResourceType = typeof(AssetTr))]
        public int SelectedCurrency { get; set; }


    }
}