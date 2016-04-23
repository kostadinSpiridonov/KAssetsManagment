using KAssets.Areas.HelpModule.Models;
using KAssets.Resources.Translation;
using KAssets.Resources.Translation.BillTr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KAssets.Models
{
    public class BillViewModel
    {
        public int Id { get; set; }

        public int OrganisationId { get; set; }

        public string IBAN { get; set; }

        [Display(Name = "Currency",
            ResourceType = typeof(Common))]
        public string Currency { get; set; }
    }

    public class AddBillViewModel
    {
        public int Id { get; set; }

        [Required]
        public int OrganisationId { get; set; }

        [Required(
            ErrorMessageResourceName = "IBANIsRequired",
            ErrorMessageResourceType = typeof(BillTr))]
        [DenyHtml]
        public string IBAN { get; set; }

        [Display(Name = "Currency",
            ResourceType = typeof(Common))]
        public List<CurrencyViewModel> Currency { get; set; }

        [Required]
        public int SelectedCurrency { get; set; }
    }

    public class AddBillProviderViewModel
    {
        public int Id { get; set; }

        [Required(
            ErrorMessageResourceName = "IBANIsRequired",
            ErrorMessageResourceType = typeof(BillTr))]
        [DenyHtml]
        public string IBAN { get; set; }

        [Display(Name = "Currency",
            ResourceType = typeof(Common))]
        public List<CurrencyViewModel> Currency { get; set; }

        [Required]
        public int SelectedCurrency { get; set; }
    }

    public class EditBillViewModel
    {
        [Required]
        public int BillId { get; set; }

        [Required]
        public int OrganisationId { get; set; }

        [Required(
        ErrorMessageResourceName = "IBANIsRequired",
        ErrorMessageResourceType = typeof(BillTr))]
        [DenyHtml]
        public string IBAN { get; set; }

        [Display(Name = "Currency",
            ResourceType = typeof(Common))]
        public List<CurrencyViewModel> Currency { get; set; }

        [Required]
        public int SelectedCurrency { get; set; }
    }
}