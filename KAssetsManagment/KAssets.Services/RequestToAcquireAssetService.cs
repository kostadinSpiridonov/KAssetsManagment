using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace KAssets.Services
{
    public interface IRequestToAcquireItemsService
    {
        int Add(RequestToAcquireItems model);
        void AddItemsToRequest(int requestId, List<int> itemsIds);
        ICollection<RequestToAcquireItems> GetAll();
        RequestToAcquireItems GetById(int id);
        void SetApproved(int requestId);
        void AddWantCountItems(Dictionary<int, int> pairs, int requestId);
        void AddGaveCountItems(Dictionary<int, int> pairs, int requestId);
        void AddApprovedCountItems(Dictionary<int, int> pairs, int requestId);
        void SetAssetsGave(int requestId);
        void ReduceItemCount(int itemId, int requesetId);
        void RemoveItemCount(int itemId, int requestId);
        void SetFinished(int requestId);
        void SetNotApproved(int id);
        void SetItemsNotGave(int id);
        void AddPackingSlip(int id, int packingSlipId);
        void AddMessage(int id, string message);
    }

    public class RequestToAcquireAssestService : BaseService, IRequestToAcquireItemsService
    {
        /// <summary>
        /// Add a new request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(RequestToAcquireItems model)
        {
            var request = this.context.RequestsToAcquireItems.Add(model);
            this.context.SaveChanges();
           
            return request.Id;
        }

        /// <summary>
        /// Add items to a certain request
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="itemsIds"></param>
        public void AddItemsToRequest(int requestId, List<int> itemsIds)
        {
            var request = this.context.RequestsToAcquireItems.Find(requestId);
         
            foreach (var id in itemsIds)
            {
                var item = this.context.Items.Find(id);
                request.Items.Add(item);
            }

            this.context.SaveChanges();
        }

        /// <summary>
        /// Get all requests
        /// </summary>
        /// <returns></returns>
        public ICollection<RequestToAcquireItems> GetAll()
        {
            return this.context.RequestsToAcquireItems
                .Include(x => x.From)
                .Include(x=>x.ToUser)
                .Include(x => x.Items)
                .Include(x => x.Location)
                .Include(x => x.CountSelectedItems)
                .Include(x => x.PackingSlip)
                .ToList();
        }

        /// <summary>
        /// Get a request by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RequestToAcquireItems GetById(int id)
        {
            return this.context.RequestsToAcquireItems
                .Include(x => x.From)
                .Include(x => x.ToUser)
                .Include(x => x.Items)
                .Include(x => x.Location)
                .Include(x => x.CountSelectedItems)
                .Include(x => x.PackingSlip)
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Set approved
        /// </summary>
        /// <param name="requestId"></param>
        public void SetApproved(int requestId)
        {
            this.context.RequestsToAcquireItems.Find(requestId).IsApproved = true;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Set not approved
        /// </summary>
        /// <param name="requestId"></param>
        public void SetNotApproved(int id)
        {
             this.context.RequestsToAcquireItems.Find(id).IsApproved = false;
            this.context.SaveChanges();
        }
      
        /// <summary>
        /// Add count of selected offers items
        /// </summary>
        /// <param name="pairs"></param>
        public void AddWantCountItems(Dictionary<int, int> pairs, int requestId)
        {
            var request = this.context.RequestsToAcquireItems.Find(requestId);

            foreach (var item in pairs)
            {
                request.CountSelectedItems.Add(new CountItems
                    {
                        Key = item.Key,
                        Want = item.Value
                    });
            }
            this.context.SaveChanges();
        }

        /// <summary>
        /// Set the assets are gave
        /// </summary>
        /// <param name="requestId"></param>
        public void SetAssetsGave(int requestId)
        {
            this.context.RequestsToAcquireItems.Find(requestId).AreItemsGave = true;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Reduce item quantity from request
        /// </summary>
        /// <param name="itemId"></param>
        public void ReduceItemCount(int itemId, int requestId)
        {
            var request = this.context.RequestsToAcquireItems.Find(requestId);
            request.CountSelectedItems.Where(x => x.Key == itemId).FirstOrDefault().Want--;
        
            this.context.SaveChanges();
        }

        /// <summary>
        /// Remove item count
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="requestId"></param>
        public void RemoveItemCount(int itemId, int requestId)
        {
            var request = this.context.RequestsToAcquireItems.Find(requestId);
            var itemCount = request.CountSelectedItems.Where(x => x.Key == itemId).FirstOrDefault();
            this.context.CountItems.Remove(itemCount);
          
            this.context.SaveChanges();
        }

        /// <summary>
        /// Add gave count of items
        /// </summary>
        /// <param name="pairs"></param>
        /// <param name="requestId"></param>
        public void AddGaveCountItems(Dictionary<int, int> pairs, int requestId)
        {
            var request = this.context.RequestsToAcquireItems.Find(requestId);
            foreach (var item in pairs)
            {
                request.CountSelectedItems.Where(x => x.Key == item.Key).FirstOrDefault().Give = item.Value;
            }
            this.context.SaveChanges();
        }

        /// <summary>
        /// Set request is finished
        /// </summary>
        /// <param name="requestId"></param>
        public void SetFinished(int requestId)
        {
            this.context.RequestsToAcquireItems.Find(requestId).Finished = true;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Add approved count of items
        /// </summary>
        /// <param name="pairs"></param>
        /// <param name="requestId"></param>
        public void AddApprovedCountItems(Dictionary<int, int> pairs, int requestId)
        {
            var request = this.context.RequestsToAcquireItems.Find(requestId);
          
            foreach (var item in pairs)
            {
                request.CountSelectedItems.Where(x => x.Key == item.Key).FirstOrDefault().Approved = item.Value;
            }

            this.context.SaveChanges();
        }

        /// <summary>
        /// Set items not gave
        /// </summary>
        /// <param name="id"></param>
        public void SetItemsNotGave(int id)
        {
            this.context.RequestsToAcquireItems.Find(id).AreItemsGave = false;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Add a packing slip
        /// </summary>
        /// <param name="id"></param>
        /// <param name="packingSlipId"></param>
        public void AddPackingSlip(int id, int packingSlipId)
        {
            this.context.RequestsToAcquireItems.Find(id).PackingSlipId = packingSlipId;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Add message to request
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        public void AddMessage(int id, string message)
        {
            this.context.RequestsToAcquireItems.Find(id).Message = message;
            this.context.SaveChanges();
        }
    }
}
