using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KAssets.Areas.Reports.Models
{
    public class CreateAssetStatusReport
    {
        public List<string> Statuses { get; set; }

        [Required
         (ErrorMessageResourceName = "StatusIsRequired",
          ErrorMessageResourceType = typeof(KAssets.Resources.Translation.ReportsArea.Reports))]
        public string SelectedStatus { get; set; }
    }
}