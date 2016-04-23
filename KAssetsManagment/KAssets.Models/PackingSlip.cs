using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAssets.Models
{
    public class PackingSlip
    {
        public int Id { get; set; }


        public string FromUserId { get; set; }

        [ForeignKey("FromUserId")]
        public virtual ApplicationUser FromUser { get; set; }


        public string ToUserId { get; set; }

        [ForeignKey("ToUserId")]
        public virtual ApplicationUser ToUser { get; set; }


        public bool Given { get; set; }

        public bool Received { get; set; }

        public virtual DateTime DateOfGiven { get; set; }

        public virtual DateTime DateOfReceived { get; set; }
    }
}
