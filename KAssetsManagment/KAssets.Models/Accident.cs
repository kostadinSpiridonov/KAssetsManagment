using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAssets.Models
{
    public class Accident
    {
        public int Id { get; set; }


        [Required]
        public string FromId { get; set; }

        [ForeignKey("FromId")]
        public virtual ApplicationUser From { get; set; }

        [Required]
        public string Message { get; set; }

        public string Answer { get; set; }

        public bool IsAnswered { get; set; }

        [Required]
        public virtual DateTime DateOfSend { get; set; }

        public virtual DateTime ReplyingDate { get; set; }

        public bool IsSeenByUser { get; set; }
    }
}
