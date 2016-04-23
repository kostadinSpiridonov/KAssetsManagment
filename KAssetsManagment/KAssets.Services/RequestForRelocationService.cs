using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace KAssets.Services
{
    public interface IRequestForRelocationService
    {
        void Add(RequestForRelocation model);
        ICollection<RequestForRelocation> GetAll();
        RequestForRelocation GetById(int id);
        void SetFinished(int id);
        void SetApproved(int id);
        bool Exist(string assetId);
        void SetAssetGiven(int id);
        void SetSeenFromApprover(int id);
        void AddPackingSlip(int id, int psId);
    }

    public class RequestForRelocationService : BaseService, IRequestForRelocationService
    {
        /// <summary>
        /// Add a request for relocation
        /// </summary>
        /// <param name="model"></param>
        public void Add(RequestForRelocation model)
        {
            this.context.RequestsForRelocation.Add(model);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Get all requests for relocation
        /// </summary>
        /// <returns></returns>
        public ICollection<RequestForRelocation> GetAll()
        {
            return this.context.RequestsForRelocation
                .Include(x => x.Asset)
                .Include(x => x.From)
                .Include(x => x.ToSite)
                .Include(x => x.ToUser)
                .Include(x => x.ToLocation)
                .Include(x => x.PackingSlip)
                .ToList();
        }

        /// <summary>
        /// Get request by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RequestForRelocation GetById(int id)
        {
            return this.context.RequestsForRelocation
                .Include(x => x.Asset)
                .Include(x => x.From)
                .Include(x => x.ToSite)
                .Include(x => x.ToUser)
                .Include(x => x.ToLocation)
                .Include(x => x.PackingSlip)
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Set request is finished
        /// </summary>
        /// <param name="id"></param>
        public void SetFinished(int id)
        {
            this.context.RequestsForRelocation.Find(id).IsFinished = true;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Set request is approved
        /// </summary>
        /// <param name="id"></param>
        public void SetApproved(int id)
        {
            this.context.RequestsForRelocation.Find(id).IsApproved = true;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Verify whether the request exist
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        public bool Exist(string assetId)
        {
            return this.context.RequestsForRelocation
                .Where(x => !x.IsFinished)
                .Any(x => x.AssetId == assetId);
        }

        /// <summary>
        /// Set assets are given
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public void SetAssetGiven(int id)
        {
            this.context.RequestsForRelocation.Find(id).AreAssetGiven = true;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Add a packing slip to request
        /// </summary>
        /// <param name="id"></param>
        /// <param name="psId"></param>
        public void AddPackingSlip(int id, int psId)
        {
            this.context.RequestsForRelocation.Find(id).PackingSlipId = psId;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Set seen from approver
        /// </summary>
        /// <param name="id"></param>
        public void SetSeenFromApprover(int id)
        {
            this.context.RequestsForRelocation.Find(id).SeenFromApprover = true;
            this.context.SaveChanges();
        }
    }
}
