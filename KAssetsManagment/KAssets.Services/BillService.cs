using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace KAssets.Services
{
    public interface IBillService
    {
        Bill GetBill(int id);
        bool HasOrganisationBill(int id);
        int Add(Bill model);
        void AddBillToOrganisation(int billId, int organisationId);
        void Update(Bill model);
        Bill GetById(int id);
    }

    public class BillService:BaseService,IBillService
    {
        /// <summary>
        /// Get bill by organisation id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Bill GetBill(int id)
        {
            return this.context.Organisations.Include(x=>x.Sites)
                .Include(x=>x.Bill).Where(x=>x.Id==id).FirstOrDefault().Bill;
        }

        /// <summary>
        /// Check if the organisation has bill
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HasOrganisationBill(int id)
        {
            var bill = this.context.Organisations.Find(id).Bill;
            
            if(bill==null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Add a new bill
        /// </summary>
        /// <param name="model"></param>
        public int Add(Bill model)
        {
            var bill =this.context.Bills.Add(model);
            this.context.SaveChanges();

            return bill.Id;
        }

        /// <summary>
        /// Add a bill to organisation
        /// </summary>
        /// <param name="billId"></param>
        /// <param name="organisationId"></param>
        public void AddBillToOrganisation(int billId, int organisationId)
        {
            var organisation = this.context.Organisations.Find(organisationId);
            organisation.BillId = billId;

            this.context.SaveChanges();
        }

        /// <summary>
        /// Update bill
        /// </summary>
        /// <param name="bill"></param>
        public void Update(Bill model)
        {
            var bill = this.context.Bills.Find(model.Id);
            
            bill.IBAN = model.IBAN;
            bill.CurrencyId = model.CurrencyId;
            
            this.context.SaveChanges();
        }

        /// <summary>
        /// Get bill by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Bill GetById(int id)
        {
            return this.context.Bills
                .Include(x=>x.Currency)
                .Where(x=>x.Id==id)
                .FirstOrDefault();
        }
    }
}
