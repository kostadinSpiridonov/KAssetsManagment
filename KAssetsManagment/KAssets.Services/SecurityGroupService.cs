using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace KAssets.Services
{
    public interface ISecurityGroupService
    {
        ICollection<SecurityGroup> GetAll();
        SecurityGroup Add(SecurityGroup model);
        void Remove(int id);
        bool IsGroupUsed(int id);
        void Update(SecurityGroup model);
        SecurityGroup GetById(int id);
        void AddRights(int id, List<int> idS);
        void AddUserToSecurityGroups(string email, List<int> groupIds);
        void RemoveUserFromAllSecurityGroups(string userId);
        bool IsSecurityGroupExist(string name);
    }

    public class SecurityGroupService : BaseService, ISecurityGroupService
    {
        /// <summary>
        /// Get all roles
        /// </summary>
        /// <returns></returns>
        public ICollection<SecurityGroup> GetAll()
        {
            return this.context.SecurityGroups
                .Include(x => x.Rights)
                .Include(x=>x.ApplicationUsers)
                .ToList();
        }

        /// <summary>
        /// Add new role
        /// </summary>
        /// <param name="model"></param>
        public SecurityGroup Add(SecurityGroup model)
        {
            var secGroup = this.context.SecurityGroups.Add(model);
            this.context.SaveChanges();
            
            return secGroup;
        }

        /// <summary>
        /// Remove role
        /// </summary>
        /// <param name="id"></param>
        public void Remove(int id)
        {
            var group = this.context.SecurityGroups.Find(id);
            var users = this.context.Users.Include(x=>x.SecurityGroups);
            
            foreach (var item in users)
            {

                item.SecurityGroups.Remove(group);
            }
          
            this.context.SecurityGroups.Remove(group);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Check if the role is used
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsGroupUsed(int id)
        {
            return this.context.Users.Any(x => x.SecurityGroups.Any(y => y.Id == id));
        }

        /// <summary>
        /// Update a exist role
        /// </summary>
        /// <param name="model"></param>
        public void Update(SecurityGroup model)
        {
            var role = this.context.SecurityGroups.Find(model.Id);
            role.Name = model.Name;

            while (role.Rights.Count > 0)
            {
                role.Rights.Remove(role.Rights.First());
            }

            foreach (var item in model.Rights)
            {
                role.Rights.Add(this.context.Rights.Find(item.Id));
            }

            this.context.SaveChanges();
        }

        /// <summary>
        /// Get role by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SecurityGroup GetById(int id)
        {
            return this.context.SecurityGroups
                .Include(x=>x.ApplicationUsers)
                .Include(x=>x.Rights)
                .Where(x=>x.Id==id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Add rights to certain role
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idS"></param>
        public void AddRights(int id, List<int> idS)
        {
            var role = this.context.SecurityGroups.Find(id);
          
            foreach (var item in idS)
            {
                var right = this.context.Rights.Where(x => x.Id == item).First();
                role.Rights.Add(right);
            }

            this.context.SaveChanges();
        }

        /// <summary>
        /// Add user to certain security groups
        /// </summary>
        /// <param name="email"></param>
        /// <param name="groupIds"></param>
        public void AddUserToSecurityGroups(string email, List<int> groupIds)
        {
            var user = this.context.Users.Where(x => x.Email == email).FirstOrDefault();
           
            foreach (var item in groupIds)
            {
                this.context.SecurityGroups.Find(item).ApplicationUsers.Add(user);
            }

            this.context.SaveChanges();
        }

        /// <summary>
        /// Remove user from all security groups
        /// </summary>
        /// <param name="userId"></param>
        public void RemoveUserFromAllSecurityGroups(string userId)
        {
            var user = this.context.Users.Find(userId);
           
            while(user.SecurityGroups.Count>0)
            {
                this.context.SecurityGroups.Find(user.SecurityGroups.First().Id).ApplicationUsers.Remove(user);
            }

            this.context.SaveChanges();
        }

        /// <summary>
        /// Check if security group exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsSecurityGroupExist(string name)
        {
            return this.context.SecurityGroups.Any(x => x.Name == name);
        }
    }
}
