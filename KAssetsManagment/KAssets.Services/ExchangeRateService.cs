using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace KAssets.Services
{
    public interface IExchangeRateService
    {
        void Add(ExchangeRate model);
        ExchangeRate GetById(int id);
        ICollection<ExchangeRate> GetAll();
        bool Exist(int firstId, int secondId,int orgId);
        void Update(ExchangeRate model);
        double GetRate(int firstId, string userId);
        double GetRate(int firstId, int secondId);
        double GetRateNotation(int firstId, string nitation);
    }

    public class ExchangeRateService : BaseService, IExchangeRateService
    {
        /// <summary>
        /// Add a new exchange rate
        /// </summary>
        /// <param name="model"></param>
        public void Add(ExchangeRate model)
        {
            this.context.ExchangeRates.Add(model);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Get exchange rate by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ExchangeRate GetById(int id)
        {
            return this.context.ExchangeRates
                .Include(x=>x.To)
                .Include(x=>x.From)
                .Where(x=>x.Id==id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Get all exchange rates
        /// </summary>
        /// <returns></returns>
        public ICollection<ExchangeRate> GetAll()
        {
            return this.context.ExchangeRates
                .Include(x=>x.To)
                .Include(x=>x.From)
                .ToList();
        }

        /// <summary>
        /// Verify whether exist a exchange rate with specific codes
        /// </summary>
        /// <param name="code1"></param>
        /// <param name="code2"></param>
        /// <returns></returns>
        public bool Exist(int firstId, int secondId,int orgId)
        {
            var first = this.context.ExchangeRates.Where(x=>x.OrganisationId==orgId).Any(x => x.FromId == firstId && x.ToId == secondId);
            var second = this.context.ExchangeRates.Where(x => x.OrganisationId == orgId).Any(x => x.FromId == secondId && x.ToId == firstId);

            if (first || second)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Update exchange rate
        /// </summary>
        /// <param name="model"></param>
        public void Update(ExchangeRate model)
        {
            var exRate = this.context.ExchangeRates.Find(model.Id);
           
            exRate.ToId = model.ToId;
            exRate.FromId = model.FromId;
            exRate.ExchangeRateValue = model.ExchangeRateValue;

            this.context.SaveChanges();
        }

        /// <summary>
        /// Get exchange rate by id and user
        /// </summary>
        /// <param name="firstId"></param>
        /// <param name="secondId"></param>
        /// <returns></returns>
        public double GetRate(int firstId, string userId)
        {
            int secondId = 1;
            
            //Get the user' organisation base currency
            if (this.context.Users.Find(userId).Site.Organisation.Bill != null)
            {
                secondId = this.context.Users.Find(userId).Site.Organisation.Bill.CurrencyId;
            }
            else
            {
                secondId = this.context.Currency.First().Id;
            }

            //Get exchange rate between user'organisation base currency and currency with id "firstId"
            if (this.context.ExchangeRates.Any(x => x.FromId.Value == firstId && x.ToId == secondId))
            {
                return this.context.ExchangeRates.Where(x => x.FromId.Value == firstId && x.ToId == secondId).First().ExchangeRateValue;
            }

            if (this.context.ExchangeRates.Any(x => x.FromId.Value == secondId && x.ToId == firstId))
            {
                return 1 / (this.context.ExchangeRates.Where(x => x.FromId.Value == secondId && x.ToId == firstId).First().ExchangeRateValue);
            }
            return (double)1;
        }

        /// <summary>
        /// Get exchange rate if and notation
        /// </summary>
        /// <param name="firstId"></param>
        /// <param name="secondId"></param>
        /// <returns></returns>
        public double GetRateNotation(int firstId,string notation)
        {
            var firstCurrencyOrganisation = this.context.Currency.Find(firstId).OrganisationId;
            //Get currency by "notation"
            int secondId = this.context.Currency.Where(x => x.Code == notation)
                .Where(x=>x.OrganisationId==firstCurrencyOrganisation).FirstOrDefault().Id;

            if (secondId != 0)
            {
                //Get exchange rate value by two currency (firstId, secondId)
                if (this.context.ExchangeRates.Any(x => x.FromId.Value == firstId && x.ToId == secondId))
                {
                   return  1 /this.context.ExchangeRates.Where(x => x.FromId.Value == firstId && x.ToId == secondId).First().ExchangeRateValue;
                }

                if (this.context.ExchangeRates.Any(x => x.FromId.Value == secondId && x.ToId == firstId))
                {
                    return  (this.context.ExchangeRates.Where(x => x.FromId.Value == secondId && x.ToId == firstId).First().ExchangeRateValue);
                }
            }
            return (double)1;
        }

        /// <summary>
        /// Get exchange rate by two currencies
        /// </summary>
        /// <param name="firstId"></param>
        /// <param name="secondId"></param>
        /// <returns></returns>
        public double GetRate(int firstId, int secondId)
        {
            if (this.context.ExchangeRates.Any(x => x.FromId.Value == firstId && x.ToId == secondId))
            {
                return this.context.ExchangeRates.Where(x => x.FromId.Value == firstId && x.ToId == secondId).First().ExchangeRateValue;
            }

            if (this.context.ExchangeRates.Any(x => x.FromId.Value == secondId && x.ToId == firstId))
            {
                return 1 / (this.context.ExchangeRates.Where(x => x.FromId.Value == secondId && x.ToId == firstId).First().ExchangeRateValue);
            }
            return (double)1;
        }
    }
}
