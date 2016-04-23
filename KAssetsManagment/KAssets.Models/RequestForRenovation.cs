using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAssets.Models
{
    public class RequestForRenovation
    {
        public int Id { get; set; }


        public string FromId { get; set; }

        [ForeignKey("FromId")]
        public virtual ApplicationUser From { get; set; }


        public string AssetId { get; set; }

        [ForeignKey("AssetId")]
        public virtual Asset Asset { get; set; }


        public virtual DateTime DateOfSend { get; set; }

        public bool IsApproved { get; set; }

        public bool IsFinished { get; set; }

        public string ProblemMessage { get; set; }

        public bool IsSeenFromApprover { get; set; }

        public bool IsAssetRenovated { get; set; }

        public bool IsAssetGave { get; set; }


        public int? IssuePackingSlipId { get; set; }

        [ForeignKey("IssuePackingSlipId")]
        public virtual PackingSlip IssuePackingSlip { get; set; }


        public int? AcceptancePackingSlipId { get; set; }

        [ForeignKey("AcceptancePackingSlipId")]
        public virtual PackingSlip AcceptancePackingSlip { get; set; }


        public bool AssetIsReturned { get; set; }
    }
}
