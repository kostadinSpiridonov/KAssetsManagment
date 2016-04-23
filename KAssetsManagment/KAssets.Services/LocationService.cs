using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace KAssets.Services
{
    public interface ILocationService
    {
        Location Add(Location model);
        void AddLocationToUser(string email, string locationCode);
        void Update(Location model);
        Location GetById(string id);
        bool Exist(string code);
        bool ExistUpdate(string code,string oldCode);
        void Remove(string id);
        ICollection<Location> GetAll();
    }

    public class LocationService : BaseService, ILocationService
    {
        /// <summary>
        /// Add a new location
        /// </summary>
        /// <param name="model"></param>
        public Location Add(Location model)
        {
            var location = this.context.Locations.Add(model);
            this.context.SaveChanges();
          
            return location;
        }

        /// <summary>
        /// Add a location to certain user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="locationCode"></param>
        public void AddLocationToUser(string email, string locationCode)
        {
            var location = this.context.Locations.Find(locationCode);
          
            if (location == null)
            {
                var user = this.context.Users.Where(x => x.Email == email).FirstOrDefault().LocationId = null;
            }
            else
            {
                var user = this.context.Users.Where(x => x.Email == email).FirstOrDefault().LocationId = location.Code;
            }

            this.context.SaveChanges();
        }

        /// <summary>
        /// Update a location
        /// </summary>
        /// <param name="model"></param>
        public void Update(Location model)
        {
            var location = this.context.Locations.Find(model.Code);

            location.Country = model.Country;
            location.Street = model.Street;
            location.StreetNumber = model.StreetNumber;
            location.Town = model.Town;
            location.Latitude = model.Latitude;
            location.Longitude = model.Longitude;

            this.context.SaveChanges();
        }

        /// <summary>
        /// Get location by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Location GetById(string id)
        {
            return this.context.Locations.Find(id);
        }

        /// <summary>
        /// Verify whether the location exist
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool Exist(string code)
        {
            return this.context.Locations.Any(x => x.Code == code);
        }

        /// <summary>
        /// Remove a location
        /// </summary>
        /// <param name="id"></param>
        public void Remove(string id)
        {
            var location = this.context.Locations.Find(id);
            this.context.Locations.Remove(location);
         
            this.context.SaveChanges();
        }

        /// <summary>
        /// Get all locations
        /// </summary>
        /// <returns></returns>
        public ICollection<Location> GetAll()
        {
            return this.context.Locations.ToList();
        }


        /// <summary>
        /// Verify whether the location exist
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool ExistUpdate(string code,string oldCode)
        {
            var locations = this.context.Locations.Where(x => x.Code != oldCode);

            return locations.Any(x => x.Code == code);
        }
    }
}
