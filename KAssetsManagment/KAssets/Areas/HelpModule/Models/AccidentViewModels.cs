using KAssets.Models;
using KAssets.Resources.Translation;
using KAssets.Resources.Translation.AccidentTr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KAssets.Areas.HelpModule.Models
{
    public class ListAccidentViewModel
    {
        public int Id { get; set; }

        public string Date { get; set; }

        [Display(
        Name = "From",
        ResourceType = typeof(Common))]
        public string From { get; set; }

        public bool IsSeen { get; set; }
    }

    public class AddAccidentViewModel
    {
        [DenyHtml]
        [Required
         (ErrorMessageResourceName = "MessageRequired",
          ErrorMessageResourceType = typeof(AccidentTr))]
        [Display(
        Name = "Message",
        ResourceType = typeof(AccidentTr))]
        public string Message { get; set; }
    }

    public class SetAnswerViewModel
    {
        [DenyHtml]
        [Required]
        public string Answer { get; set; }

        [Required]
        public int Id { get; set; }
    }

    public class ViewAccidentViewModel
    {
        public int Id { get; set; }

        [Display(
        Name = "From",
        ResourceType = typeof(Common))]
        public string From { get; set; }

        [Display(
        Name = "DateOfSend",
        ResourceType = typeof(KAssets.Resources.Translation.ReportsArea.Reports))]
        public string DateOfSend { get; set; }

        [Display(
        Name = "Message",
        ResourceType = typeof(Common))]
        public string Message { get; set; }

        [Display(
        Name = "Answer",
        ResourceType = typeof(Common))]
        public string Answer { get; set; }

        [Display(
        Name = "DateOfReply",
        ResourceType = typeof(KAssets.Resources.Translation.ReportsArea.Reports))]
        public string DateOfRaply { get; set; }
    }
}