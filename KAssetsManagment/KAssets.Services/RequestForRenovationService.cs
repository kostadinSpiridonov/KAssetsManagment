using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace KAssets.Services
{
    public interface IRequestForRenovationService
    {
        void Add(RequestForRenovation model);
        bool Exist(string assetId);
        ICollection<RequestForRenovation> GetAll();
        RequestForRenovation GetById(int id);
        void SetFinished(int id);
        void SetApproved(int id);
        void SetIsSeenFromApprover(int id);
        void SetIsRenovated(int id);
        void SetIsAssetGave(int id);
        void AddIssuePackingSlip(int id, int pkId);
        void AddAcceptancePackingSlip(int id, int pkId);
        void SetReturned(int id);
    }

    public class RequestForRenovationService:BaseService,IRequestForRenovationService
    {
        /// <summary>
        /// Add a new renovation request
        /// </summary>
        /// <param name="model"></param>
        public void Add(RequestForRenovation model)
        {
            this.context.RequestsFоrRenovation.Add(model);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Verify whether the certain request exist
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        public bool Exist(string assetId)
        {
            return this.context.RequestsFоrRenovation.Any(x => x.AssetId == assetId&&!x.IsFinished);
        }

        /// <summary>
        /// Get all requests
        /// </summary>
        /// <returns></returns>
        public ICollection<RequestForRenovation> GetAll()
        {
            return this.context.RequestsFоrRenovation
                .Include(x => x.From)
                .Include(x => x.Asset)
                .Include(x => x.IssuePackingSlip)
                .Include(x => x.AcceptancePackingSlip)
                .ToList();
        }

        /// <summary>
        /// Get request by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RequestForRenovation GetById(int id)
        {
            return this.context.RequestsFоrRenovation
                .Include(x => x.From)
                .Include(x => x.Asset)
                .Include(x => x.IssuePackingSlip)
                .Include(x => x.AcceptancePackingSlip)
                .Where(x => x.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Set request is finished
        /// </summary>
        /// <param name="id"></param>
        public void SetFinished(int id)
        {
            this.context.RequestsFоrRenovation.Find(id).IsFinished = true;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Set request is approved
        /// </summary>
        /// <param name="id"></param>
        public void SetApproved(int id)
        {
            this.context.RequestsFоrRenovation.Find(id).IsApproved = true;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Set request is seen from approver
        /// </summary>
        /// <param name="id"></param>
        public void SetIsSeenFromApprover(int id)
        {
            this.context.RequestsFоrRenovation.Find(id).IsSeenFromApprover = true;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Set asset is renovated
        /// </summary>
        /// <param name="id"></param>
        public void SetIsRenovated(int id)
        {
            this.context.RequestsFоrRenovation.Find(id).IsAssetRenovated = true;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Set asset is gave
        /// </summary>
        /// <param name="id"></param>
        public void SetIsAssetGave(int id)
        {
            this.context.RequestsFоrRenovation.Find(id).IsAssetGave = true;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Add issue packing slip
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pkId"></param>
        public void AddIssuePackingSlip(int id, int pkId)
        {
            this.context.RequestsFоrRenovation.Find(id).IssuePackingSlipId = pkId;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Add acceptance packing slip
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pkId"></param>
        public void AddAcceptancePackingSlip(int id, int pkId)
        {
            this.context.RequestsFоrRenovation.Find(id).AcceptancePackingSlipId = pkId;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Set returned
        /// </summary>
        /// <param name="id"></param>
        public void SetReturned(int id)
        {
            this.context.RequestsFоrRenovation.Find(id).AssetIsReturned = true;
            this.context.SaveChanges();
        }
    }
}
