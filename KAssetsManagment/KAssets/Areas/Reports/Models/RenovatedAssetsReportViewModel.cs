using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using KAssets.Models;
using System.Web;
using KAssets.Areas.AssetsActions.Models;

namespace KAssets.Areas.Reports.Models
{
    public class CreateRenovatedAssetsReportViewModel
    {
        [Display(
        Name = "FromDate",
        ResourceType = typeof(KAssets.Resources.Translation.ReportsArea.Reports))]
        [Required
         (ErrorMessageResourceName = "FromDateRequired",
          ErrorMessageResourceType = typeof(KAssets.Resources.Translation.ReportsArea.Reports))]
        public DateTime From { get; set; }

        [Display(
        Name = "ToDate",
        ResourceType = typeof(KAssets.Resources.Translation.ReportsArea.Reports))]
        [Required
         (ErrorMessageResourceName = "ToDateRequired",
          ErrorMessageResourceType = typeof(KAssets.Resources.Translation.ReportsArea.Reports))]
        public DateTime To { get; set; }
    }

    public class RenovatedAssetsReportResultViewModel
    {
        public List<AssetViewModel> Assets { get; set; }

        [Display(
        Name = "FromDate",
        ResourceType = typeof(KAssets.Resources.Translation.ReportsArea.Reports))]
        public string FromDate { get; set; }

        [Display(
        Name = "ToDate",
        ResourceType = typeof(KAssets.Resources.Translation.ReportsArea.Reports))]
        public string ToDate { get; set; }
    }
}