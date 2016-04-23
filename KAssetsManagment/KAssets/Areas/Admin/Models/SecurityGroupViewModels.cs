using KAssets.Models;
using KAssets.Resources.Translation;
using KAssets.Resources.Translation.AdminArea;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KAssets.Areas.Admin.Models
{
    public class SecurityGroupViewModel
    {
        public int Id { get; set; }

        [Display(
            Name = "Name",
            ResourceType = typeof(Common))]
        public string Name { get; set; }
    }

    public class AddSecurityGroupViewModel
    {
        public int Id { get; set; }

        [Required
        (ErrorMessageResourceName = "NameIsRequired",
        ErrorMessageResourceType = typeof(KAssets.Resources.Translation.AdminArea.Admin))]

        [DenyHtml]
        [Display(
            Name = "Name",
            ResourceType = typeof(Common))]
        public string Name { get; set; }

        [Display(
            Name = "Rights",
            ResourceType = typeof(Common))]
        public List<SelectedRightViewModel> SelectedRights { get; set; }
    }

    public class EditSecurityGroupViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required
       (ErrorMessageResourceName = "NameIsRequired",
       ErrorMessageResourceType = typeof(KAssets.Resources.Translation.AdminArea.Admin))]

        [DenyHtml]
        [Display(
            Name = "Name",
            ResourceType = typeof(Common))]
        public string Name { get; set; }

        [Display(
         Name = "Rights",
         ResourceType = typeof(Common))]
        public List<SelectedRightViewModel> SelectedRights { get; set; }
    }

    public class DetailsSecurityGroupViewModel
    {
        public int Id { get; set; }

        [Display(
            Name = "Name",
            ResourceType = typeof(Common))]
        public string Name { get; set; }

        [Display(
         Name = "Rights",
         ResourceType = typeof(Common))]
        public List<RightViewModel> Rights { get; set; }
    }

    public class SelectSecurityGroupViewModel
    {
        public SecurityGroupViewModel SecurityGroup { get; set; }

        public bool IsSelected { get; set; }
    }
}