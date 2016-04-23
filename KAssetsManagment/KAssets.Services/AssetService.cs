using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace KAssets.Services
{
    public interface IAssetService
    {
        Asset Add(Asset model);
        bool Exist(string id);
        ICollection<Asset> GetAll();
        Asset GetById(string id);
        void Update(Asset model);
        void Relocate(string id, int siteId);
        void ChangeStatus(string id, string status);
        void ChangeLocation(string id, string code);
        void ChangeUser(string id, string userId);
    }

    public class AssetService:BaseService,IAssetService
    {
        /// <summary>
        /// Add a new asset
        /// </summary>
        /// <param name="model"></param>
        public Asset Add(Asset model)
        {
            var asset=this.context.Assets.Add(model);
            this.context.SaveChanges();
            return asset;
        }

        /// <summary>
        /// Verify whether certain asset exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Exist(string id)
        {
            return this.context.Assets.Any(x => x.InventoryNumber == id);
        }

      
        /// <summary>
        /// Get asset by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Asset GetById(string id)
        {
            return this.context.Assets
                .Include(x=>x.Item)
                .Include(x=>x.Location)
                .Include(x=>x.Site)
                .Include(x=>x.History)
                .Include(x=>x.User)
                .Include(x=>x.Price)
                .Where(x => x.InventoryNumber == id).FirstOrDefault();
        }

        /// <summary>
        /// Get all assets
        /// </summary>
        /// <returns></returns>
        public ICollection<Asset> GetAll()
        {
            return this.context.Assets
                .Include(x=>x.Item)
                .Include(x=>x.Location)
                .Include(x=>x.Site)
                .Include(x=>x.History)
                .Include(x=>x.User)
                .Include(x=>x.Price)
                .ToList();
        }

        /// <summary>
        /// Update a asset
        /// </summary>
        /// <param name="model"></param>
        public void Update(Asset model)
        {
            var asset = this.context.Assets.Find(model.InventoryNumber);
          
            asset.Producer=model.Producer;
            asset.Brand=model.Brand;
            asset.Model = model.Model;
            asset.Price.Value = model.Price.Value;
            asset.Price.CurrencyId = model.Price.CurrencyId;
            asset.Type = model.Type;

            this.context.SaveChanges();
        }

        /// <summary>
        /// Relocate the asset (change the site only)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="siteId"></param>
        public void Relocate(string id, int siteId)
        {
            this.context.Assets.Find(id).SiteId = siteId;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Change asset status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        public void ChangeStatus(string id, string status)
        {
            this.context.Assets.Find(id).Status = status;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Change location of asset
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code"></param>
        public void ChangeLocation(string id, string code)
        {
            this.context.Assets.Find(id).LocationId = code;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Change user of asset
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        public void ChangeUser(string id, string userId)
        {
            if (userId == null)
            {
                this.context.Assets.Find(id).UserId = string.Empty;
            }

            this.context.Assets.Find(id).UserId = userId;
            this.context.SaveChanges();
        }
    }
}
