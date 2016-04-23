using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace KAssets.Services
{
    public interface IUserService
    {
        ICollection<ApplicationUser> GetAll();
        ApplicationUser GetById(string id);
        void Update(ApplicationUser user);
        void Block(string userId);
        void UnBlock(string userId);
        void UserSiteAndOrgUpdate(string email, int organisationId, int siteId);
        ApplicationUser GetByEmail(string email);
        int GetUserOrganisationId(string userId);
    }

    public class UserService : BaseService, IUserService
    {
        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns></returns>
        public ICollection<ApplicationUser> GetAll()
        {
            return this.context.Users
                .Include(x => x.SecurityGroups)
                .Include(x => x.Location)
                .Include(x => x.Events)
                .Include(x => x.Site)
                .ToList();
        }

        /// <summary>
        /// Get a user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ApplicationUser GetById(string id)
        {
            return this.context.Users
                .Include(x => x.SecurityGroups)
                .Include(x => x.Location)
                .Include(x => x.Events)
                .Include(x => x.Site)
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="user"></param>
        public void Update(ApplicationUser user)
        {
            var findUser = this.context.Users.Where(x => x.Id == user.Id).FirstOrDefault();
         
            findUser.AboutMe = user.AboutMe;
            findUser.Email = user.Email;
            findUser.FirstName = user.FirstName;
            findUser.LastName = user.LastName;
            findUser.SecondName = user.SecondName;
            findUser.Skype = user.Skype;
            findUser.PasswordHash = user.PasswordHash;
           
            this.context.SaveChanges();
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="userId"></param>
        public void Block(string userId)
        {
            var user = this.context.Users.Find(userId);
            
            user.Status="Blocked";
            
            while(user.SecurityGroups.Count!=0)
            {
                user.SecurityGroups.Remove(user.SecurityGroups.First());
            }
            
            this.context.SaveChanges();

        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="userId"></param>
        public void UnBlock(string userId)
        {
            var user = this.context.Users.Find(userId);

            user.Status = "Active";

            while (user.SecurityGroups.Count != 0)
            {
                this.context.SecurityGroups.Remove(user.SecurityGroups.First());
            }

            this.context.SaveChanges();

        }

        /// <summary>
        /// Set user organisation and site
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="organisationId"></param>
        /// <param name="siteId"></param>
        public void UserSiteAndOrgUpdate(string email, int organisationId, int siteId)
        {
            var user = this.context.Users.Where(x => x.Email == email).FirstOrDefault();
           
            if (siteId != 0)
            {
                user.SiteId = siteId;
            }
            else
            {
                user.SiteId =new  Nullable<int>();
            }

            this.context.SaveChanges();
        }

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public ApplicationUser GetByEmail(string email)
        {
            return this.context.Users
                .Include(x => x.SecurityGroups)
                .Include(x => x.Location)
                .Include(x => x.Events)
                .Include(x => x.Site)
                .Where(x => x.Email == email)
                .FirstOrDefault();
        }

        /// <summary>
        /// Get user organisation id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetUserOrganisationId(string userId)
        {
            var user = this.context.Users.Find(userId);
            if(user.Site==null)
            {
                return 0;
            }
            return user.Site.OrganisationId;
        }
    }
}
