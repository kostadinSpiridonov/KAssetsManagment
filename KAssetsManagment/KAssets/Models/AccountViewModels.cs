using KAssets.Areas.HelpModule.Models;
using KAssets.Resources.Translation;
using KAssets.Resources.Translation.AccountTr;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KAssets.Models
{

    public class ShortUserDetails
    {
        [DenyHtml]
        public string Id { get; set; }

        [Display(Name="Name",
            ResourceType=typeof(Common))]
        public string FullName { get; set; }
    }

    public class UserDetailsViewModelBase
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
        public List<string> SecurityGroups { get; set; }

        public string Id { get; set; }

        [Display(
        Name = "Location",
        ResourceType = typeof(Common))]
        public LocationViewModel Location { get; set; }
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
    }

    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required
        (ErrorMessageResourceName = "EmailIsRequired",
        ErrorMessageResourceType = typeof(Common))]
        [Display(
        Name = "Email",
        ResourceType = typeof(Common))]
        [EmailAddress(ErrorMessageResourceName = "NotCorrectEmail",
            ErrorMessageResourceType = typeof(Common))]
        [DenyHtml]
        public string Email { get; set; }

        [Required
       (ErrorMessageResourceName = "PassRequired",
       ErrorMessageResourceType = typeof(Common))]
        [DataType(DataType.Password)]
        [Display(
        Name = "Password",
        ResourceType = typeof(Common))]
        [DenyHtml]
        public string Password { get; set; }

        [Display(
       Name = "RememberMe",
       ResourceType = typeof(AccountTr))]
        public bool RememberMe { get; set; }
    }


    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
    public class ShowUsersViewModel
    {
        public int SiteId { get; set; }

        public List<ShowUserViewModel> Users { get; set; }
    }

    public class ShowUserViewModel
    {
        [Display(Name = "Id",
            ResourceType = typeof(Common))]
        public string Id { get; set; }

        [Display(Name = "Name",
            ResourceType = typeof(Common))]
        public string Name { get; set; }
    }

    public class ChooseUserViewModel
    {
        [DenyHtml]
        public string Id { get; set; }

        [DenyHtml]
        public string Name { get; set; }

        public bool IsSelected { get; set; }
    }

    public class AddUserToSiteViewModel
    {
        public int SiteId { get; set; }

        public List<ChooseUserViewModel> Users { get; set; }
    }

    public class UserLViewModel
    {
        [DenyHtml]
        public string Id { get; set; }

        [DenyHtml]
        public string Name { get; set; }

        [DenyHtml]
        public string LocationId { get; set; }
    }
}
