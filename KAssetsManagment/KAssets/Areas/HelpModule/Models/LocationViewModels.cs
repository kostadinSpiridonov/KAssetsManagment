using KAssets.Areas.Admin.Models;
using KAssets.Models;
using KAssets.Resources.Translation;
using KAssets.Resources.Translation.ItemTr;
using KAssets.Resources.Translation.LocationsTr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KAssets.Areas.HelpModule.Models
{
    public class LocationViewModel
    {
        [Display(
        Name = "Code",
        ResourceType = typeof(LocationTr))]
        public string Code { get; set; }

        [Display(
        Name = "Latitude",
        ResourceType = typeof(LocationTr))]
        public string Latitude { get; set; }

        [Display(
        Name = "Longitude",
        ResourceType = typeof(LocationTr))]
        public string Longitude { get; set; }

        [Display(
        Name = "Country",
        ResourceType = typeof(LocationTr))]
        public string Country { get; set; }

        [Display(
        Name = "Town",
        ResourceType = typeof(LocationTr))]
        public string Town { get; set; }

        [Display(
        Name = "Street",
        ResourceType = typeof(LocationTr))]
        public string Street { get; set; }

        [Display(
        Name = "StreetNumber",
        ResourceType = typeof(LocationTr))]
        public int? StreetNumber { get; set; }
    }

    public class AddLocationViewModel
    {
        [Required(
            ErrorMessageResourceName = "CodeIsRequired",
            ErrorMessageResourceType = typeof(LocationTr))]
        [DenyHtml]
        [Display(
        Name = "Code",
        ResourceType = typeof(LocationTr))]
        public string Code { get; set; }

        [DenyHtml]
        [Display(
        Name = "Latitude",
        ResourceType = typeof(LocationTr))]
        public string Latitude { get; set; }

        [DenyHtml]
        [Display(
        Name = "Longitude",
        ResourceType = typeof(LocationTr))]
        public string Longitude { get; set; }

        [DenyHtml]
        [Display(
        Name = "Country",
        ResourceType = typeof(LocationTr))]
        public string Country { get; set; }

        [DenyHtml]
        [Display(
        Name = "Town",
        ResourceType = typeof(LocationTr))]
        public string Town { get; set; }

        [DenyHtml]
        [Display(
        Name = "Street",
        ResourceType = typeof(LocationTr))]
        public string Street { get; set; }

        [Display(
        Name = "StreetNumber",
        ResourceType = typeof(LocationTr))]
        public int? StreetNumber { get; set; }

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

    public class EditLocationViewModel
    {
        [Required(
           ErrorMessageResourceName = "CodeIsRequired",
           ErrorMessageResourceType = typeof(LocationTr))]
        [DenyHtml]
        [Display(
        Name = "Code",
        ResourceType = typeof(LocationTr))]
        public string OldCode { get; set; }

        [DenyHtml]
        [Display(
        Name = "Latitude",
        ResourceType = typeof(LocationTr))]
        public string Latitude { get; set; }

        [DenyHtml]
        [Display(
        Name = "Longitude",
        ResourceType = typeof(LocationTr))]
        public string Longitude { get; set; }

        [DenyHtml]
        [Display(
        Name = "Country",
        ResourceType = typeof(LocationTr))]
        public string Country { get; set; }

        [DenyHtml]
        [Display(
        Name = "Town",
        ResourceType = typeof(LocationTr))]
        public string Town { get; set; }

        [DenyHtml]
        [Display(
        Name = "Street",
        ResourceType = typeof(LocationTr))]
        public string Street { get; set; }


        [Display(
        Name = "StreetNumber",
        ResourceType = typeof(LocationTr))]
        public int? StreetNumber { get; set; }


    }

    public class ShowLocationViewModel
    {
        [Display(Name = "Code",
            ResourceType = typeof(LocationTr))]
        public string Code { get; set; }

        [Display(
         Name = "Organisation",
         ResourceType = typeof(Common))]
        public string Organisation { get; set; }
    }

    public class DropDownLocationViewModel
    {
        [DenyHtml]
        public string Code { get; set; }

        [DenyHtml]
        public string Location { get; set; }
    }
}