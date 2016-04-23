using KAssets.Areas.Admin.Models;
using KAssets.Models;
using KAssets.Resources.Translation;
using KAssets.Resources.Translation.CurrencyTr;
using KAssets.Resources.Translation.ItemTr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KAssets.Areas.HelpModule.Models
{
    public class CurrencyViewModel
    {
        public int Id { get; set; }

        [Display(
            Name = "Code",
            ResourceType = typeof(CurrencyTr))]
        public string Code { get; set; }

        [Display(
            Name = "Description",
            ResourceType = typeof(CurrencyTr))]
        public string Description { get; set; }

        [Display(
       Name = "Organisation",
       ResourceType = typeof(Common))]
        public string Organisation { get; set; }
    }

    public class AddCurrencyViewModel
    {
        [Required(
            ErrorMessageResourceName = "CodeIsRequired",
            ErrorMessageResourceType = typeof(CurrencyTr))]
        [DenyHtml]
        [Display(
            Name = "Code",
            ResourceType = typeof(CurrencyTr))]
        public string Code { get; set; }

        [Required(
           ErrorMessageResourceName = "DescriptionIsRequired",
           ErrorMessageResourceType = typeof(CurrencyTr))]
        [DenyHtml]
        [Display(
            Name = "Description",
            ResourceType = typeof(CurrencyTr))]
        public string Description { get; set; }

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

    public class EditCurrencyViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(
            ErrorMessageResourceName = "CodeIsRequired",
            ErrorMessageResourceType = typeof(CurrencyTr))]
        [DenyHtml]
        [Display(
            Name = "Code",
            ResourceType = typeof(CurrencyTr))]
        public string Code { get; set; }

        [Required(
          ErrorMessageResourceName = "DescriptionIsRequired",
          ErrorMessageResourceType = typeof(CurrencyTr))]
        [DenyHtml]
        [Display(
            Name = "Description",
            ResourceType = typeof(CurrencyTr))]
        public string Description { get; set; }
    }

}