using KAssets.Areas.Admin.Models;
using KAssets.Areas.HelpModule.Models;
using KAssets.Models;
using KAssets.Resources.Translation;
using KAssets.Resources.Translation.AssetsTr;
using KAssets.Resources.Translation.ItemTr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KAssets.Areas.Items.Models
{
    public class ShowItemsViewModel
    {
        public int MotherId { get; set; }

        public string MotherName { get; set; }

        public List<ItemViewModel> Items { get; set; }
    }

    public class ItemViewModel
    {
        public int Id { get; set; }

        [Display(Name="Brand",
            ResourceType=typeof(ItemTr))]
        public string Brand { get; set; }

        [Display(Name = "Model",
            ResourceType = typeof(ItemTr))]
        public string Model { get; set; }

        [Display(Name = "Quantity",
            ResourceType = typeof(ItemTr))]
        public int Quantity { get; set; }

        [Display(Name = "Organisation",
            ResourceType = typeof(ItemTr))]
        public string OrganisationName { get; set; }

        public bool IsInYourOrganisation { get; set; }

        [Display(Name = "Price",
            ResourceType = typeof(ItemTr))]
        public double Price { get; set; }

        [Display(Name = "Currency",
            ResourceType = typeof(ItemTr))]
        public string Currency { get; set; }

        public double CurrencyCourse { get; set; }
    }

    public class SelectItemViewModel
    {
        public int Id { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public bool IsSelected { get; set; }
    }

    public class AddItemViewModel
    {
          [Required(
            ErrorMessageResourceName = "IdIsRequired",
            ErrorMessageResourceType = typeof(ItemTr))]
        public int Id { get; set; }


        [Display(Name = "DateOfManufacture",
            ResourceType = typeof(ItemTr))]
        [Required(
          ErrorMessageResourceName = "DateOfManufactureIsRequired",
          ErrorMessageResourceType = typeof(ItemTr))]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public virtual DateTime DateOfManufacture { get; set; }

          [Required(
            ErrorMessageResourceName = "ProducerIsRequired",
            ErrorMessageResourceType = typeof(ItemTr))]
        [DenyHtml]
        [Display(Name = "Producer",
            ResourceType = typeof(ItemTr))]
        public string Producer { get; set; }

          [Required(
            ErrorMessageResourceName = "BrandIsRequired",
            ErrorMessageResourceType = typeof(ItemTr))]
        [DenyHtml]
        [Display(Name = "Brand",
            ResourceType = typeof(ItemTr))]
        public string Brand { get; set; }

          [Required(
            ErrorMessageResourceName = "ModelIsRequired",
            ErrorMessageResourceType = typeof(ItemTr))]
        [DenyHtml]
        [Display(Name = "Model",
            ResourceType = typeof(ItemTr))]
        public string ItemModel { get; set; }

          [Required(
            ErrorMessageResourceName = "PriceIsRequired",
            ErrorMessageResourceType = typeof(ItemTr))]
        [Range(0.000001, double.MaxValue,
            ErrorMessageResourceName="PositiveNumError",
            ErrorMessageResourceType=typeof(ItemTr))]
        [Display(Name = "Price",
            ResourceType = typeof(ItemTr))]
        public double Price { get; set; }


        [Display(Name = "IsRotatingItem",
            ResourceType = typeof(ItemTr))]
        public bool RotatingItem { get; set; }

        [Required(
            ErrorMessageResourceName = "QuantityIsRequired",
            ErrorMessageResourceType = typeof(ItemTr))]
        [Range(1, double.MaxValue,
            ErrorMessageResourceName="PositiveNumError",
            ErrorMessageResourceType=typeof(ItemTr))]
        [Display(Name = "Quantity",
            ResourceType = typeof(ItemTr))]
        public int Quantity { get; set; }

        [Display(Name = "Organisation",
            ResourceType = typeof(Common))]
        public List<OrganisationViewModel> Organisations { get; set; }

        [Display(Name = "Organisation",
            ResourceType = typeof(ItemTr))]
        [Required(
            ErrorMessageResourceName = "OrganisationIsRequired",
            ErrorMessageResourceType = typeof(ItemTr))]
        public int SeletedOrganisationId { get; set; }


        [Display(Name = "Currency",
            ResourceType = typeof(ItemTr))]
        public List<CurrencyViewModel> Currency { get; set; }

        [Display(Name = "Currency",
            ResourceType = typeof(ItemTr))]
        [Required(
            ErrorMessageResourceName="CurrencyIsRequired",
            ErrorMessageResourceType=typeof(ItemTr))]
        public int SelectedCurrency { get; set; }
    }

    public class EditItemViewModel
    {
        [Required]
        public int Id { get; set; }


        [Display(Name = "DateOfManufacture",
            ResourceType = typeof(ItemTr))]
        [Required(
          ErrorMessageResourceName = "DateOfManufactureIsRequired",
          ErrorMessageResourceType = typeof(ItemTr))]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public virtual DateTime DateOfManufacture { get; set; }

        [Required(
          ErrorMessageResourceName = "ProducerIsRequired",
          ErrorMessageResourceType = typeof(ItemTr))]
        [DenyHtml]
        [Display(Name = "Producer",
            ResourceType = typeof(ItemTr))]
        public string Producer { get; set; }

        [Required(
          ErrorMessageResourceName = "BrandIsRequired",
          ErrorMessageResourceType = typeof(ItemTr))]
        [DenyHtml]
        [Display(Name = "Brand",
            ResourceType = typeof(ItemTr))]
        public string Brand { get; set; }

        [Required(
          ErrorMessageResourceName = "ModelIsRequired",
          ErrorMessageResourceType = typeof(ItemTr))]
        [DenyHtml]
        [Display(Name = "Model",
            ResourceType = typeof(ItemTr))]
        public string ItemModel { get; set; }

        [Required(
          ErrorMessageResourceName = "PriceIsRequired",
          ErrorMessageResourceType = typeof(ItemTr))]
        [Range(0.000001, double.MaxValue,
            ErrorMessageResourceName = "PositiveNumError",
            ErrorMessageResourceType = typeof(ItemTr))]
        [Display(Name = "Price",
            ResourceType = typeof(ItemTr))]
        public double Price { get; set; }


        [Display(Name = "IsRotatingItem",
            ResourceType = typeof(ItemTr))]
        public bool RotatingItem { get; set; }

        [Required(
            ErrorMessageResourceName = "QuantityIsRequired",
            ErrorMessageResourceType = typeof(ItemTr))]
        [Range(1, double.MaxValue,
            ErrorMessageResourceName = "PositiveNumError",
            ErrorMessageResourceType = typeof(ItemTr))]
        [Display(Name = "Quantity",
            ResourceType = typeof(ItemTr))]
        public int Quantity { get; set; }

        public List<OrganisationViewModel> Organisations { get; set; }

        [Display(Name = "Organisation",
            ResourceType = typeof(ItemTr))]
        [Required(
            ErrorMessageResourceName = "OrganisationIsRequired",
            ErrorMessageResourceType = typeof(ItemTr))]
        public int SeletedOrganisationId { get; set; }


        [Display(Name = "Currency",
            ResourceType = typeof(ItemTr))]
        public List<CurrencyViewModel> Currency { get; set; }

        [Required(
            ErrorMessageResourceName = "CurrencyIsRequired",
            ErrorMessageResourceType = typeof(ItemTr))]
        public int SelectedCurrency { get; set; }
    }

    public class ItemDetailsViewModel
    {

        public int Id { get; set; }

         [Display(Name = "DateOfManufacture",
            ResourceType = typeof(ItemTr))]
        public virtual DateTime DateOfManufacture { get; set; }
        
        [Display(Name = "Producer",
            ResourceType = typeof(ItemTr))]
        public string Producer { get; set; }
         
        [Display(Name = "Brand",
            ResourceType = typeof(ItemTr))]
        public string Brand { get; set; }

        [Display(Name = "Model",
            ResourceType = typeof(ItemTr))]
        public string ItemModel { get; set; }

        [Display(Name = "Price",
            ResourceType = typeof(ItemTr))]
        public double Price { get; set; }

        [Display(Name = "IsRotatingItem",
            ResourceType = typeof(ItemTr))]
        public bool RotatingItem { get; set; }
        
        [Display(Name = "Quantity",
            ResourceType = typeof(ItemTr))]
        public int Quantity { get; set; }

        [Display(Name = "Organisation",
            ResourceType = typeof(ItemTr))]
        public string OrganisationName { get; set; }

        [Display(Name = "Status",
            ResourceType = typeof(Common))]
        public string Status { get; set; }

        [Display(Name = "Currency",
            ResourceType = typeof(ItemTr))]
        public string Currency { get; set; }
    }

    public class OfferItemViewModel
    {
        public int Id { get; set; }

        public string Producer { get; set; }

        public string Brand { get; set; }

        public string ItemModel { get; set; }

        public double Price { get; set; }

        public int Quantity { get; set; }

        public int MotherId { get; set; }

        public string MotherName { get; set; }

        public bool IsSelected { get; set; }

        public int SelectedCount { get; set; }

        public bool IsRotatingItem { get; set; }

        public bool CreateAsset { get; set; }

        public string Currency { get; set; }

        public double CurrencyCourse { get; set; }
    }

    public class ItemsEmailViewModel
    {
        public int Id { get; set; }

        public int Count { get; set; }
    }
}