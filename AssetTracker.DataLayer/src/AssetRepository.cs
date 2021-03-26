using System;
using System.Collections.Generic;
using System.Linq;
using MPEF.AssetTracker.Model;

namespace MPEF.AssetTracker.DataLayer
{
    public class AssetRepository : IAssetRepository
    {
        private AssetTrackerDbContext _db;

        public int Count 
        { 
            get
            {
                return _db.Assets.Count();
            }
        }

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

        public Asset GetAsset(int id)
        {
            return _db.Assets.Find(id);
        }

        public IEnumerable<Asset> GetAssets()
        {
            return _db.Assets.ToList();
        }

        public IEnumerable<Asset> GetAssetsPaged(int pageSize, int pageIndex)
        {
            return _db.Assets.Skip(pageIndex * pageSize).Take(pageSize).ToList();
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
