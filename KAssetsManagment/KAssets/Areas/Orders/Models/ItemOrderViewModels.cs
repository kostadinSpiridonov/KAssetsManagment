using KAssets.Areas.HelpModule.Models;
using KAssets.Areas.Items.Models;
using KAssets.Models;
using KAssets.Resources.Translation;
using KAssets.Resources.Translation.AssetsTr;
using KAssets.Resources.Translation.InvoiceTr;
using KAssets.Resources.Translation.OrdersTr.AssetOrderTr;
using KAssets.Resources.Translation.OrdersTr.ItemOrderTr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KAssets.Areas.Orders.Models
{
    public class AddItemAcquisitionRequestViewModel
    {
        [Required(
            ErrorMessageResourceName="ItemsAreRequired",
            ErrorMessageResourceType=typeof(ItemOrderTr))]
        [Display(Name = "Items",
            ResourceType = typeof(Common))]
        public List<ItemSelectViewModel> Items { get; set; }

         [Display(Name = "Location",
            ResourceType = typeof(Common))]
        public List<DropDownLocationViewModel> Locations { get; set; }

        [DenyHtml]
        public string SelectedLocation { get; set; }

        public List<UserLViewModel> Users { get; set; }

        [DenyHtml]
        public string SelectedUser { get; set; }

        public int RequestId { get; set; }
    }

    public class ItemSelectViewModel
    {
        public int Id { get; set; }
         
        [Display(Name = "Count",
            ResourceType = typeof(Common))]
        public int Count { get; set; }
    }

    public class ListItemAcquisitionRequestViewModel
    {

        [Display(Name = "From",
            ResourceType = typeof(Common))]
        public ShortUserDetails From { get; set; }

        [Display(Name = "DateOfSend",
            ResourceType=typeof(InvoiceTr))]
        public DateTime DateOfSend { get; set; }

        public int Id { get; set; }
    }

    public class ViewItemAcquisitionRequestViewModel
    {
        [Display(Name = "From",
            ResourceType = typeof(Common))]
        public ShortUserDetails From { get; set; }

        [Display(Name = "DateOfSend",
            ResourceType = typeof(InvoiceTr))]
        public DateTime DateOfSend { get; set; }

        public int Id { get; set; }

        [Display(Name = "Items",
            ResourceType = typeof(Common))]
        public List<ItemViewModel> Items { get; set; }

         [Display(Name = "Location",
            ResourceType = typeof(Common))]
        public string LocationName { get; set; }

        public bool IsFinished { get; set; }

         [Display(Name = "Site",
            ResourceType = typeof(Common))]
        public string SiteName { get; set; }

         [Display(Name = "ForUser",
            ResourceType = typeof(ItemOrderTr))]
        public string ForUser { get; set; }

        public int PackingSlipId { get; set; }
    }

    public class SendOffersOfRequestViewModel
    {
        public int RequestId { get; set; }

         [Display(Name = "Offers",
            ResourceType = typeof(ItemOrderTr))]
        public List<OfferItemViewModel> Offers { get; set; }

        [Display(Name = "Items",
            ResourceType = typeof(Common))]
        public List<string> ItemsNames { get; set; }

        [Display(Name = "Request",
            ResourceType = typeof(ItemOrderTr))]
        public ViewItemAcquisitionRequestViewModel Request { get; set; }

        [Display(Name = "Site",
            ResourceType = typeof(Common))]
        public string SiteName { get; set; }
    }

    public class RequestWithSelectedOffersViewModel
    {
        [Display(Name = "Request",
            ResourceType = typeof(ItemOrderTr))]
        public ViewItemAcquisitionRequestViewModel Request { get; set; }

        [Display(Name = "SendOffers",
            ResourceType = typeof(ItemOrderTr))]
        public List<OfferItemViewModel> ApprovedOffers { get; set; }

        [Display(Name = "SelectedOffers",
            ResourceType = typeof(ItemOrderTr))]
        public List<OfferItemViewModel> SelectedOffers { get; set; }

        [Display(Name = "GaveItems",
            ResourceType = typeof(ItemOrderTr))]
        public List<OfferItemViewModel> GaveItems { get; set; }

        [Display(Name = "IsApproved",
            ResourceType = typeof(AssetOrderTr))]
        public bool IsApproved { get; set; }

        [Display(Name = "AreItemsGave",
            ResourceType = typeof(ItemOrderTr))]
        public bool AreItemGave { get; set; }

        [Display(Name = "Message",
            ResourceType = typeof(ItemOrderTr))]
        public string Message { get; set; }

    }

    public class AddAssetViewModel
    {

        [Required
         (ErrorMessageResourceName = "TypeIsRequired",
          ErrorMessageResourceType = typeof(AssetTr))]
        [Display(
        Name = "Type",
        ResourceType = typeof(AssetTr))]
        public virtual TypesOfAsset Type { get; set; }


        [Required]
        public int ItemId { get; set; }
        public virtual ItemDetailsViewModel Item { get; set; }


        [Required
         (ErrorMessageResourceName = "InventoryNumberIsRequired",
          ErrorMessageResourceType = typeof(AssetTr))]
        [Display(
        Name = "InventoryNumber",
        ResourceType = typeof(AssetTr))]
        [DenyHtml]
        public string InventoryNumber { get; set; }

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
        public int RequestId { get; set; }
    }

    public class AddAssetFullViewModel
    {

        [Required
         (ErrorMessageResourceName = "TypeIsRequired",
          ErrorMessageResourceType = typeof(AssetTr))]
        [Display(
        Name = "Type",
        ResourceType = typeof(AssetTr))]
        public virtual TypesOfAsset Type { get; set; }


        [Required
         (ErrorMessageResourceName = "InventoryNumberIsRequired",
          ErrorMessageResourceType = typeof(AssetTr))]
        [Display(
        Name = "InventoryNumber",
        ResourceType = typeof(AssetTr))]
        [DenyHtml]
        public string InventoryNumber { get; set; }

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

        [Display(
        Name = "Location",
        ResourceType = typeof(Common))]
        [DenyHtml]
        public string LocationId { get; set; }


        [Required
         (ErrorMessageResourceName = "SiteIsRequired",
          ErrorMessageResourceType = typeof(AssetTr))]
        [Display(
        Name = "Site",
        ResourceType = typeof(Common))]
        public int SiteId { get; set; }

        [Display(
        Name = "User",
        ResourceType = typeof(Common))]
        [DenyHtml]
        public string UserId { get; set; }

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