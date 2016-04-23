using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace KAssets.Services
{
    public interface IInvoiceService
    {
        int Add(Invoice model);
        Invoice GetById(int id);
        ICollection<Invoice> GetAll();
        void AddItems(int id, List<int> itemIds,List<int> itemCounts);
        bool Exist(string number,int orgId);
        void SetApproved(int id);
        void SetSeenByApprover(int id);
        void SetDateOfApproving(DateTime date, int id);
        void SetDateOfPayment(DateTime date, int id);
        void SetFinished(int id);
        void SetPaid(int id);
    }

    public class InvoiceService : BaseService, IInvoiceService
    {
        /// <summary>
        /// Add a new invoice
        /// </summary>
        /// <param name="model"></param>
        public int Add(Invoice model)
        {
            var invoice = this.context.Invoices.Add(model);
            this.context.SaveChanges();
         
            return invoice.Id;
        }

        /// <summary>
        /// Get a invoice by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Invoice GetById(int id)
        {
            return this.context.Invoices
                .Include(x => x.Items)
                .Include(x => x.RequestToProvider)
                .Include(x => x.Price)
                .Include(x => x.CompiledUser)
                .Include(x => x.Provider)
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Get all invoices
        /// </summary>
        /// <returns></returns>
        public ICollection<Invoice> GetAll()
        {
            return this.context.Invoices
                .Include(x => x.Items)
                .Include(x => x.RequestToProvider)
                .Include(x => x.Price)
                .Include(x => x.CompiledUser)
                .Include(x => x.Provider).ToList();
        }

        /// <summary>
        /// Add items to invoice
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemIds"></param>
       public void AddItems(int id, List<int> itemIds,List<int> itemCounts)
        {
            var invoice = this.context.Invoices.Find(id);

            for (int i = 0; i < itemIds.Count();i++ )
            {
                var itemReal = this.context.Items.Find(itemIds[i]);
               
                invoice.Items.Add(itemReal);
                invoice.CounteItems.Add(new CountItems
                    {
                        Key=itemReal.Id,
                        Want=itemCounts[i]
                    });
            }

            this.context.SaveChanges();
        }

        /// <summary>
        /// Verify whether exist invoice with specific number
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public bool Exist(string number,int orgId)
        {
            return this.context.Invoices.Where(x=>x.CompiledUser.Site.OrganisationId==orgId)
                .Any(x => x.Number == number);
        }

        /// <summary>
        /// Set the invoice is approved
        /// </summary>
        /// <param name="id"></param>
        public void SetApproved(int id)
        {
            this.context.Invoices.Find(id).IsApproved = true;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Set the invoice is seen by approver
        /// </summary>
        /// <param name="id"></param>
        public void SetSeenByApprover(int id)
        {
            this.context.Invoices.Find(id).IsSeenByApproved = true;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Set date of approving
        /// </summary>
        /// <param name="date"></param>
        /// <param name="id"></param>
        public void SetDateOfApproving(DateTime date, int id)
        {
            this.context.Invoices.Find(id).DateOfApproving = date;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Set invoice is paid
        /// </summary>
        /// <param name="id"></param>
        public void SetPaid(int id)
        {
            this.context.Invoices.Find(id).IsPaid = true;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Set date of approving
        /// </summary>
        /// <param name="date"></param>
        /// <param name="id"></param>
        public void SetDateOfPayment(DateTime date, int id)
        {
            this.context.Invoices.Find(id).DateOfPayment = date;
            this.context.SaveChanges();
        }


        /// <summary>
        /// Set invoice is finished
        /// </summary>
        /// <param name="date"></param>
        /// <param name="id"></param>
        public void SetFinished(int id)
        {
            this.context.Invoices.Find(id).Finished = true;
            this.context.SaveChanges();
        }
    }
}
