using System.Collections.Generic;

namespace MP1.AssetTracker.Model
{
    /// <summary>
    /// An OfficeRepository is responsible for saving and loading repositories,
    /// as well as maintaining a cache of repositories in memory.
    /// </summary>
    public interface IOfficeRepository
    {
        void AddOffice(Office office);
        Office GetOffice(int id);
        Office GetOffice(string country);
        IEnumerable<Office> GetOffices();
        void UpdateOffice(Office office);
        void DeleteOffice(int id);
    }
}