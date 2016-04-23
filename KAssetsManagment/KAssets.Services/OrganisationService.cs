using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace KAssets.Services
{
    public interface IOrganisationService
    {
        ICollection<Organisation> GetAll();
        void Add(Organisation model);
        Organisation GetById(int id);
        void Update(Organisation model);
        void Remove(int id);
        bool IsExist(string name);
        bool IsExistUpdate(string name,int id);
    }

    public class OrganisationService : BaseService, IOrganisationService
    {
        /// <summary>
        /// Get all organisations
        /// </summary>
        /// <returns></returns>
        public ICollection<Organisation> GetAll()
        {
            return this.context.Organisations
                .Include(x=>x.Bill)
                .Include(x=>x.Sites)
                .Include(x=>x.Items)
                .ToList();
        }

        /// <summary>
        /// Add a new organisation
        /// </summary>
        /// <param name="model"></param>
        public void Add(Organisation model)
        {
            this.context.Organisations.Add(model);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Get organisation by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Organisation GetById(int id)
        {
            return this.context.Organisations
                .Include(x=>x.Bill)
                .Include(x => x.Sites)
                .Include(x => x.Items)
                .Where(x=>x.Id==id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Update a organisation
        /// </summary>
        /// <param name="model"></param>
        public void Update(Organisation model)
        {
            var organisation = this.context.Organisations.Find(model.Id);
            organisation.Name = model.Name;
            organisation.EmailClient = model.EmailClient;
            organisation.EmailClientPassword = model.EmailClientPassword;
            organisation.Address = model.Address;

            this.context.SaveChanges();
        }

        /// <summary>
        /// Remove a organisation
        /// </summary>
        /// <param name="id"></param>
        public void Remove(int id)
        {
            var organisation = this.context.Organisations.Find(id);
            this.context.Organisations.Remove(organisation);
       
            this.context.SaveChanges();
        }

        /// <summary>
        /// Verify whether there is organisation with the same name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsExist(string name)
        {
            return this.context.Organisations.Any(x => x.Name == name);
        }

        /// <summary>
        /// Verify whether there is organisation with the same name 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsExistUpdate(string name,int id)
        {
            var organisations=this.context.Organisations.Where(x => x.Id != id);
            return organisations.Any(x => x.Name == name);
        }
    }
}
