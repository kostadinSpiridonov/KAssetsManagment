using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAssets.Models
{
    public class ProviderItemOffer
    {
        public int Id { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public string Producer { get; set; }

        public bool IsApproved { get; set; } 

        public int Quantity { get; set; }


        public int PriceId { get; set; }

        [ForeignKey("PriceId")]
        public virtual Price Price { get; set; }
    }
}
