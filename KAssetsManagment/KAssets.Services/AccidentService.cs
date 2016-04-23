using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
namespace KAssets.Services
{
    public interface IAccidentService
    {
        void Add(Accident model);
        void SetAnswer(int id, string message,DateTime date);
        ICollection<Accident> GetAll();
        Accident GetById(int id);
        void SetSeenByUser(int id);
    }

    public class AccidentService : BaseService, IAccidentService
    {
        /// <summary>
        /// Add a new accident
        /// </summary>
        /// <param name="model"></param>
        public void Add(Accident model)
        {
            this.context.Accidents.Add(model);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Set answer to the certain accident
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        public void SetAnswer(int id, string message,DateTime date)
        {
            var accident = this.context.Accidents.Find(id);

            accident.Answer = message;
            accident.IsAnswered = true;
            accident.ReplyingDate = date;

            this.context.SaveChanges();
        }

        /// <summary>
        /// Get all accidents
        /// </summary>
        /// <returns></returns>
        public ICollection<Accident> GetAll()
        {
            return this.context.Accidents.Include(x => x.From).ToList();
        }

        /// <summary>
        /// Get accident by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Accident GetById(int id)
        {
            return this.context.Accidents.Include(x=>x.From)
                .Where(x=>x.Id==id).FirstOrDefault();
        }

        /// <summary>
        /// Set seen by user
        /// </summary>
        /// <param name="id"></param>
        public void SetSeenByUser(int id)
        {
            this.context.Accidents.Find(id).IsSeenByUser = true;
            this.context.SaveChanges();
        }
    }
}
