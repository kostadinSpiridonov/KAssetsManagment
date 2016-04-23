using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace KAssets.Services
{
    public interface IProviderService
    {
        void Add(Provider model);
        void Delete(int id);
        Provider GetById(int id);
        ICollection<Provider> GetAll();
        void Update(Provider model);
        Provider GetByEmail(string email);
        bool Exist(string email,int orgId);
        bool ExistUpdate(string email,int id,int orgId);
    }

    public class ProviderService:BaseService,IProviderService
    {
        /// <summary>
        /// Add a new provider
        /// </summary>
        /// <param name="model"></param>
        public void Add(Provider model)
        {
            this.context.Providers.Add(model);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Delete provider
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            this.context.Providers.Find(id).Status = "Deleted";
            this.context.SaveChanges();
        }

        /// <summary>
        /// Get provider by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Provider GetById(int id)
        {
            return this.context.Providers
                .Include(x=>x.Bill)
                .Where(x=>x.Id==id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Get all providers
        /// </summary>
        /// <returns></returns>
        public ICollection<Provider> GetAll()
        {
            return this.context.Providers
                .Include(x => x.Bill)
                .ToList();
        }

        /// <summary>
        /// Update a provider
        /// </summary>
        /// <param name="model"></param>
        public void Update(Provider model)
        {
            var provider = this.context.Providers.Find(model.Id);

            provider.Email = model.Email;
            provider.Name = model.Name;
            provider.Phone = model.Phone;
            provider.Bulstat = model.Bulstat;
            provider.Address = model.Address;

            this.context.SaveChanges();
        }

        /// <summary>
        /// Get provider by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Provider GetByEmail(string email)
        {
            return this.context.Providers
                .Include(x => x.Bill)
                .Where(x => x.Status == "Active")
                .Where(x => x.Email == email).FirstOrDefault();
        }

        /// <summary>
        /// Verify whether exist provider with email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool Exist(string email,int orgId)
        {
            return this.context.Providers
                .Where(x => x.Status == "Active")
                .Where(x=>x.OrganisationId==orgId)
                .Any(x => x.Email == email);
        }


        /// <summary>
        /// Verify whether exist provider with email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool ExistUpdate(string email, int id,int orgId)
        {
            var providers = this.context.Providers.Where(x => x.Id != id)
                .Where(x=>x.OrganisationId==orgId);

            return providers.Any(x => x.Email == email);
        }
    }
}
