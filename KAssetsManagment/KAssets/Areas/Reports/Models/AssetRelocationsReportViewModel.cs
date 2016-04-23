using KAssets.Areas.AssetsActions.Models;
using KAssets.Models;
using KAssets.Resources.Translation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KAssets.Areas.Reports.Models
{
    public class CreateAssetRelocationsReportViewModel
    {

        [Required
         (ErrorMessageResourceName = "AssetIsRequired",
          ErrorMessageResourceType = typeof(KAssets.Resources.Translation.ReportsArea.Reports))]
        [Display(
        Name = "Asset",
        ResourceType = typeof(Common))]
        public string SelectedAsset { get; set; }
    }

    public class AssetRelocationsReportResult
    {

        public AssetDetailsViewModel AssetDetails { get; set; }

        public List<AssetRelocationReportViewModel> History { get; set; }

    }

    public class AssetRelocationReportViewModel
    {
        [Display(
        Name = "FromSite",
        ResourceType = typeof(KAssets.Resources.Translation.ReportsArea.Reports))]
        public string FromSite { get; set; }

        [Display(
        Name = "ToSiteRel",
        ResourceType = typeof(KAssets.Resources.Translation.ReportsArea.Reports))]
        public string ToSite { get; set; }

        [Display(
        Name = "FromUser",
        ResourceType = typeof(KAssets.Resources.Translation.ReportsArea.Reports))]
        public string FromUser { get; set; }

        [Display(
        Name = "ToUser",
        ResourceType = typeof(KAssets.Resources.Translation.ReportsArea.Reports))]
        public string ToUser { get; set; }

        [Display(
        Name = "ToLocation",
        ResourceType = typeof(KAssets.Resources.Translation.ReportsArea.Reports))]
        public string ToLocation { get; set; }

        [Display(
        Name = "FromLocation",
        ResourceType = typeof(KAssets.Resources.Translation.ReportsArea.Reports))]
        public string FromLocation { get; set; }

        [Display(
        Name = "DateOfGiven",
        ResourceType = typeof(KAssets.Resources.Translation.ReportsArea.Reports))]
        public string DateOfGiven { get; set; }

        [Display(
        Name = "DateOfReceived",
        ResourceType = typeof(KAssets.Resources.Translation.ReportsArea.Reports))]
        public string DateOfReceived { get; set; }
    }
}