using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAssets.Models
{
    public class Asset
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string InventoryNumber { get; set; }

        [Required]
        public virtual TypesOfAsset Type { get; set; }

        public int? ItemId { get; set; }
        public virtual Item Item { get; set; }


        public string LocationId { get; set; }

        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }

        [Required]
        public int Guarantee { get; set; }


        public int? SiteId { get; set; }
        [ForeignKey("SiteId")]
        public virtual Site Site { get; set; }


        public virtual AssetHistory History { get; set; }

        public string Status { get; set; }


        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [Required]
        public string Producer { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public string Model { get; set; }


        public int? PriceId { get; set; }

        [ForeignKey("PriceId")]
        public virtual Price Price { get; set; }
    }
}
