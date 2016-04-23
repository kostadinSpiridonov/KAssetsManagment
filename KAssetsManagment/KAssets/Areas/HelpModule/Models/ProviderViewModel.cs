using KAssets.Areas.Admin.Models;
using KAssets.Models;
using KAssets.Resources.Translation;
using KAssets.Resources.Translation.ItemTr;
using KAssets.Resources.Translation.ProviderTr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KAssets.Areas.HelpModule.Models
{
    public class ProviderViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Name",
            ResourceType = typeof(ProviderTr))]
        public string Name { get; set; }

        [Display(Name = "Email",
            ResourceType = typeof(ProviderTr))]
        public string Email { get; set; }
        
        [Display(
         Name = "Organisation",
         ResourceType = typeof(Common))]
        public string Organisation { get; set; }
    }

    public class ProviderDetailsViewModel
    {
        public int Id { get; set; }

        [Display(Name="Name",
            ResourceType=typeof(ProviderTr))]
        public string Name { get; set; }

        [Display(Name = "Email",
            ResourceType = typeof(ProviderTr))]
        public string Email { get; set; }

        [Display(Name = "Bulstat",
            ResourceType = typeof(ProviderTr))]
        public string Bulstat { get; set; }

        [Display(Name = "Address",
            ResourceType = typeof(ProviderTr))]
        public string Address { get; set; }

        [Display(Name = "Phone",
            ResourceType = typeof(ProviderTr))]
        public string Phone { get; set; }

        [Display(Name = "Status",
            ResourceType = typeof(ProviderTr))]
        public string Status { get; set; }

        public BillViewModel Bill { get; set; }
    }

    public class AddProviderViewModel
    {
        [Required(
            ErrorMessageResourceName="NameIsRequired",
            ErrorMessageResourceType=typeof(ProviderTr))]
        [DenyHtml]
        [Display(Name = "Name",
            ResourceType = typeof(ProviderTr))]
        public string Name { get; set; }

        [Required(
            ErrorMessageResourceName = "EmailIsRequired",
            ErrorMessageResourceType = typeof(ProviderTr))]
        [EmailAddress(
         ErrorMessageResourceType = typeof(Common),
         ErrorMessageResourceName = "NotCorrectEmail",
         ErrorMessage = null
         )]
        [DenyHtml]
        [Display(Name = "Email",
            ResourceType = typeof(ProviderTr))]
        public string Email { get; set; }

        [DenyHtml]
        [Display(Name = "Phone",
            ResourceType = typeof(ProviderTr))]
        public string Phone { get; set; }

        [Required(
            ErrorMessageResourceName = "BulstatIsRequired",
            ErrorMessageResourceType = typeof(ProviderTr))]
        [DenyHtml]
        [Display(Name = "Bulstat",
            ResourceType = typeof(ProviderTr))]
        public string Bulstat { get; set; }

        [Required(
            ErrorMessageResourceName = "AddressIsRequired",
            ErrorMessageResourceType = typeof(ProviderTr))]
        [DenyHtml]
        [Display(Name = "Address",
            ResourceType = typeof(ProviderTr))]
        public string Address { get; set; }

        public AddBillProviderViewModel Bill { get; set; }

        
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
    

    public class EditProviderViewModel
    {
        public int Id { get; set; }

        [Required(
            ErrorMessageResourceName = "NameIsRequired",
            ErrorMessageResourceType = typeof(ProviderTr))]
        [DenyHtml]
        [Display(Name = "Name",
            ResourceType = typeof(ProviderTr))]
        public string Name { get; set; }

        [Required(
            ErrorMessageResourceName = "EmailIsRequired",
            ErrorMessageResourceType = typeof(ProviderTr))]
        [EmailAddress(
         ErrorMessageResourceType = typeof(Common),
         ErrorMessageResourceName = "NotCorrectEmail",
         ErrorMessage = null
         )]
        [DenyHtml]
        [Display(Name = "Email",
            ResourceType = typeof(ProviderTr))]
        public string Email { get; set; }

        [DenyHtml]
        [Display(Name = "Phone",
            ResourceType = typeof(ProviderTr))]
        public string Phone { get; set; }

        [DenyHtml]
        public string Status { get; set; }

        [Required(
            ErrorMessageResourceName = "BulstatIsRequired",
            ErrorMessageResourceType = typeof(ProviderTr))]
        [Display(Name = "Bulstat",
            ResourceType = typeof(ProviderTr))]
        public string Bulstat { get; set; }

        [Required(
            ErrorMessageResourceName = "AddressIsRequired",
            ErrorMessageResourceType = typeof(ProviderTr))]
        [DenyHtml]
        [Display(Name = "Address",
            ResourceType = typeof(ProviderTr))]
        public string Address { get; set; }

        public AddBillProviderViewModel Bill { get; set; }

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
}