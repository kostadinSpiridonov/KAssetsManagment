using KAssets.Models;
using KAssets.Resources.Translation;
using KAssets.Resources.Translation.SiteTr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KAssets.Areas.Admin.Models
{
    public class SiteViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Name",
            ResourceType = typeof(Common))]
        public string Name { get; set; }

        public int OrganisationId { get; set; }

        public string OrganisationName { get; set; }
    }

    public class AddSiteViewModel
    {
        public int Id { get; set; }

        [Required]
        public int OrganisationId { get; set; }

        [Required(
            ErrorMessageResourceName="NameIsRequired",
            ErrorMessageResourceType=typeof(SiteTr))]
        [Display(
            Name="Name",
            ResourceType=typeof(SiteTr))]
        [DenyHtml]
        public string Name { get; set; }
    }

    public class ShowSitesViewModel
    {
        public List<SiteViewModel> Sites { get; set; }

        public int OrganisationId { get; set; }
    }

    public class EditSiteViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(
            ErrorMessageResourceName = "NameIsRequired",
            ErrorMessageResourceType = typeof(SiteTr))]
        [Display(
            Name = "Name",
            ResourceType = typeof(SiteTr))]
        [DenyHtml]
        public string Name { get; set; }
    }

    public class DeleteSiteViewMode
    {
        public int Id { get; set; }

        public int OrganisationId { get; set; }
    }
}