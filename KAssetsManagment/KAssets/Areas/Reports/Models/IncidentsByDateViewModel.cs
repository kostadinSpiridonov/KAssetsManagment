using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KAssets.Resources.Translation;

namespace KAssets.Areas.Reports.Models
{
    public class CreateAccidentesByDateReport
    {
        [Display(
        Name = "DateOfSend",
        ResourceType = typeof(KAssets.Resources.Translation.ReportsArea.Reports))]
        public DateTime? DateOfSend { get; set; }

        [Display(
        Name = "DateOfReply",
        ResourceType = typeof(KAssets.Resources.Translation.ReportsArea.Reports))]
        public DateTime? DateOfReply { get; set; }

        [Display(
        Name = "SearchType",
        ResourceType = typeof(KAssets.Resources.Translation.ReportsArea.Reports))]
        public bool ORorAND { get; set; }
    }


    public class ListAccidentViewModelReport
    {
        public int Id { get; set; }

        [Display(
        Name = "DateOfSend",
        ResourceType = typeof(KAssets.Resources.Translation.ReportsArea.Reports))]
        public string DateOfSend { get; set; }

        [Display(
        Name = "DateOfReply",
        ResourceType = typeof(KAssets.Resources.Translation.ReportsArea.Reports))]
        public string DateOfReply { get; set; }

        [Display(
        Name = "From",
        ResourceType = typeof(Common))]
        public string From { get; set; }

        public bool IsReply { get; set; }
    }
}