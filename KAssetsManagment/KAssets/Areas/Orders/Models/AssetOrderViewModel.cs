using KAssets.Areas.AssetsActions.Models;
using KAssets.Models;
using KAssets.Resources.Translation;
using KAssets.Resources.Translation.OrdersTr.AssetOrderTr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KAssets.Areas.Orders.Models
{
    public class SendReqeustAssetOrderViewModel
    {
        [DenyHtml]
        public List<string> Assets { get; set; }
    }


    public class ListAssetRequestViewModel
    {
        [Display(
            Name="From",
            ResourceType=typeof(Common))]
        public ShortUserDetails From { get; set; }

        [Display(Name = "DateOfSend",
            ResourceType = typeof(AssetOrderTr))]
        public DateTime DateOfSend { get; set; }

        public int Id { get; set; }
    }

    public class BaseRequestForAssetViewModel
    {
        public RequestForAssetViewModel Request { get; set; }

        public List<RequestAssetViewModel> WantAssets { get; set; }

        public List<RequestAssetViewModel> ApprovedAssets { get; set; }

        public List<RequestAssetViewModel> GivenAssets { get; set; }

        public bool IsApproved { get; set; }

        public bool AreItemGave { get; set; }
    }

    public class RequestAssetViewModel
    {
        public bool Selected { get; set; }

        public AssetViewModel Asset { get; set; }
    }

    public class RequestForAssetViewModel
    {
        [Display(
            Name = "From",
            ResourceType = typeof(Common))]
        public ShortUserDetails From { get; set; }

        [Display(Name = "DateOfSend",
            ResourceType = typeof(AssetOrderTr))]
        public DateTime DateOfSend { get; set; }

        public int Id { get; set; }

        public bool IsFinished { get; set; }

        public int PackingSlipId { get; set; }
    }

}