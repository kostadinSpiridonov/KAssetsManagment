using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KAssets.Resources.Translation.AssetsTr;
using KAssets.Resources.Translation.InvoiceTr;
using KAssets.Resources.Translation.RelocationTr;
using KAssets.Resources.Translation;
using KAssets.Resources.Translation.RenovationTr;
using KAssets.Models;
using KAssets.Areas.HelpModule.Models;

namespace KAssets.Areas.AssetsActions.Models
{
    public class CreateRelocationRequestViewModel
    {
        [Required(
            ErrorMessageResourceName = "AssetIsRequired",
            ErrorMessageResourceType = typeof(RelocationTr))]
        [Display(Name = "Asset",
            ResourceType = typeof(Common))]
        [DenyHtml]
        public string SelectedAsset { get; set; }


        [Required(
            ErrorMessageResourceName = "ToSiteIsRequired",
            ErrorMessageResourceType = typeof(RelocationTr))]
        [Display(Name = "ToSite",
            ResourceType = typeof(RelocationTr))]
        [DenyHtml]
        public string ToSite { get; set; }

        [Display(Name = "ToUser",
            ResourceType = typeof(RelocationTr))]
        [DenyHtml]
        public string ToUser { get; set; }

        [Display(Name = "ToLocation",
            ResourceType = typeof(RelocationTr))]
        [DenyHtml]
        public string ToLocation { get; set; }
    }

    public class ChooseAssetViewModel
    {
        [Display(
        Name = "InventoryNumber",
        ResourceType = typeof(AssetTr))]
        public string InventoryNumber { get; set; }
        [Display(
        Name = "Brand",
        ResourceType = typeof(AssetTr))]
        public string Brand { get; set; }
        [Display(
        Name = "Model",
        ResourceType = typeof(AssetTr))]
        public string AssetModel { get; set; }

        public bool IsInYourOrganisation { get; set; }
        [Display(
        Name = "SiteName",
        ResourceType = typeof(AssetTr))]
        public string SiteName { get; set; }

        [Display(
        Name = "LocationCode",
        ResourceType = typeof(AssetTr))]
        public string LocationCode { get; set; }

        [Display(
        Name = "UserId",
        ResourceType = typeof(AssetTr))]
        public string UserId { get; set; }

    }

    public class RequestForRelocationViewModel
    {
        public int Id { get; set; }

        [Display(
        Name = "From",
        ResourceType = typeof(RelocationTr))]
        public string FromName { get; set; }

        [Display(
        Name = "InventoryNumber",
        ResourceType = typeof(AssetTr))]
        public string InventoryNumber { get; set; }

        [Display(Name = "DateOfSend",
            ResourceType = typeof(InvoiceTr))]
        public string DateOfSend { get; set; }
    }

    public class RequestForRelocationDetails
    {
        [Display(Name = "Asset",
             ResourceType = typeof(Common))]
        public AssetDetailsViewModel Asset { get; set; }

        [Display(
        Name = "From",
        ResourceType = typeof(RelocationTr))]
        public string FromName { get; set; }

        [Display(Name = "ToSite",
         ResourceType = typeof(RelocationTr))]
        public string ToSiteName { get; set; }

        [Display(Name = "DateOfSend",
            ResourceType = typeof(InvoiceTr))]
        public string DateOfSend { get; set; }

        public int Id { get; set; }

        [Display(Name = "ToUser",
            ResourceType = typeof(RelocationTr))]
        public string ToUser { get; set; }

        [Display(Name = "ToLocation",
            ResourceType = typeof(RelocationTr))]
        public string ToLocation { get; set; }
    }

    public class RelocationHistoryRequest
    {
        [Display(
        Name = "InventoryNumber",
        ResourceType = typeof(AssetTr))]
        public string InventoryNumber { get; set; }

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
        Name = "Site",
        ResourceType = typeof(Common))]
        public string SiteName { get; set; }

        [Display(
        Name = "From",
        ResourceType = typeof(RelocationTr))]
        public string FromName { get; set; }

        public string FromId { get; set; }

        [Display(
        Name = "Type",
        ResourceType = typeof(AssetTr))]
        public string Type { get; set; }

        public int Id { get; set; }

        [Display(
        Name = "IsApproved",
        ResourceType = typeof(RenovationTr))]
        public bool IsApproved { get; set; }

        [Display(
        Name = "ToSite",
        ResourceType = typeof(RelocationTr))]
        public string ToSite { get; set; }

        [Display(
        Name = "FromUser",
        ResourceType = typeof(RelocationTr))]
        public string FromUser { get; set; }

        [Display(
        Name = "FromLocation",
        ResourceType = typeof(RelocationTr))]
        public string FromLocation { get; set; }

        [Display(
        Name = "ToUser",
        ResourceType = typeof(RelocationTr))]
        public string ToUser { get; set; }

        [Display(
        Name = "DateOfManufacture",
        ResourceType = typeof(AssetTr))]
        public string ToLocation { get; set; }

        public int PackingSlipId { get; set; }

        [Display(
        Name = "Currency",
        ResourceType = typeof(Common))]
        public string Currency { get; set; }
    }
}