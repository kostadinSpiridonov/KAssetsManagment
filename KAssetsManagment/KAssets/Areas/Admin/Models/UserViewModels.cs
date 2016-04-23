using KAssets.Areas.HelpModule.Models;
using KAssets.Models;
using KAssets.Resources.Translation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KAssets.Areas.Admin.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Display(
           Name = "FirstName",
           ResourceType = typeof(Common))]
        public string FirstName { get; set; }

        [Display(
           Name = "SecondName",
           ResourceType = typeof(Common))]
        public string SecondName { get; set; }

        [Display(
           Name = "LastName",
           ResourceType = typeof(Common))]
        public string LastName { get; set; }

        [Display(
           Name = "Email",
           ResourceType = typeof(Common))]
        public string Email { get; set; }
    }

    public class RegisterViewModel
    {
        [Required
          (ErrorMessageResourceName = "EmailIsRequired",
           ErrorMessageResourceType = typeof(Common))]
        [EmailAddress(
          ErrorMessageResourceType = typeof(Common),
          ErrorMessageResourceName = "NotCorrectEmail",
          ErrorMessage = null
          )]
        [Display(
           Name = "Email",
           ResourceType = typeof(Common))]
        [DenyHtml]
        public string Email { get; set; }

        [Required
          (ErrorMessageResourceName = "PassRequired",
           ErrorMessageResourceType = typeof(Common))]
        [StringLength(100,
             ErrorMessageResourceName = "ShoudBeAtLeast",
             ErrorMessageResourceType = typeof(Common),
             MinimumLength = 6
             )]
        [DataType(DataType.Password)]
        [Display(
           Name = "Password",
           ResourceType = typeof(Common))]
        [DenyHtml]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(
         Name = "ConfirmPass",
         ResourceType = typeof(Common))]
        [Compare("Password", ErrorMessageResourceName = "ComparePassError",
            ErrorMessageResourceType = typeof(Common))]
        [DenyHtml]
        public string ConfirmPassword { get; set; }

        [Display(
        Name = "SecGroups",
        ResourceType = typeof(KAssets.Resources.Translation.AdminArea.Admin))]
        public List<SelectSecurityGroupViewModel> SelectedSecurityGroups { get; set; }

        public List<DropDownLocationViewModel> Locations { get; set; }

        [Display(
        Name = "Location",
        ResourceType = typeof(Common))]
        [DenyHtml]
        public string SelectedLocationCode { get; set; }

        [Display(Name = "Organisation",
        ResourceType = typeof(Common))]
        public List<OrganisationDropDownViewModel> Organisations { get; set; }

        public int? SelectedOrganisationId { get; set; }

        [Display(Name = "Site",
        ResourceType = typeof(Common))]
        public int? SelectedSiteId { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required
         (ErrorMessageResourceName = "EmailIsRequired",
          ErrorMessageResourceType = typeof(Common))]
        [EmailAddress(
          ErrorMessageResourceType = typeof(Common),
          ErrorMessageResourceName = "NotCorrectEmail",
          ErrorMessage = null
          )]
        [Display(
           Name = "Email",
           ResourceType = typeof(Common))]
        [DenyHtml]
        public string Email { get; set; }

        [Required
        (ErrorMessageResourceName = "PassRequired",
         ErrorMessageResourceType = typeof(Common))]
        [StringLength(100,
             ErrorMessageResourceName = "ShoudBeAtLeast",
             ErrorMessageResourceType = typeof(Common),
             MinimumLength = 6
             )]
        [DataType(DataType.Password)]
        [Display(
           Name = "Password",
           ResourceType = typeof(Common))]
        [DenyHtml]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(
         Name = "ConfirmPass",
         ResourceType = typeof(Common))]
        [Compare("Password", ErrorMessageResourceName = "ComparePassError",
            ErrorMessageResourceType = typeof(Common))]
        [DenyHtml]
        public string ConfirmPassword { get; set; }

        [Display(
           Name = "FirstName",
           ResourceType = typeof(Common))]
        [DenyHtml]
        public string FirstName { get; set; }

        [Display(
           Name = "SecondName",
           ResourceType = typeof(Common))]
        [DenyHtml]
        public string SecondName { get; set; }

        [Display(
           Name = "LastName",
           ResourceType = typeof(Common))]
        [DenyHtml]
        public string LastName { get; set; }

        [Display(
           Name = "AboutMe",
           ResourceType = typeof(Common))]
        [DenyHtml]
        public string AboutMe { get; set; }

        [Display(
           Name = "Skype",
           ResourceType = typeof(Common))]
        [DenyHtml]
        public string Skype { get; set; }

        [Display(
           Name = "SecGroups",
        ResourceType = typeof(KAssets.Resources.Translation.AdminArea.Admin))]
        public List<SelectSecurityGroupViewModel> SecurityGroups { get; set; }

        public List<DropDownLocationViewModel> Locations { get; set; }

        [Display(
           Name = "Location",
           ResourceType = typeof(Common))]
        [DenyHtml]
        public string SelectedLocationCode { get; set; }


        [Display(
           Name = "Organisation",
           ResourceType = typeof(Common))]
        public List<OrganisationDropDownViewModel> Organisations { get; set; }

        [Display(
           Name = "Organisation",
           ResourceType = typeof(Common))]
        public int? SelectedOrganisationId { get; set; }

        [Display(
           Name = "Site",
           ResourceType = typeof(Common))]
        public int? SelectedSiteId { get; set; }
    }

    public class UserDetailsViewModel
    {
        [Display(
           Name = "Email",
           ResourceType = typeof(Common))]
        public string Email { get; set; }

        [Display(
           Name = "FirstName",
           ResourceType = typeof(Common))]
        public string FirstName { get; set; }

        [Display(
           Name = "SecondName",
           ResourceType = typeof(Common))]
        public string SecondName { get; set; }

        [Display(
           Name = "LastName",
           ResourceType = typeof(Common))]
        public string LastName { get; set; }

        [Display(
           Name = "AboutMe",
           ResourceType = typeof(Common))]
        public string AboutMe { get; set; }

        [Display(
           Name = "Skype",
           ResourceType = typeof(Common))]
        public string Skype { get; set; }

        [Display(
           Name = "SecGroups",
           ResourceType = typeof(KAssets.Resources.Translation.AdminArea.Admin))]
        public List<SecurityGroupViewModel> SecurityGroups { get; set; }

        public string Id { get; set; }

        [Display(
           Name = "Location",
           ResourceType = typeof(Common))]
        public LocationViewModel Location { get; set; }

        [Display(
           Name = "Organisation",
           ResourceType = typeof(Common))]
        public string Organisation { get; set; }

        [Display(
           Name = "Status",
           ResourceType = typeof(Common))]
        public string Status { get; set; }

        [Display(
           Name = "Site",
           ResourceType = typeof(Common))]
        public string Site { get; set; }
    }
}