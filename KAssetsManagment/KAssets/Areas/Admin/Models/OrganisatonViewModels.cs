using KAssets.Models;
using KAssets.Resources.Translation;
using KAssets.Resources.Translation.OrganisationTr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KAssets.Areas.Admin.Models
{
    public class OrganisationViewModel
    {
        public int Id { get; set; }

        [Required(
            ErrorMessageResourceName="NameIsRequired",
            ErrorMessageResourceType=typeof(OrganisationTr))]
        [DenyHtml]
        [Display(Name="Name",
            ResourceType=typeof(OrganisationTr))]
        public string Name { get; set; }

        [Required(
            ErrorMessageResourceName = "AddressIsRequired",
            ErrorMessageResourceType = typeof(OrganisationTr))]
        [DenyHtml]
        [Display(Name = "Address",
            ResourceType = typeof(OrganisationTr))]
        public string Address { get; set; }

        [Required(
        ErrorMessageResourceName = "EmailClientIsRequired",
        ErrorMessageResourceType = typeof(OrganisationTr))]
        [DenyHtml]
        [EmailAddress(
        ErrorMessageResourceType = typeof(Common),
        ErrorMessageResourceName = "NotCorrectEmail",
        ErrorMessage = null
        )]
        [Display(Name = "EmailClient",
            ResourceType = typeof(OrganisationTr))]
        public string EmaiClient { get; set; }

        [Required(
        ErrorMessageResourceName = "EmailClientPassIsRequired",
        ErrorMessageResourceType = typeof(OrganisationTr))]
        [DenyHtml]
        [DataType(DataType.Password)]
        [Display(Name = "EmailClientPass",
            ResourceType = typeof(OrganisationTr))]
        public string EmailClientPassword { get; set; }
    }

    public class EditOrganisationViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(
            ErrorMessageResourceName = "NameIsRequired",
            ErrorMessageResourceType = typeof(OrganisationTr))]
        [DenyHtml]
        [Display(Name = "Name",
            ResourceType = typeof(OrganisationTr))]
        public string Name { get; set; }

        [Required(
            ErrorMessageResourceName = "AddressIsRequired",
            ErrorMessageResourceType = typeof(OrganisationTr))]
        [DenyHtml]
        [Display(Name = "Address",
            ResourceType = typeof(OrganisationTr))]
        public string Address { get; set; }

        [Required(
    ErrorMessageResourceName = "EmailClientIsRequired",
    ErrorMessageResourceType = typeof(OrganisationTr))]
        [DenyHtml]
        [EmailAddress(
        ErrorMessageResourceType = typeof(Common),
        ErrorMessageResourceName = "NotCorrectEmail",
        ErrorMessage = null
        )]
        [Display(Name = "EmailClient",
            ResourceType = typeof(OrganisationTr))]
        public string EmaiClient { get; set; }

        [Required(
        ErrorMessageResourceName = "EmailClientPassIsRequired",
        ErrorMessageResourceType = typeof(OrganisationTr))]
        [DenyHtml]
        [DataType(DataType.Password)]
        [Display(Name = "EmailClientPass",
            ResourceType = typeof(OrganisationTr))]
        public string EmailClientPassword { get; set; }
    }

    public class OrganisationDropDownViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<SiteViewModel> Sites { get; set; }
    }

}