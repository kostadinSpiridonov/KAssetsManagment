using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace KAssets.Services
{
    public interface IRequestToProviderService
    {
        int Add(RequestToProvider model);
        void AddOffer(ProviderItemOffer model, int requestId);
        ICollection<RequestToProvider> GetAll();
        RequestToProvider GetById(int id);
        void SetFinished(int id);
        void SetApproved(int id);
        void SetIsSeenByApproved(int id);
        void SetApprovedOffer(int id, List<int> idS);
        void AddWantItems(int id, List<int> idS);
        void AddGiveItems(int id, List<int> idS);
        void AddCountWantItems(int id, Dictionary<int, int> idSCount);
    }

    public class RequestToProviderService : BaseService, IRequestToProviderService
    {
        /// <summary>
        /// Add a new request
        /// </summary>
        /// <param name="model"></param>
        public int Add(RequestToProvider model)
        {
            var po = this.context.RequestsToProvider.Add(model);
            this.context.SaveChanges();
          
            return po.Id;
        }

        /// <summary>
        /// Add a offer to request
        /// </summary>
        /// <param name="model"></param>
        /// <param name="requestId"></param>
        public void AddOffer(ProviderItemOffer model, int requestId)
        {
            var offer = this.context.ProviderItemOffers.Add(model);

            this.context.RequestsToProvider.Find(requestId).SendOffers.Add(offer);
           
            this.context.SaveChanges();
        }

        /// <summary>
        /// Get all requests
        /// </summary>
        /// <returns></returns>
        public ICollection<RequestToProvider> GetAll()
        {
            return this.context.RequestsToProvider
                .Include(x => x.From)
                .Include(x => x.Provider)
                .Include(x => x.SendOffers)
                .Include(x => x.CountItems)
                .Include(x => x.WantItems)
                .Include(x => x.GiveItems)
                .ToList();
        }

        /// <summary>
        /// Get request by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RequestToProvider GetById(int id)
        {
            return this.context.RequestsToProvider
                .Include(x => x.From)
                .Include(x => x.Provider)
                .Include(x => x.SendOffers)
                .Include(x => x.CountItems)
                .Include(x => x.WantItems)
                .Include(x => x.GiveItems)
                .ToList().Where(x => x.Id == id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Set request is finished
        /// </summary>
        /// <param name="id"></param>
        public void SetFinished(int id)
        {
            this.context.RequestsToProvider.Find(id).IsFinished = true;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Set request is approved
        /// </summary>
        /// <param name="id"></param>
        public void SetApproved(int id)
        {
            this.context.RequestsToProvider.Find(id).IsApproved = true;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Set request is seen by approver
        /// </summary>
        /// <param name="id"></param>
        public void SetIsSeenByApproved(int id)
        {
            this.context.RequestsToProvider.Find(id).IsSeenByApprover = true;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Set approved offer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="offerId"></param>
        public void SetApprovedOffer(int id, List<int> idS)
        {
            foreach (var item in idS)
            {
                this.context.RequestsToProvider.Find(id).SendOffers.Where(x => x.Id == item).FirstOrDefault().IsApproved = true;
            }

            this.context.SaveChanges();
        }

        /// <summary>
        /// Add wants items
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idS"></param>
        public void AddWantItems(int id, List<int> idS)
        {
            var po = this.context.RequestsToProvider.Find(id);

            foreach (var item in idS)
            {
                var realItem = this.context.Items.Find(item);
                po.WantItems.Add(realItem);
            }

            this.context.SaveChanges();
        }

        /// <summary>
        /// Add cont wants items
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idSCount"></param>
        public void AddCountWantItems(int id, Dictionary<int, int> idSCount)
        {
            var po = this.context.RequestsToProvider.Find(id);

            foreach (var item in idSCount)
            {
                po.CountItems.Add(new CountItems
                    {
                        Key = item.Key,
                        Want = item.Value
                    });
            }

            this.context.SaveChanges();
        }

        /// <summary>
        /// Add give items
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idS"></param>
        public void AddGiveItems(int id, List<int> idS)
        {
            var po = this.context.RequestsToProvider.Find(id);

            foreach (var item in idS)
            {
                var realItem = this.context.Items.Find(item);
                po.GiveItems.Add(realItem);
                po.CountItems.Add(
                    new CountItems
                    {
                        Key = realItem.Id,
                        Give = realItem.Quantity
                    });
            }

            this.context.SaveChanges();
        }
    }
}
