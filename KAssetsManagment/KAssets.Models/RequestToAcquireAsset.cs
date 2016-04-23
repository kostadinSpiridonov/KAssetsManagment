using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAssets.Models
{
    public class RequestToAcquireItems
    {
        public int Id { get; set; }

        public string Message { get; set; }

        private ICollection<CountItems> countSelectedItem;

        public virtual ICollection<CountItems> CountSelectedItems
        {
            get
            {
                return this.countSelectedItem;
            }

            set
            {
                this.countSelectedItem = value;
            }
        }

        public RequestToAcquireItems()
        {
            this.items = new HashSet<Item>();
            this.countSelectedItem = new HashSet<CountItems>();
        }

        private ICollection<Item> items;


        [Required]
        public string FromId { get; set; }

        [ForeignKey("FromId")]
        public virtual ApplicationUser From { get; set; }


        public virtual ICollection<Item> Items
        {
            get
            {
                return this.items;
            }
            set
            {
                this.items = value;
            }
        }

        public virtual DateTime DateOfSend { get; set; }

        public bool IsApproved { get; set; }

        public bool AreItemsGave { get; set; }

        public bool Finished { get; set; }


        public string LocationId { get; set; }

        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }


        public string ToUserId { get; set; }

        [ForeignKey("ToUserId")]
        public virtual ApplicationUser ToUser { get; set; }


        public int? PackingSlipId { get; set; }

        [ForeignKey("PackingSlipId")]
        public virtual PackingSlip PackingSlip { get; set; }
    }
}
