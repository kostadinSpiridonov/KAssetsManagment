using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace KAssets.Models
{
    public class ApplicationUser : IdentityUser
    {
        private ICollection<SecurityGroup> securityGroups;

        private ICollection<Event> events;

        public ApplicationUser()
        {
            this.securityGroups = new HashSet<SecurityGroup>();
            this.events = new HashSet<Event>();
        }

        public string Status { get; set; }

        public string FirstName { get; set; }

        public string SecondName { get; set; }

        public string LastName { get; set; }

        public string AboutMe { get; set; }

        public string Skype { get; set; }

        public virtual ICollection<SecurityGroup> SecurityGroups
        {
            get
            {
                return this.securityGroups;
            }
            set
            {
                this.securityGroups = value;
            }
        }

        public virtual ICollection<Event> Events
        {
            get
            {
                return this.events;
            }
            set
            {
                this.events = value;
            }
        }

       
        public string LocationId { get; set; }

        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }


        public int? SiteId { get; set; }
        public virtual Site Site { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}
