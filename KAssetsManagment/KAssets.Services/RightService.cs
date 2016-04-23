using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace KAssets.Services
{
    public interface IRightService
    {
        ICollection<Right> GetAll();
        Right GetById(int id);
        bool IsUserHasRightByEmail(string email, string rightName);
        bool IsUserHasRightById(string userId, string rightName);
        bool IsRightExist(string name);
    }

    public class RightService : BaseService, IRightService
    {
        /// <summary>
        /// Get all rights
        /// </summary>
        /// <returns></returns>
        public ICollection<Right> GetAll()
        {
            return this.context.Rights.ToList();
        }

        /// <summary>
        /// Get right by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Right GetById(int id)
        {
            return this.context.Rights.Find(id);
        }

        /// <summary>
        /// Check if the user has certain right by email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="rightName"></param>
        /// <returns></returns>
        public bool IsUserHasRightByEmail(string email, string rightName)
        {
            var user = this.context.Users.Where(x => x.Email == email).FirstOrDefault();
         
            foreach (var item in user.SecurityGroups)
            {
                if (item.Rights.Select(x => x.Name).Any(x => x == rightName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///  Check if the user has certain right by id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="rightName"></param>
        /// <returns></returns>
        public bool IsUserHasRightById(string userId, string rightName)
        {
            var user = this.context.Users.Where(x => x.Id == userId).FirstOrDefault();
           
            foreach (var item in user.SecurityGroups)
            {
                if (item.Rights.Select(x => x.Name).Any(x => x == rightName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if the right exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsRightExist(string name)
        {
            return this.context.Rights.Any(x => x.Name == name);
        }
    }
}
