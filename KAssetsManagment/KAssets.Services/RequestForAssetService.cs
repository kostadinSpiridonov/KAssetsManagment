using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace KAssets.Services
{
    public interface IRequestForAssetService
    {
        int Add(RequestForAsset model);
        void AddAssetsToRequest(int id, List<string> assets);
        ICollection<RequestForAsset> GetAll();
        RequestForAsset GetById(int id);
        void SetFinished(int id);
        void SetNotApproved(int id);
        void SetApproved(int id);
        void AddApprovedAssets(int id, List<string> assets);
        void AddGivenAssets(int id, List<string> assets);
        void AddPackingSlip(int id, int psId);
        void SetAssetsGiven(int id);
    }

    public class RequestForAssetService:BaseService,IRequestForAssetService
    {
        /// <summary>
        /// Add a new request for asset
        /// </summary>
        /// <param name="model"></param>
        public int Add(RequestForAsset model)
        {
            var request=this.context.RequestForAssets.Add(model);
            this.context.SaveChanges();
         
            return request.Id;
        }

        /// <summary>
        /// Add assets to request
        /// </summary>
        /// <param name="id"></param>
        /// <param name="assets"></param>
        public void AddAssetsToRequest(int id, List<string> assets)
        {
            var request = this.context.RequestForAssets.Find(id);

            foreach (var item in assets)
            {
                var asset = this.context.Assets.Find(item);
                request.Assets.Add(asset);    
            }

            this.context.SaveChanges();
        }

        /// <summary>
        /// Get all request for asset
        /// </summary>
        /// <returns></returns>
        public ICollection<RequestForAsset> GetAll()
        {
            return this.context.RequestForAssets
                .Include(x => x.Assets)
                .Include(x => x.From)
                .Include(x => x.ApprovedAssets)
                .Include(x => x.GivenAssets)
                .Include(x => x.PackingSlip)
                .ToList();
        }

        /// <summary>
        /// Get request by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RequestForAsset GetById(int id)
        {
            return this.context.RequestForAssets
                .Include(x => x.Assets)
                .Include(x => x.From)
                .Include(x => x.ApprovedAssets)
                .Include(x => x.GivenAssets)
                .Include(x => x.PackingSlip)
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        /// <summary>
        /// St request is finished
        /// </summary>
        /// <param name="id"></param>
        public void SetFinished(int id)
        {
            this.context.RequestForAssets.Find(id).Finished = true;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Set request is not approved
        /// </summary>
        /// <param name="id"></param>
        public void SetNotApproved(int id)
        {
            this.context.RequestForAssets.Find(id).IsApproved = false;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Add approved assets
        /// </summary>
        /// <param name="id"></param>
        /// <param name="assets"></param>
        public void AddApprovedAssets(int id, List<string> assets)
        {
            var request = this.context.RequestForAssets.Find(id);

            foreach (var item in assets)
            {
                var asset = this.context.Assets.Find(item);
                request.ApprovedAssets.Add(asset);
            }

            this.context.SaveChanges();
        }


        /// <summary>
        /// Set request is approved
        /// </summary>
        /// <param name="id"></param>
        public void SetApproved(int id)
        {
            this.context.RequestForAssets.Find(id).IsApproved = true;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Add picking slip
        /// </summary>
        /// <param name="id"></param>
        /// <param name="psId"></param>
        public void AddPackingSlip(int id, int psId)
        {
            this.context.RequestForAssets.Find(id).PackingSlipId = psId;
            this.context.SaveChanges();
        }


        /// <summary>
        /// Set assets are given
        /// </summary>
        /// <param name="id"></param>
        public void SetAssetsGiven(int id)
        {
            this.context.RequestForAssets.Find(id).AreAssetsGave = true;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Set assets are given
        /// </summary>
        /// <param name="id"></param>
        /// <param name="assets"></param>
        public void AddGivenAssets(int id, List<string> assets)
        {
            var request = this.context.RequestForAssets.Find(id);

            foreach (var item in assets)
            {
                var asset = this.context.Assets.Find(item);
                request.GivenAssets.Add(asset);
            }

            this.context.SaveChanges();
        }
    }
}
