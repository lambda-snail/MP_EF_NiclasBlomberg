using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MP1.AssetTracker.Model;

namespace MP1.AssetTracker.DataLayer
{
    public class AssetRepository : IAssetRepository
    {
        private AssetTrackerDbContext _db;

        public AssetRepository(AssetTrackerDbContext database) 
        {
            if(database == null)
            {
                throw new ArgumentNullException("AssetRepository: No database specified");
            }
            _db = database;
        }

        public void AddAsset(Dictionary<string, string> parameters)
        {
            switch(parameters["Type"])
            {
                case "Computer":
                    AddAsset(
                        new Computer(
                            DateTime.Parse(parameters["PurchaseDate"]),
                            DateTime.Parse(parameters["ExpiryDate"]),
                            double.Parse(parameters["Price"]),
                            parameters["ModelName"],
                            int.Parse(parameters["OfficeID"]),
                            parameters["OS"],
                            parameters["RAM"],
                            parameters["Processor"]
                            )
                    );
                    break;
                case "Cellphone":
                    AddAsset(
                        new Cellphone(
                            DateTime.Parse(parameters["PurchaseDate"]),
                            DateTime.Parse(parameters["ExpiryDate"]),
                            double.Parse(parameters["Price"]),
                            parameters["ModelName"],
                            int.Parse(parameters["OfficeID"]),
                            parameters["PhoneOperator"],
                            parameters["PhoneNumber"]
                            )
                    );
                    break;
                default:
                    throw new ArgumentException("Unknown asset type.");
            }
        }

        public void AddAsset(Asset asset)
        {
            if (asset == null)
                throw new ArgumentNullException("Argument null: AddAsset");

            _db.Add(asset);
            _db.SaveChanges();
        }

        /// <summary>
        /// Retreive the Asset with the specified id. If no such Asset exists, the method returns null.
        /// </summary>
        public Asset GetAsset(int id)
        {
            return _db.Assets.Find(id);
        }

        public IEnumerable<Asset> GetAssets()
        {
            return _db.Assets.ToList();
        }

        public void UpdateAsset(Asset asset)
        {
            _db.Update(asset);
            _db.SaveChanges();
        }

        public void DeleteAsset(int id)
        {
            var asset = _db.Assets.Find(id);
            if(asset != null)
            {
                _db.Remove(asset);
                _db.SaveChanges();
            }
        }
    }
}
