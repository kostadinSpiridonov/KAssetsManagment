using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAssets.Services
{
    public interface ICurrencyService
    {
        void Add(Currency model);
        Currency GetById(int id);
        ICollection<Currency> GetAll();
        void Update(Currency model);
        bool Exist(string code,int organisationId);
        bool ExistUpdate(string code, int id, int organisationId);
    }

    public class CurrencyService:BaseService,ICurrencyService
    {
        /// <summary>
        /// Add a new currency
        /// </summary>
        /// <param name="model"></param>
        public void Add(Currency model)
        {
            this.context.Currency.Add(model);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Get currency by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Currency GetById(int id)
        {
            return this.context.Currency.Find(id);
        }

        /// <summary>
        /// Get all currencies
        /// </summary>
        /// <returns></returns>
        public ICollection<Currency> GetAll()
        {
            return this.context.Currency.ToList();
        }

        /// <summary>
        /// Update a currency
        /// </summary>
        /// <param name="model"></param>
        public void Update(Currency model)
        {
            var currency = this.context.Currency.Find(model.Id);
           
            currency.Description = model.Description;
            currency.Code = model.Code;
           
            this.context.SaveChanges();
        }

        /// <summary>
        /// Verify whether there is a currency with specific code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool Exist(string code, int organisationId)
        {
            return this.context.Currency.Where(x=>x.OrganisationId==organisationId).Any(x => x.Code == code);
        }


        /// <summary>
        /// Verify whether there is a currency with specific code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool ExistUpdate(string code, int id, int organisationId)
        {
            var curr = this.context.Currency.Where(x => x.OrganisationId == organisationId).Where(x => x.Id != id);

            return curr.Any(x => x.Code == code);
        }
    }
}
