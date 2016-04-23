using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
namespace KAssets.Services
{
    public interface ISiteService
    {
        void Add(Site model);
        ICollection<Site> GetForOrganisation(int id);
        ICollection<Site> GetAll();
        Site GetById(int id);
        void Update(Site model);
        bool IsExist(string name);
        bool IsExistUpdate(string name,int id);
        void Remove(int id);
        void AddUsersToSite(int siteId, List<string> userIds);
        void RemoveUserFromSite(int siteId, string userId);
    }

    public class SiteService:BaseService,ISiteService
    {
        /// <summary>
        /// Add a new site 
        /// </summary>
        /// <param name="model"></param>
        public void Add(Site model)
        {
            this.context.Sites.Add(model);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Get sites for certain organisation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ICollection<Site> GetForOrganisation(int id)
        {
            return this.context.Sites
                .Include(x => x.Organisation)
                .Include(x => x.Users)
                .Include(x => x.Assets)
                .Where(x => x.OrganisationId == id)
                .ToList();
        }

        /// <summary>
        /// Get a site by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Site GetById(int id)
        {
            return this.context.Sites
                .Include(x => x.Users)
                .Include(x => x.Organisation)
                .Include(x => x.Assets)
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Update a site
        /// </summary>
        /// <param name="model"></param>
        public void Update(Site model)
        {
            var site = this.context.Sites.Find(model.Id);
            site.Name = model.Name;
           
            this.context.SaveChanges();
        }

        /// <summary>
        /// Verify whether there is a site with tha same name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsExist(string name)
        {
            return this.context.Sites.Any(x => x.Name == name);
        }

        /// <summary>
        /// Remove a site
        /// </summary>
        /// <param name="id"></param>
        public void Remove(int id)
        {
            var site = this.context.Sites.Find(id);
            this.context.Sites.Remove(site);
        
            this.context.SaveChanges();

        }

        /// <summary>
        /// Add users to a site
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="userIds"></param>
        public void AddUsersToSite(int siteId, List<string> userIds)
        {
            var site = this.context.Sites.Find(siteId);
          
            foreach (var item in userIds)
            {
                var user = this.context.Users.Find(item);
                site.Users.Add(user);
            }

            this.context.SaveChanges();
        }

        /// <summary>
        /// Remove a user from a site
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="userId"></param>
        public void RemoveUserFromSite(int siteId, string userId)
        {
            var site = this.context.Sites.Find(siteId);
            var user = this.context.Users.Find(userId);

            site.Users.Remove(user);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Get all sites
        /// </summary>
        /// <returns></returns>
        public ICollection<Site> GetAll()
        {
            return this.context.Sites
                 .Include(x => x.Organisation)
                 .Include(x => x.Users)
                 .Include(x => x.Assets)
                 .ToList();
        }

        /// <summary>
        /// Verify whether there is a site with tha same name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsExistUpdate(string name, int id)
        {
            var sites = this.context.Sites.Where(x => x.Id != id);

            return sites.Any(x => x.Name == name);
        }
    }
}
