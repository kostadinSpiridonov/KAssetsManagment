using KAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace KAssets.Services
{
    public interface IItemService
    {
        ICollection<Item> GetItemsForOrganisaion(int id);
        Item Add(Item model);
        Item GetById(int id);
        void Update(Item model);
        void Delete(int id);
        ICollection<Item> GetAll();
        ICollection<Item> GetAllByBrandAndModel(string brand, string model);
        void ReduceItemQuantity(int itemId);
        bool Exist(int id);
    }

    public class ItemService:BaseService,IItemService
    {
        /// <summary>
        /// Get all item for certain organiastion
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ICollection<Item> GetItemsForOrganisaion(int id)
        {
            return this.context.Organisations.Find(id).Items.ToList();
        }

        /// <summary>
        /// Add a new item
        /// </summary>
        /// <param name="model"></param>
        public Item Add(Item model)
        {
            var item=this.context.Items.Add(model);
            this.context.SaveChanges();
         
            return item;
        }

        /// <summary>
        /// Get a item by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Item GetById(int id)
        {
            return this.context.Items
                .Include(x=>x.Organisation)
                .Include(x=>x.Price)
                .Where(x=>x.Id==id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Update a item
        /// </summary>
        /// <param name="model"></param>
        public void Update(Item model)
        {
            var item = this.context.Items.Find(model.Id);
        
            item.Brand = model.Brand;
            item.DateOfManufacture = model.DateOfManufacture;
            item.Model = model.Model;
            item.Producer = model.Producer;
            item.Quantity = model.Quantity;
            item.RotatingItem = model.RotatingItem;
            item.Price.CurrencyId = model.Price.CurrencyId;
            item.Price.Value = model.Price.Value;
       
            this.context.SaveChanges();
        }

        /// <summary>
        /// Delete a item
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            var item = this.context.Items.Find(id);
            item.Status = "Deleted";
            this.context.SaveChanges();
        }


        /// <summary>
        /// Get all items 
        /// </summary>
        /// <returns></returns>
        public ICollection<Item> GetAll()
        {
            return this.context.Items
                .Include(x => x.Organisation)
                .Include(x => x.Price)
                .ToList();
        }

        /// <summary>
        /// Get a item by brand  and model
        /// </summary>
        /// <param name="brand"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ICollection<Item> GetAllByBrandAndModel(string brand, string model)
        {
            return this.context.Items
                .Include(x => x.Organisation)
                .Include(x=>x.Price)
                .Where(x => x.Brand.ToLower() == brand && x.Model.ToLower() == model)
                .ToList();
        }

        /// <summary>
        /// Reduce item quantity
        /// </summary>
        /// <param name="itemId"></param>
        public void ReduceItemQuantity(int itemId)
        {
            var item = this.context.Items.Find(itemId);
            item.Quantity--;
           
            this.context.SaveChanges();
        }

        /// <summary>
        /// Verify whether the item exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Exist(int id)
        {
            return this.context.Items.Any(x => x.Id == id);
        }
    }
}
