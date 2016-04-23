using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace KAssets.Services
{
    public interface IRequestForScrappingService
    {
        void Add(RequestForScrapping model);
        bool Exist(string assetId);
        ICollection<RequestForScrapping> GetAll();
        RequestForScrapping GetById(int id);
        void ApproveRequest(int id);
        void SetFinished(int id);
    }

    public class RequestForScrappingService : BaseService, IRequestForScrappingService
    {
        /// <summary>
        /// Add a new request for scrapping
        /// </summary>
        /// <param name="model"></param>
        public void Add(RequestForScrapping model)
        {
            this.context.RequestsForScrapping.Add(model);
            this.context.SaveChanges();
        }

        /// <summary>
        /// verify whether is there a request for asset
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        public bool Exist(string assetId)
        {
            return this.context.RequestsForScrapping
                .Where(x => !x.IsFinished)
                .Any(x => x.AssetId == assetId);
        }

        /// <summary>
        /// Get all requests
        /// </summary>
        /// <returns></returns>
        public ICollection<RequestForScrapping> GetAll()
        {
            return this.context.RequestsForScrapping
                .Include(x => x.Asset)
                .Include(x => x.From)
                .ToList();
        }

        /// <summary>
        /// Get request by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RequestForScrapping GetById(int id)
        {
            return this.context.RequestsForScrapping
                .Include(x => x.Asset)
                .Include(x => x.From)
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Approve request
        /// </summary>
        /// <param name="id"></param>
        public void ApproveRequest(int id)
        {
            this.context.RequestsForScrapping.Find(id).IsApproved = true;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Set reuqest is finished
        /// </summary>
        /// <param name="id"></param>
        public void SetFinished(int id)
        {
            this.context.RequestsForScrapping.Find(id).IsFinished = true;
            this.context.SaveChanges();
        }
    }
}
