using KAssets.Areas.Admin.Models;
using KAssets.Models;
using KAssets.Resources.Translation;
using KAssets.Resources.Translation.ExchangeRateTr;
using KAssets.Resources.Translation.ItemTr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KAssets.Areas.HelpModule.Models
{
    public class ExchangeRateViewModel
    {
        public int Id { get; set; }

        [Display(Name = "FromCurrency",
          ResourceType = typeof(ExchangeRateTr))]
        [Required(
            ErrorMessageResourceName = "FromCurrencyIsRequired",
            ErrorMessageResourceType = typeof(ExchangeRateTr))]
        [DenyHtml]
        public string From { get; set; }

        [Display(Name = "ToCurrency",
         ResourceType = typeof(ExchangeRateTr))]
        [Required(
            ErrorMessageResourceName = "ToCurrencyIsRequired",
            ErrorMessageResourceType = typeof(ExchangeRateTr))]
        [DenyHtml]
        public string To { get; set; }


        [Display(Name = "Rate",
            ResourceType = typeof(ExchangeRateTr))]
        [Required(
            ErrorMessageResourceName = "RateIsRequired",
            ErrorMessageResourceType = typeof(ExchangeRateTr))]
        public double Rate { get; set; }

        [Display(
       Name = "Organisation",
       ResourceType = typeof(Common))]
        public string Organisation { get; set; }
    }

    public class AddExchangeRateViewModel
    {
        public int Id { get; set; }

        [Display(Name = "FromCurrency",
            ResourceType = typeof(ExchangeRateTr))]
        [Required(
            ErrorMessageResourceName = "FromCurrencyIsRequired",
            ErrorMessageResourceType = typeof(ExchangeRateTr))]
        public int From { get; set; }


        [Display(Name = "ToCurrency",
            ResourceType = typeof(ExchangeRateTr))]
        [Required(
            ErrorMessageResourceName = "ToCurrencyIsRequired",
            ErrorMessageResourceType = typeof(ExchangeRateTr))]
        public int To { get; set; }


        [Display(Name = "Rate",
            ResourceType = typeof(ExchangeRateTr))]
        [Required(
            ErrorMessageResourceName = "RateIsRequired",
            ErrorMessageResourceType = typeof(ExchangeRateTr))]
        public double Rate { get; set; }

        [Display(Name = "Organisation",
        ResourceType = typeof(Common))]
        public List<OrganisationViewModel> Organisations { get; set; }

        [Display(Name = "Organisation",
            ResourceType = typeof(ItemTr))]
        [Required(
            ErrorMessageResourceName = "OrganisationIsRequired",
            ErrorMessageResourceType = typeof(ItemTr))]
        public int SeletedOrganisationId { get; set; }
    }

    public class EditExchangeRateViewModel
    {
        [Required]
        public int Id { get; set; }

        [Display(Name = "FromCurrency",
            ResourceType = typeof(ExchangeRateTr))]
        [Required(
            ErrorMessageResourceName = "FromCurrencyIsRequired",
            ErrorMessageResourceType = typeof(ExchangeRateTr))]
        public int From { get; set; }

        [Display(Name = "ToCurrency",
            ResourceType = typeof(ExchangeRateTr))]
        [Required(
            ErrorMessageResourceName = "ToCurrencyIsRequired",
            ErrorMessageResourceType = typeof(ExchangeRateTr))]
        public int To { get; set; }


        [Display(Name = "Rate",
            ResourceType = typeof(ExchangeRateTr))]
        [Required(
            ErrorMessageResourceName = "RateIsRequired",
            ErrorMessageResourceType = typeof(ExchangeRateTr))]
        public double Rate { get; set; }

        [Display(Name = "Organisation",
          ResourceType = typeof(ItemTr))]
        [Required(
            ErrorMessageResourceName = "OrganisationIsRequired",
            ErrorMessageResourceType = typeof(ItemTr))]
        public int SeletedOrganisationId { get; set; }
    }
}