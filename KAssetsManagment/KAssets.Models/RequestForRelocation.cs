using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAssets.Models
{
    public class RequestForRelocation
    {
        public int Id { get; set; }


        public string AssetId { get; set; }

        [ForeignKey("AssetId")]
        public virtual Asset Asset { get; set; }


        public string FromId { get; set; }

        public virtual ApplicationUser From { get; set; }


        public int ToSiteId { get; set; }

        [ForeignKey("ToSiteId")]
        public virtual Site ToSite { get; set; }


        public virtual DateTime DateOfSend { get; set; }

        public bool IsApproved { get; set; }

        public bool IsFinished { get; set; }

        public bool AreAssetGiven { get; set; }

        public bool SeenFromApprover { get; set; }

        public string OldSiteName { get; set; }

        public string OldLocationCode { get; set; }

        public string OldUserId { get; set; }


        public string ToUserId { get; set; }

        [ForeignKey("ToUserId")]
        public virtual ApplicationUser ToUser { get; set; }


        public string ToLocationId { get; set; }

        [ForeignKey("ToLocationId")]
        public virtual Location ToLocation { get; set; }


        public int? PackingSlipId { get; set; }

        [ForeignKey("PackingSlipId")]
        public virtual PackingSlip PackingSlip { get; set; }
    }
}
