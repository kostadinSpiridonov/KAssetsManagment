using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAssets.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        [Required]
        public string Number { get; set; }

        public int? RequestToProviderId { get; set; }

        [ForeignKey("RequestToProviderId")]
        public virtual RequestToProvider RequestToProvider { get; set; }

        public Invoice()
        {
            this.items = new HashSet<Item>();
            this.countItems = new HashSet<CountItems>();
        }

        private ICollection<CountItems> countItems;

        public virtual ICollection<CountItems> CounteItems
        {
            get
            {
                return this.countItems;
            }
            set
            {
                this.countItems = value;
            }
        }

        private ICollection<Item> items;

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

       
        public int PriceId { get; set; }

        [ForeignKey("PriceId")]
        public virtual Price Price { get; set; }


        public string CompiledUserId { get; set; }

        [ForeignKey("CompiledUserId")]
        public virtual ApplicationUser CompiledUser { get; set; }


        public int? ProviderId { get; set; }

        [ForeignKey("ProviderId")]
        public virtual Provider Provider { get; set; }

        [Required]
        public string RecipientMOL { get; set; }

        public bool IsPaid { get; set; }

        public bool IsApproved { get; set; }

        public bool IsSeenByApproved { get; set; }

        public bool Finished { get; set; }

        [Required]
        public virtual DateTime DateOfCreation { get; set; }

        public virtual DateTime DateOfApproving { get; set; }

        public virtual DateTime PaymentPeriod { get; set; }

        public virtual DateTime DateOfPayment { get; set; }
    }
}
