using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace KAssets.Services
{
    public interface IEventService
    {
        ICollection<Event> GetAllForUser(string userId);
        void Add(Event model);
        void AddForUserGroup(Event model, string right, int organisation);
        void SetSeenForUser(string userId);
        void SetSeen(int id);
    }

    public class EventService : BaseService, IEventService
    {
        /// <summary>
        /// Get all events for certain user
        /// </summary>
        /// <returns></returns>
        public ICollection<Event> GetAllForUser(string userId)
        {
            return this.context.Events
                .Include(x => x.User)
                .Where(x => x.UserId == userId)
                .ToList();
        }

        /// <summary>
        /// Add a new event
        /// </summary>
        /// <param name="model"></param>
        public void Add(Event model)
        {
            this.context.Events.Add(model);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Add a event to users with certain right
        /// </summary>
        /// <param name="model"></param>
        /// <param name="right"></param>
        public void AddForUserGroup(Event model, string right, int organisation)
        {
            var users = this.context.Users.Where(x => x.SecurityGroups.Count != 0)
                .Where(x=>x.Site!=null)
                .Where(x=>x.Site.OrganisationId==organisation)
                .ToList();

            foreach (var item in users)
            {
                bool conTi = false;

                //Check if the user has e security group with certain right
                foreach (var sec in item.SecurityGroups)
                {
                    if (sec.Rights.Any(x => x.Name == right))
                    {
                        conTi = true;
                    }
                }

                //If user has security group with certain right add a event to user
                if (conTi)
                {
                    var newOBJ = new Event
                    {
                        Content = model.Content,
                        Date = model.Date,
                        EventRelocationUrl = model.EventRelocationUrl
                    };
                    newOBJ.UserId = item.Id;
                    this.context.Events.Add(newOBJ);
                    this.context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Set events are seen for user
        /// </summary>
        /// <param name="userId"></param>
        public void SetSeenForUser(string userId)
        {
            var events = this.context.Events.Where(x => x.UserId == userId);

            foreach (var item in events)
            {
                item.IsSeen = true;
            }

            this.context.SaveChanges();
        }

        /// <summary>
        /// Set event is seen
        /// </summary>
        /// <param name="id"></param>
        public void SetSeen(int id)
        {
            this.context.Events.Find(id).IsSeen = true;
            this.context.SaveChanges();
        }
    }
}
