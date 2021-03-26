using System.Collections.Generic;

namespace MPEF.AssetTracker.Model
{
    /// <summary>
    /// An AssetRepository is responsible for saving and loading repositories,
    /// as well as maintaining a cache of repositories in memory.
    /// </summary>
    public interface IAssetRepository
    {
        void AddAsset(Asset asset);

        /// <summary>
        /// Constructs an asset from the specified parameters.
        /// The parameters are:
        /// 
        /// Type
        /// PurchaseDate
        /// ExpiryDate
        /// Price
        /// ModelName
        /// OfficeID
        /// 
        /// If Type is "Computer" then in addition the following parameters will need to be set:
        /// 
        /// OS
        /// RAM
        /// Processor
        /// 
        /// If Type is "Cellphone" then in addition the following parameters will need to be set:
        /// 
        /// PhoneOperator
        /// PhoneNumber
        /// 
        /// TODO:
        ///   Should return type be int, returning the id of the new asset?
        /// 
        /// </summary>
        void AddAsset(Dictionary<string, string> parameters);
        Asset GetAsset(int id);
        IEnumerable<Asset> GetAssets();
        void UpdateAsset(Asset asset);
        void DeleteAsset(int id);
    }
}