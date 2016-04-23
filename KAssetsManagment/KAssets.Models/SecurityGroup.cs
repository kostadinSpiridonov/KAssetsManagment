using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAssets.Models
{
    public class SecurityGroup
    {
        private ICollection<Right> rights;

        private ICollection<ApplicationUser> applicationUsers;

        public SecurityGroup()
        {
            this.rights = new HashSet<Right>();
            this.applicationUsers = new HashSet<ApplicationUser>();
        }

        public int Id { get; set; }

        [Required]
        public string  Name { get; set; }

        public virtual ICollection<Right> Rights
        {
            get
            {
                return this.rights;
            }
            set
            {
                this.rights = value;
            }
        }

        public virtual ICollection<ApplicationUser> ApplicationUsers
        {
            get
            {
                return this.applicationUsers;
            }
            set
            {
                this.applicationUsers = value;
            }
        }
    }
}
