using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace KAssets.Services
{
    public interface IPackingSlipService
    {
        int Add(PackingSlip model);
        void SetReceived(int id, DateTime time);
        void SetGiven(int id, DateTime time);
        PackingSlip GetById(int id);
        void SetFromUser(int id, string userId);
        void SetToUser(int id, string userId);
    }

    public class PackingSlipService : BaseService, IPackingSlipService
    {
        /// <summary>
        /// Add a new packing slip
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(PackingSlip model)
        {
            var paSl = this.context.PackingSlips.Add(model);
            this.context.SaveChanges();
            return paSl.Id;
        }

        /// <summary>
        /// Set packing slip is recevied
        /// </summary>
        /// <param name="id"></param>
        public void SetReceived(int id,DateTime time)
        {
            var packSlip = this.context.PackingSlips.Find(id);
          
            packSlip.Received = true;
            packSlip.DateOfReceived = time;
            
            this.context.SaveChanges();
        }

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PackingSlip GetById(int id)
        {
            return this.context.PackingSlips
                .Include(x => x.FromUser)
                .Include(x => x.ToUser)
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Set packing slip is givven
        /// </summary>
        /// <param name="id"></param>
        public void SetGiven(int id, DateTime time)
        {
            var packSlip = this.context.PackingSlips.Find(id);

            packSlip.Given = true;
            packSlip.DateOfGiven = time;

            this.context.SaveChanges();
        }

        /// <summary>
        /// Set from user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        public void SetFromUser(int id, string userId)
        {
            this.context.PackingSlips.Find(id).FromUserId = userId;
            this.context.SaveChanges();
        }

        /// <summary>
        /// Set to user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        public void SetToUser(int id, string userId)
        {
            this.context.PackingSlips.Find(id).ToUserId = userId;
            this.context.SaveChanges();
        }
    }
}
