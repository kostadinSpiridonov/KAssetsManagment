using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAssets.Models
{
    public class Site
    {
        private ICollection<ApplicationUser> users;

        private ICollection<Asset> assets;

        public Site()
        {
            this.users= new HashSet<ApplicationUser>();
            this.assets = new HashSet<Asset>();
        }


        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<ApplicationUser> Users
        {
            get
            {
                return this.users;
            }

            set
            {
                this.users = value;
            }
        }


        public virtual ICollection<Asset> Assets
        {
            get
            {
                return this.assets;
            }

            set
            {
                this.assets = value;
            }
        }

        public int OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }


    }
}
