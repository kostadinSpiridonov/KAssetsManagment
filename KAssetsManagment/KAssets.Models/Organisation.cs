using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAssets.Models
{
    public class Organisation
    {
        public Organisation()
        {
            this.items = new HashSet<Item>();
            this.sites = new HashSet<Site>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        private ICollection<Site> sites;

        private ICollection<Item> items;

        public virtual ICollection<Site> Sites
        {
            get
            {
                return this.sites;
            }

            set
            {
                this.sites = value;
            }
        }

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

        public int? BillId { get; set; }
        public virtual Bill Bill { get; set; }

        [EmailAddress]
        [Required]
        public string EmailClient { get; set; }

        [Required]
        public string EmailClientPassword { get; set; }
    }
}
