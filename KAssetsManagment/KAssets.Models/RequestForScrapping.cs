using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAssets.Models
{
    public class RequestForScrapping
    {
        public int Id { get; set; }


        public string AssetId { get; set; }

        [ForeignKey("AssetId")]
        public virtual Asset Asset { get; set; }


        public string FromId { get; set; }

        [ForeignKey("FromId")]
        public virtual ApplicationUser From { get; set; }


        public bool IsApproved { get; set; }

        public virtual DateTime DateOfSend { get; set; }

        public bool IsFinished { get; set; }
    }
}
