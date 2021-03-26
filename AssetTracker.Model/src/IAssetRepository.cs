using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MPEF.AssetTracker.Model
{
    /// <summary>
    /// An AssetRepository is responsible for saving and loading repositories,
    /// as well as maintaining a cache of repositories in memory.
    /// </summary>
    public interface IAssetRepository
    {
        /// <summary>
        /// The number of assets in the system.
        /// </summary>
        public int Count { get; }

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

        /// <summary>
        /// Retreive the Asset with the specified id. If no such Asset exists, the method returns null.
        /// </summary>
        Asset GetAsset(int id);

        /// <summary>
        /// Returns a collection of all assets in the system.
        /// </summary>
        IEnumerable<Asset> GetAssets();

        /// <summary>
        /// Returns a subset of all assets, filtered by the given lambda expression.
        /// </summary>
        IEnumerable<Asset> GetAssets(Func<Asset, bool> predicate);

        /// <summary>
        /// Returns an IQueryable pointing to the underlying Asset collection. This is useful when
        /// an object that can participate in Linq expressions is needed, and deferred execution can
        /// be taken advantage of.
        /// </summary>
        System.Linq.IQueryable<Asset> GetQueriable();

        /// <summary>
        /// Lets the system know that a given Asset has been changed.
        /// </summary>
        /// <param name="asset"></param>
        void UpdateAsset(Asset asset);

        /// <summary>
        /// Remove the Asset with the correspondin id. If the Asset does not exists nothing happens.
        /// </summary>
        void DeleteAsset(int id);
    }
}