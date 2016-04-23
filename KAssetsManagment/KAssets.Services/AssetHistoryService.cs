using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace KAssets.Services
{
    public interface IAssetHistoryService
    {
        void Add(AssetHistory model);
        void AddHistoryRow(HistoryRow model, string historyId);
        AssetHistory GetByAssetId(string assetId);
    };

    public class AssetHistoryService:BaseService,IAssetHistoryService
    {
        /// <summary>
        /// Add a new asset history 
        /// </summary>
        /// <param name="model"></param>
        public void Add(AssetHistory model)
        {
            this.context.AssetHistories.Add(model);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Add a new history row
        /// </summary>
        /// <param name="model"></param>
        /// <param name="historyId"></param>
        public void AddHistoryRow(HistoryRow model, string historyId)
        {
            var history = this.context.AssetHistories.Find(historyId);
            var row=this.context.HistoryRows.Add(model);
           
            history.Rows.Add(row);
          
            this.context.SaveChanges();
        }

        /// <summary>
        /// Get a history bu asset id
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        public AssetHistory GetByAssetId(string assetId)
        {
            return this.context.AssetHistories.Include(x=>x.Rows).Include(x=>x.Asset).Where(x => x.AssetId == assetId).FirstOrDefault();
        }
    };
}
