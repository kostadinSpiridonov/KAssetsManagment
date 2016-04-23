using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAssets.Models
{
    public class ExchangeRate
    {
        public int Id { get; set; }


        public int? FromId { get; set; }

        [ForeignKey("FromId")]
        public virtual Currency From { get; set; }


        public int? ToId { get; set; }

        [ForeignKey("ToId")]
        public virtual Currency To { get; set; }

        [Required]
        public double ExchangeRateValue { get; set; }


        public int OrganisationId { get; set; }

        [ForeignKey("OrganisationId")]
        public virtual Organisation Organisation { get; set; }
    }
}
