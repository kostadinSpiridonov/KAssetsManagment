using KAssets.Areas.HelpModule.Models;
using KAssets.Areas.Items.Models;
using KAssets.Models;
using KAssets.Resources.Translation;
using KAssets.Resources.Translation.AssetsTr;
using KAssets.Resources.Translation.InvoiceTr;
using KAssets.Resources.Translation.ItemTr;
using KAssets.Resources.Translation.OrdersTr.ProviderOrder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KAssets.Areas.Orders.Models
{
    public class CreateProviderRequestViewModel
    {
        [Required(
            ErrorMessageResourceName="ProviderIsRequired",
            ErrorMessageResourceType=typeof(ProviderOrderTr))]
        [Display(
         Name = "Provider",
         ResourceType = typeof(Common))]
        public int Provider { get; set; }

        [Required(
            ErrorMessageResourceName = "ContentIsRequired",
            ErrorMessageResourceType = typeof(ProviderOrderTr))]
        [DenyHtml]
        [Display(
         Name = "Content",
         ResourceType = typeof(Common))]
        public string Content { get; set; }

        [Required(
            ErrorMessageResourceName = "SubjectIsRequired",
            ErrorMessageResourceType = typeof(ProviderOrderTr))]
        [DenyHtml]
        [Display(
         Name = "Subject",
         ResourceType = typeof(ProviderOrderTr))]
        public string Subject { get; set; }

        public List<ItemsEmailViewModel> ItemsAndCount { get; set; }
    }

    public class ProviderOfferViewModel
    {
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

        [Required(
          ErrorMessageResourceName = "QuantityIsRequired",
          ErrorMessageResourceType = typeof(ItemTr))]
        [Range(1, double.MaxValue,
            ErrorMessageResourceName = "PositiveNumError",
            ErrorMessageResourceType = typeof(ItemTr))]
        [Display(Name = "Quantity",
            ResourceType = typeof(ItemTr))]
        public int Quantity { get; set; }

        [Display(Name = "Currency",
            ResourceType = typeof(ItemTr))]
        public List<CurrencyViewModel> Currency { get; set; }

        [Required]
        [Display(Name = "Currency",
            ResourceType = typeof(ItemTr))]
        public int SelectedCurrency { get; set; }

        [Display(Name = "Currency",
            ResourceType = typeof(ItemTr))]
        public string CurrencyNotaion { get; set; }

        public bool IsSelected { get; set; }

        public int Id { get; set; }
    }

    public class AddProviderToRequestViewModel
    {
        public List<ProviderOfferViewModel> Offers { get; set; }

        [DenyHtml]
        [Display(Name = "ProviderEmail",
            ResourceType = typeof(ProviderOrderTr))]
        public string ProviderEmail { get; set; }

        [Required(
            ErrorMessageResourceName="OrderIdIsRequired",
            ErrorMessageResourceType=typeof(ProviderOrderTr))]
        [Display(Name = "OrderId",
            ResourceType = typeof(ProviderOrderTr))]
        public int PoId { get; set; }
    }

    public class RequestToProviderViewModel
    {
        public int Id { get; set; }

        [Display(Name = "From",
            ResourceType = typeof(InvoiceTr))]
        public string FromUser { get; set; }

        [Display(Name = "Provider",
            ResourceType = typeof(InvoiceTr))]
        public string Provider { get; set; }

        [Display(Name = "DateOfSend",
            ResourceType = typeof(InvoiceTr))]
        public string DateOfSend { get; set; }
    }

    public class RequestToProviderFullViewModel
    {
        [Display(Name = "Offers",
            ResourceType = typeof(InvoiceTr))]
        public List<ProviderOfferViewModel> Offers { get; set; }


        [Display(Name = "WantItems",
            ResourceType = typeof(InvoiceTr))]
        public List<ProviderOfferViewModel> WantItems { get; set; }


        [Display(Name = "GiveItems",
            ResourceType = typeof(InvoiceTr))]
        public List<ProviderOfferViewModel> GiveItems { get; set; }

        [Display(Name = "ProviderEmail",
            ResourceType = typeof(ProviderOrderTr))]
        public string ProviderEmail { get; set; }

        [Display(Name = "From",
            ResourceType = typeof(InvoiceTr))]
        public string FromName { get; set; }

        [Display(Name = "DateOfSend",
            ResourceType = typeof(InvoiceTr))]
        public string DateOfSend { get; set; }

        public int Id { get; set; }

        [Display(Name = "SentEmail",
            ResourceType = typeof(InvoiceTr))]
        public string SentEmail { get; set; }
    }

    public class RequestToProviderInvoiceModel
    {
        [Display(Name = "Offers",
            ResourceType = typeof(ProviderOrderTr))]
        public List<ItemViewModel> Offers { get; set; }

        [Display(Name = "Provider",
            ResourceType = typeof(InvoiceTr))]
        public string ProviderEmail { get; set; }

        [Display(Name = "From",
            ResourceType = typeof(InvoiceTr))]
        public string FromName { get; set; }

        [Display(Name = "DateOfSend",
            ResourceType = typeof(InvoiceTr))]
        public string DateOfSend { get; set; }

        public int Id { get; set; }

        [Display(Name = "SentEmail",
            ResourceType = typeof(InvoiceTr))]
        public string SentEmail { get; set; }

        public int ProviderId { get; set; }
    }

    public class ViewSentRequestToProviderViewModel
    {
        [Display(Name = "DateOfSend",
            ResourceType = typeof(ProviderOrderTr))]
        public string DateOfSend { get; set; }

        [Display(Name = "From",
            ResourceType = typeof(ProviderOrderTr))]
        public string FromName { get; set; }

        public int Id { get; set; }

        [Display(Name = "WantItems",
            ResourceType = typeof(ProviderOrderTr))]
        public List<ItemViewModel> WantItems { get; set; }

        [Display(Name = "Subject",
            ResourceType = typeof(ProviderOrderTr))]
        public string EmailSubject { get; set; }

        [Display(Name = "Body",
            ResourceType = typeof(ProviderOrderTr))]
        public string EmailBody { get; set; }

        [Display(Name = "To",
            ResourceType = typeof(ProviderOrderTr))]
        public string To { get; set; }
    }

    public class AddItemsFromRequest
    {
        [Display(Name = "Items",
            ResourceType = typeof(ProviderOrderTr))]
        public List<AddItemViewModel> Items { get; set; }

        [Display(Name = "OrderId",
            ResourceType=typeof(ProviderOrderTr))]
        [Required(
            ErrorMessageResourceName="OrderIdIsRequired",
            ErrorMessageResourceType=typeof(ProviderOrderTr))]
        public int PoId { get; set; }
    }
}