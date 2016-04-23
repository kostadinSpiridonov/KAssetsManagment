using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAssets.Models
{
    public class Item
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public virtual DateTime DateOfManufacture { get; set; }

        [Required]
        public string Producer { get; set; }

        [Required]
        public string Brand { get; set; }
        [Required]
        public string Model { get; set; }


        public int PriceId { get; set; }

        [ForeignKey("PriceId")]
        public virtual Price Price { get; set; }


        public bool RotatingItem { get; set; }

        public int Quantity { get; set; }

        public int OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }

        [Required]
        public string Status { get; set; }


    }
}
